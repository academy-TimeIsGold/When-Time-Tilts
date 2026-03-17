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

    [Tooltip("오브젝트 리스폰 시간")]
    public float respawnDelay = 3.0f;

    [Tooltip("오브젝트 흔들 강도")]
    public float shakeMagnitude = 0.1f;

    [Tooltip("오브젝트 흔들 속도")]
    public float shakeSpeed = 20.0f;

    private Rigidbody2D rb;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool isTriggered = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        //Platform 초기 위치 저장
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //트리거된 대상의 태그가 Player이고 발동되지 않은 상태이면
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                isTriggered = true;
                StartCoroutine(FallRoutine());
            }
        }
    }

    private IEnumerator FallRoutine()
    {
        //효과음
        //if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("Platform", 0);

        float timer = 0f;
        while (timer < fallDelay)
        {
            timer += Time.deltaTime;

            //사인파(Mathf.Sin)와 시간(timer)을 이용해 빠르게 변하는 무작위 값을 계산
            float randomX = (Mathf.Sin(timer * shakeSpeed) + Random.Range(-1f, 1f)) * shakeMagnitude;
            float randomY = (Mathf.Sin((timer + 0.1f) * shakeSpeed) + Random.Range(-1f, 1f)) * shakeMagnitude;

            //원래 위치(startPos)에 계산된 무작위 값을 더해 흔들리는 효과 적용
            transform.position = startPos + new Vector3(randomX, randomY, 0f);

            //한 프레임 쉼
            yield return null;
        }
        
        //추락 직전 위치를 원래대로 맞춤
        transform.position = startPos;

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

        //다시 초기 위치값 저장
        transform.position = startPos;
        transform.rotation = startRot;

        //발판 초기화
        isTriggered = false;
    }
}
