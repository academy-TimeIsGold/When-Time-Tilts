using System.Collections;
using UnityEngine;

public class NpcAmbient : MonoBehaviour
{
    [Header("NPC 설정")]
    [Tooltip("CSV의 npc_id 컬럼값과 정확히 동일하게 입력")]
    [SerializeField] private string npcId;

    [Header("말풍선 UI - UI 완성 후 연결")]
    [SerializeField] private SpeechBubbleUI bubbleUI;

    [Header("대사 출력 간격")]
    [SerializeField] private float minInterval = 5f; // 최소 간격 (초)
    [SerializeField] private float maxInterval = 10f; // 최대 간격 (초)

    // 현재 진행 중인 그룹 ID
    // null -> 처음(entry)부터 시작
    // 값 있음 -> 순서 진행 중
    private string currentGroupId = null;

    private void Start()
    {
        StartCoroutine(AmbientRoutine());
    }

    private IEnumerator AmbientRoutine()
    {
        // DailogueSystem 로드 완료 대기
        while (DialogueSystem.Instance == null || !DialogueSystem.Instance.IsLoaded)
        {
            yield return null;
        }

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            AgeState playerAge = TimeSystemManager.Instance.GetCurrentAgeState();
            DialogueSystem.Instance.PlayAmbientDialogue(npcId, playerAge, ref currentGroupId, bubbleUI);
        }
    }
}
