using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// [말풍선 UI 총괄 매니저]
/// 역할: 말풍선을 띄워주는 UI총괄 역할
/// 타임라인의 방송을 수신하고, 캐릭터 위치를 추적해 말풍선을 띄웁니다.
/// </summary>
public class DialogueUIManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [Tooltip("껐다 켰다 할 말풍선 전체 부모 오브젝트")]
    public GameObject speechBubbleRoot;
    [Tooltip("글자가 출력될 TextMeshPro 컴포넌트")]
    public TextMeshProUGUI dialogueTextUI;

    private static Dictionary<string, Transform> actorRegistry = new Dictionary<string, Transform>();

  
    // ActorProfile에서 자기를 등록할 때 쓰는 함수
    public static void RegisterActor(string id, Transform headTransform)
    {
        if (!actorRegistry.ContainsKey(id))
        {
            actorRegistry.Add(id, headTransform);
        }
    }

    public static void UnregisterActor(string id)
    {
        if (actorRegistry.ContainsKey(id))
        {
            actorRegistry.Remove(id);
        }
    }

    private void OnEnable()
    {
        // 타임라인 클립에서 쏘는 이벤트 구독!
        //DialogueBehaviour.OnBubbleStateChanged += HandleBubbleState;
    }

    private void OnDisable()
    {
        // 구독 해제
        // DialogueBehaviour.OnBubbleStateChanged -= HandleBubbleState;
    }

    /// <summary>
    /// 타임라인 클립에 들어오거나 나갈 때 자동 실행
    /// </summary>
    /*
    private void HandleBubbleState(bool isOn, DialogueLine line)
    {
        if (isOn)
        {
            // 1. 명부에서 화자(speakerID) 찾기
            if (actorRegistry.TryGetValue(line.speakerID, out Transform targetTransform))
            {
                // 2. 말풍선을 캐릭터 머리 위치로 이동
                // (World Space 캔버스를 사용하는 것을 추천합니다)
                speechBubbleRoot.transform.position = targetTransform.position;

                // 3. 텍스트 내용 적용
                dialogueTextUI.text = line.dialogueText;

                // 4. 말풍선 나타나기
                speechBubbleRoot.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"[DialogueUI] 씬에 '{line.speakerID}' 이름표를 가진 캐릭터가 없습니다!");
            }
        }
        else
        {
            // 타임라인 재생바가 클립을 벗어났으므로 말풍선 끄기
            speechBubbleRoot.SetActive(false);
        }
    }
    */
}
