using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PuzzleTrigger : MonoBehaviour
{
    [Header("트리거 작동 시 실행할 이벤트")]
    public UnityEvent OnTriggerActivated;

    [Header("트리거 설정")]
    [Tooltip("한 번만 작동? (스테이지 클리어는 1번만 작동해야함)")]
    public bool triggerOnlyOnce = true;
    
    private bool _triggered = false; //중복 실행 방지 안전장치 bool 변수

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //전에 이미 작동했다면 빠져나옴
        if (_triggered && triggerOnlyOnce) return;

        //Tag가 Player인지 확인
        if (collision.CompareTag("Player")) ActivateTrigger(collision.gameObject);
    }
    
    private void ActivateTrigger(GameObject player)
    {
        //트리거 작동
        _triggered = true;

        //Player 캐릭터의 움직임을 완전히 멈춤
        Rigidbody2D rb = null;
        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }
        }

        Debug.Log($"{gameObject.name} 퍼즐 트리거 작동. 클리어 연출 시작");

        //연결되어 있는 연출 이벤트 작동
        OnTriggerActivated?.Invoke();        
    }

    /*      
     * TODO: [스크린 매니저 연동] ScreenManager를 통한 FadeIn, FadeOut 추가
     * TODO: [세이브 매니저 연동] SaveloadManager를 통한 새로운 SavePoint 지정 스크립트 추가
     */

    //혹시 퍼즐이 초기화돼서 트리거도 다시 켜야 할 때를 대비한 함수
    public void ResetTrigger()
    {
        _triggered = false;
    }
}
