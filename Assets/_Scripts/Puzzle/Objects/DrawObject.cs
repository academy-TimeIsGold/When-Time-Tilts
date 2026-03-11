using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HingeJoint2D))]
public class DrawObject : PuzzleMechanism
{
    [Header("물리 설정")]
    [Tooltip("다리가 떨어지는 힘")]
    public float fallingGravityScale = 2.0f;

    [Tooltip("다리가 떨어졌을 때 최소 각도")]
    public float minFallingAngle = 90f;
    [Tooltip("다리가 떨어졌을 때 최대 각도")]
    public float targetFallingAngle = 90f;

    [Header("복구 설정")]
    [Tooltip("제자리로 돌아오는 데 걸리는 시간(초)")]
    public float returnDuration = 2.0f;

    private Rigidbody2D rb;
    private HingeJoint2D hinge;
    private JointAngleLimits2D limits;
    private bool isDropped = false;

    //초기 위치 기억용 변수
    private Vector3 startPos;
    private Quaternion startRot;

    //코루틴 추적용 변수
    private Coroutine returnCorutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hinge = GetComponent<HingeJoint2D>();

        //초기 물리 세팅
        rb.simulated = false;
        rb.gravityScale = 0f;

        //각도 제한
        limits = new JointAngleLimits2D();
        limits.min = minFallingAngle;
        limits.max = targetFallingAngle;

        hinge.limits = limits;
        hinge.useLimits = true;

        startPos = transform.position;
        startRot = transform.rotation;
    }

    //발판을 눌렀을 때 해당 각도만큼 변경
    public override void ActivateMechanism()
    {
        if (isDropped) return;
        isDropped = true;

        if(returnCorutine != null) StopCoroutine(returnCorutine);

        rb.simulated = true;
        rb.gravityScale = fallingGravityScale;
    }

    //발판에서 떨어졌을 때 초기위치로 변경
    public override void DeactivateMechanism()
    {
        if (!isDropped) return;
        isDropped = false;

        rb.simulated = false;        
        rb.gravityScale = 0f;

        if (returnCorutine != null) StopCoroutine(returnCorutine);
        returnCorutine = StartCoroutine(ReturnCorutine());
    }

    private IEnumerator ReturnCorutine()
    {
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        float timer = 0f;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float t = timer / returnDuration;

            //위치와 각도를 부드럽게 복구
            transform.position = Vector3.Lerp(currentPos, startPos, t);
            transform.rotation = Quaternion.Lerp(currentRot, startRot, t);

            yield return null;
        }
        
        //코루틴이 끝나면 처음 위치로 초기화
        transform.position = startPos;
        transform.rotation = startRot;
    }
}
