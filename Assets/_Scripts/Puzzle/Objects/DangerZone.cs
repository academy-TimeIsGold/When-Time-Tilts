using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class DangerZone : MonoBehaviour
{
    [Header("리셋 연출 설정")]
    [SerializeField] private float _resetDelay = 0.5f;
    [SerializeField] private GameObject _dangerVFX;   //강제 리셋 시 발생하는 효과 (파티클, 스프라이트 등)

    private bool _triggered = false; //중복 실행 방지 안전장치 bool 변수

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //player 태그 확인 & 트리거가 작동하지 않았을 때 적용
        if (!_triggered && collision.CompareTag("Player"))
        {
            //리셋을 위한 플레이어 정보 넘겨줌
            TriggerRest(collision.gameObject);
        }
    }

    //리셋트리거 작동 함수
    private void TriggerRest(GameObject player)
    {
        _triggered = true;
        
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
        if (_dangerVFX != null)
        {
            //효과가 켜져 있으면 강제로 비활성화 후 활성화 시켜 재생
            _dangerVFX.SetActive(false);
            _dangerVFX.SetActive(true);
        }

        //리셋 딜레이
        yield return new WaitForSeconds(_resetDelay);

        //리셋 포인트로 이동 (임시)
        if (player !=null)
        {
            //플레이어 위치를 해당 지점으로 이동
            //(추후 세이브 포인트가 생기면 해당 부분 수정)
            player.transform.position = new Vector2(0, 3);           
            
            //유니티 물리엔진 켜기
            if (rb != null) rb.simulated = true;
        }

        //DangerZone 재활용을 위한 초기화
        if(_dangerVFX != null) _dangerVFX.SetActive(false);
        _triggered = false;
    }

    public bool IsPlayerInDanger()
    {
        return _triggered;
    }
}
