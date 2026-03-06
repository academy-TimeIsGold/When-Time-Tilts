using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [Tooltip("캐릭터 대사(웅나나 등) 효과음을 재생할 오디오 소스")]
    public AudioSource audioSource;

    [Header("타이핑 설정")]
    [Tooltip("글자가 쳐지는 기본 속도 (0.05초면 1초에 20글자)")]
    public float baseTypeSpeed = 0.05f;

    // 말풍선이 따라다녀야할 타겟의 위치를 기억할 변수
    private Transform currentTargetTransform;

    private static Dictionary<string, Transform> actorRegistry = new Dictionary<string, Transform>();

    // 현재 타이핑 중인 코루틴을 추적 (안전장치용)
    private Coroutine typingCoroutine;


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
        DialogueBehaviour.OnBubbleStateChanged += HandleBubbleState;
    }

    private void OnDisable()
    {
        // 구독 해제
        DialogueBehaviour.OnBubbleStateChanged -= HandleBubbleState;
    }

    private void OnDestroy()
    {
        actorRegistry.Clear();
    }

    // 말풍선이 캐릭터를 따라다니게 
    private void LateUpdate()
    {
        if (speechBubbleRoot.activeSelf && currentTargetTransform != null)
        {
            speechBubbleRoot.transform.position = currentTargetTransform.position;
        }
    }

    /// <summary>
    /// 타임라인 클립에 들어오거나 나갈 때 자동 실행
    /// </summary>
    private void HandleBubbleState(bool isOn, DialogueLine line)
    {
        // 1. 기존에 돌고 있던 타이핑 애니메이션이 있다면 강제 중지 (UI 버그 방지)
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (isOn)
        {
            if (actorRegistry.TryGetValue(line.speakerID, out Transform targetTransform))
            {
                currentTargetTransform = targetTransform;

                // 말풍선 위치 이동 및 활성화
                speechBubbleRoot.transform.position = targetTransform.position;
                speechBubbleRoot.SetActive(true);

                // 2. 타이핑 코루틴 시작
                typingCoroutine = StartCoroutine(TypeSentence(line));
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

            currentTargetTransform = null;
        }
    }

    /// <summary>
    /// 글자를 한 글자씩 보여주는 코루틴
    /// </summary>
    private IEnumerator TypeSentence(DialogueLine line)
    {
        // 1. 텍스트 내용 적용 및 보이는 글자 수 0으로 초기화
        dialogueTextUI.text = line.dialogueText;
        dialogueTextUI.maxVisibleCharacters = 0;

        // 2. 배속 적용 계산 (Multiplier가 0일 경우 에러 방지를 위해 최소값 0.1 세팅)
        float speedMultiplier = Mathf.Max(0.1f, line.typeSpeedMultiplier);
        float currentTypeSpeed = baseTypeSpeed / speedMultiplier;

        // 3. 음성 파일(voiceClip)이 있다면 오디오 재생
        if (line.voiceClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(line.voiceClip);
        }

        // 4. 한 글자씩 타다닥 나타나게 하기
        int totalCharacters = line.dialogueText.Length;
        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueTextUI.maxVisibleCharacters = i;

            // 공백(띄어쓰기)일 때는 멈추지 않고 바로 다음 글자로 넘어가게해서 더 자연스럽게 처리
            if (i < totalCharacters && line.dialogueText[i] != ' ')
            {
                yield return new WaitForSeconds(currentTypeSpeed);
            }
        }
    }
}
