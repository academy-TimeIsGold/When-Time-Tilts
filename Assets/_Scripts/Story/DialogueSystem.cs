using System;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    private NpcDialogueDatabase database;
    private NpcDialogueLoader loader;

    public bool IsLoaded => loader != null && loader.IsLoaded;

    public Action onDialogueLoaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        database = new NpcDialogueDatabase();
        loader = GetComponent<NpcDialogueLoader>();

        if (loader == null)
        {
            loader = gameObject.AddComponent<NpcDialogueLoader>();
        }
    }

    private void Start()
    {
        loader.onLoadComplete += () => onDialogueLoaded?.Invoke();
        loader.Initialize(database);
    }

    // NPC 말풍선 대사 시작
    public void PlayAmbientDialogue(string npcId, AgeState playerAge,
        ref string currentGroupId, SpeechBubbleUI bubbleUI)
    {
        if (!IsLoaded) return;

        NpcDialogueDef def;

        if (string.IsNullOrEmpty(currentGroupId))
        {
            // 처음 시작 또는 순서 완료 후 재시작 -> entry에서 랜덤 선택
            if(!database.TryGetEntryDialogue(npcId, playerAge, out def)) return;
        }
        else
        {
            // 순서 진행 중 -> 현재 그룹에서 대사 가져오기
            if(!database.TryGetRandom(currentGroupId, playerAge, out def)) return;
        }

        // 대사 출력
        if (bubbleUI != null)
        {
            bubbleUI.ShowDialogue(def.speaker, def.textKr, def.displayTime);
        }
        else
        {
            Debug.Log($"[DialogueSystem] AMBIENT — {def.speaker}: {def.textKr}");
        }

        // 다음 그룹으로 전진 (없으면 null -> 다음 호출 시 처음부터)
        currentGroupId = string.IsNullOrEmpty(def.nextGroupId) ? null : def.nextGroupId;
    }
}
