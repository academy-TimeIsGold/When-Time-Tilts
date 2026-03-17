using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class OldPlatform : MonoBehaviour
{
    [Header("추락 설정")]
    [Tooltip("오브젝트 추락까지 소요 시간(초)")]
    public float fallDelay = 1.3f;
    [Tooltip("오브젝트 추락 중력")]
    public float fallingGravity = 3.0f;

    [Header("복구 설정")]
    [Tooltip("오브젝트 리스폰 시간")]
    public float respawnDelay = 3.0f;

    [Header("진동 설정")]
    [Tooltip("오브젝트 흔들 강도")]
    public float shakeMagnitude = 0.1f;
    [Tooltip("오브젝트 흔들 속도")]
    public float shakeSpeed = 20.0f;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isTriggered = false;

    private Transform originalParent;
    private Transform respawnAnchor;
    private Vector3 startPos;
    private Quaternion startRot;    

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        //Platform 초기 위치 저장
        originalParent = transform.parent;

        //본인 자리에 투명한 앵커를 생성
        GameObject ancherObject = new GameObject(gameObject.name + "_Ancor");

        //부모가 있다면 앵커도 자식으로 넣어 같이 회전
        if (originalParent != null) ancherObject.transform.SetParent(originalParent);

        //앵커 위치에 현재 발판의 위치로 고정
        ancherObject.transform.position = transform.position;
        respawnAnchor = ancherObject.transform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //트리거된 대상의 태그가 Player이고 발동되지 않은 상태이면
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            //플레이어가 발판보다 위에서 밟았을 때만 작동
            if (collision.transform.position.y > transform.position.y)
            {
                isTriggered = true;
                Debug.Log($"[{gameObject.name}] 밟음 감지! 추락을 시작합니다.");
                StartCoroutine(FallRoutine());
            }
        }
    }

    private IEnumerator FallRoutine()
    {
        //효과음
        //if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("Platform", 0);
        
        transform.SetParent(null);

        float timer = 0f;
        while (timer < fallDelay)
        {
            timer += Time.deltaTime;

            //사인파(Mathf.Sin)와 시간(timer)을 이용해 빠르게 변하는 무작위 값을 계산
            float randomX = (Mathf.Sin(timer * shakeSpeed) + Random.Range(-1f, 1f)) * shakeMagnitude;
            float randomY = (Mathf.Sin((timer + 0.1f) * shakeSpeed) + Random.Range(-1f, 1f)) * shakeMagnitude;

            //관람차를 기준으로 흔들리도로
            transform.localPosition = respawnAnchor.position + new Vector3(randomX, randomY, 0f);

            //물리 엔진 동기화
            rb.position = transform.position;                                   

            yield return null;
        }
        
        //추락 직전 위치를 원래대로 맞춤
        transform.localPosition = respawnAnchor.position;
        rb.position = transform.position;

        //Trigger 변경
        col.isTrigger = true;

        //물리 엔진 상태를 변경하여 오브젝트 추락
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallingGravity;

        //추락 후 리스폰 시간만큼 대기
        yield return new WaitForSeconds(respawnDelay);

        ResetPlatform();
    }

    private void ResetPlatform()
    {
        //다시 고정
        rb.bodyType = RigidbodyType2D.Kinematic;

        //가속도, 회전력 초기화
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        //원래 부모 오브젝트로 복귀
        transform.SetParent(originalParent);

        //다시 초기 위치값 저장
        transform.position = respawnAnchor.position;
        transform.rotation = startRot;        
        rb.position = transform.position;

        //콜리더 변경
        col.isTrigger = false;

        //발판 초기화
        isTriggered = false;
    }
}
