using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PuzzleTrigger : MonoBehaviour
{
    [Header("트리거 설정")]    
    public bool triggerOnlyOnce = true;

    [Header("클리어 연출: 자동 이동 설정")]
    [Tooltip("자동으로 이동할 X축 속도 (오른쪽: 양수 / 왼쪽: 음수")]
    public float autoMoveSpeed = 3f;
    [Tooltip("자동 이동을 유지할 시간 (초)")]
    public float autoMoveDuration = 2f;

    [Header("다음 세이브 포인트")]
    [Tooltip("이 스테이지 클리어 후 이동할 세이브 포인트 데이터")]
    [SerializeField] private SavePointData nextSavePointData;
    
    private bool isTriggered = false; //중복 실행 방지 안전장치 bool 변수    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //전에 이미 작동했다면 빠져나옴
        if (isTriggered && triggerOnlyOnce) return;

        //Tag가 Player인지 확인
        if (collision.CompareTag("Player"))
        {
            //트리거 작동
            isTriggered = true;

            Debug.Log($"{gameObject.name} 퍼즐 트리거 작동. 자동 이동 및 클리어 연출 시작");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartStageClearSequence(this, collision.gameObject);
            }

            else
            {
                Debug.Log("GameManger가 없습니다. 확인바랍니다.");
            }
        }       
    }

    public void StartCinematicWalk(GameObject player)
    {
        StartCoroutine(CinematicWalkRoutine(player));
    }

    private IEnumerator CinematicWalkRoutine(GameObject player)
    {
        //컴포넌트 초기화
        Rigidbody2D rb = player.GetComponentInChildren<Rigidbody2D>();                
        
        if (rb != null)
        {            
            float timer = 0f;

            //지정된 자유 이동 시간 동안 반복
            while (timer < autoMoveDuration)
            {                              
                //Y축은 그대로, X축만 지정 속도로 이동
                rb.linearVelocity = new Vector2(autoMoveSpeed, rb.linearVelocity.y);
                rb.gravityScale = 0f;                

                timer += Time.deltaTime;
                yield return null;
            }

            //화면 FadeOut
            yield return ScreenManager.Instance.FadeOut();

            //이동이 끝나면 정지
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            rb.gravityScale = 1f;
        }

        // 다음 세이브 포인트로 GameManager 갱신 + 파일 저장
        if (nextSavePointData != null)
        {
            GameManager.Instance.UpdateSavePoint(null, nextSavePointData);

            if (player != null)
            {
                player.transform.position = nextSavePointData.playerPosition;
            }
        }
        else
        {
            Debug.LogWarning($"[PuzzleTrigger] {gameObject.name}: nextSavePointData가 연결되지 않았습니다.");
        }

        //화면 FadeIn
        yield return ScreenManager.Instance.FadeIn();

        Debug.Log("자동 걷기 연출 종료");
        GameManager.Instance.EndStageClearSequence();       
    }

    //혹시 퍼즐이 초기화돼서 트리거도 다시 켜야 할 때를 대비한 함수
    public void ResetTrigger()
    {
        isTriggered = false;
    }
}
