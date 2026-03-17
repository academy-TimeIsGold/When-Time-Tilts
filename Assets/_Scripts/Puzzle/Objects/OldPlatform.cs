using UnityEngine;
using System.Collections;
using System;

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
        //설정한 추락 시간까지 대기
        yield return new WaitForSeconds(fallDelay);

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
