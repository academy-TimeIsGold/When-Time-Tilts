using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class DangerZone : MonoBehaviour
{
    [Header("리셋 연출 설정")]
    [SerializeField] private float resetDelay = 0.5f;
    [SerializeField] private GameObject dangerVFX;   //강제 리셋 시 발생하는 효과 (파티클, 스프라이트 등)

    private bool isTriggered = false; //중복 실행 방지 안전장치 bool 변수

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //player 태그 확인 & 트리거가 작동하지 않았을 때 적용
        if (!isTriggered && collision.CompareTag("Player"))
        {
            //리셋을 위한 플레이어 정보 넘겨줌
            TriggerRest(collision.gameObject);
        }
    }

    //리셋트리거 작동 함수
    private void TriggerRest(GameObject player)
    {
        isTriggered = true;
        
        //리셋 딜레이를 위한 코루틴
        StartCoroutine(ResetRoutine(player));
    }

    private IEnumerator ResetRoutine(GameObject player)
    {
        //Rigidbody 초기화
        Rigidbody2D rb = null;

        //Trigger 작동 시 player 멈춤
        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                //가속도 0
                rb.linearVelocity = Vector2.zero;

                //유니티 물리엔진 끄기 (Trigger가 작동된 지점에서 Player 완전 정지)
                rb.simulated = false;
            }
        }

        //리셋 효과 VFX 실행
        if (dangerVFX != null)
        {
            //효과가 켜져 있으면 강제로 비활성화 후 활성화 시켜 재생
            dangerVFX.SetActive(false);
            dangerVFX.SetActive(true);
        }

        //리셋 딜레이
        yield return new WaitForSeconds(resetDelay);

        //FadeOut
        if (ScreenManager.Instance != null)
        {
            yield return ScreenManager.Instance.FadeOut();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetToSavePoint();
        }
 
    }

    public bool IsPlayerInDanger()
    {
        return isTriggered;
    }
}
