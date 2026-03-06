using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 역할:컷신 센서
/// 특정 구역에 플레이어가 닿으면 지정된 타임라인을 실행시켜줄 스크립트
/// </summary>
public class CutsceneTrigger : MonoBehaviour
{
    [Header("재생할 타임라인")]
    [SerializeField] PlayableDirector timelineToPlay;

    [Header("설정")]
    [Tooltip("체크 시 한 번 재생 후 이 트리거를 파괴")]
    [SerializeField] bool playOnlyOnce = true;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CinematicManager.Instance.PlayCutscene(timelineToPlay);

            playOnlyOnce = true;

            if (playOnlyOnce)
            {
                gameObject.SetActive(false);
            }

        }
        else
        {
            Debug.LogWarning("CinematicManager나 타임라인이 할당되지 않았습니다!");
        }
    }

}
