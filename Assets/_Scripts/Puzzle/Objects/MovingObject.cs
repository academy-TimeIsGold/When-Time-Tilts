using UnityEngine;
using System.Collections;
using System;

public class MovingObject : PuzzleMechanism
{
    [Header("")]
    [Tooltip("발판을 밟았을 때 얼마나 이동할 것인지 (예: 위로 3칸 이동하려면 Y에 3 입력)")]
    public Vector3 moveOffset;

    [Tooltip("이동하는 데 걸리는 시간 (초)")]
    public float moveDuration = 1.0f;

    [Tooltip("제자리로 돌아오는 데 걸리는 시간(초)")]
    public float returnDuration = 2.0f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Coroutine moveCoroutine;

    private void Awake()
    {
        //게임 시작 시 현재 위치를 출발점으로 offset을 더한 위치를 도착점으로 기억함
        startPos = transform.position;
        targetPos = startPos + moveOffset;
    }

    //발판이 눌렸을 때 실행
    public override void ActivateMechanism()
    {
        //이동 중이라면 멈추고 새로운 목적치로 이동
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveRoutine(targetPos, moveDuration));
    }

    //발판에서 발이 떨어졌을 때 실행
    public override void DeactivateMechanism()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveRoutine(startPos, returnDuration));
    }

    private IEnumerator MoveRoutine(Vector3 destination, float duration)
    {
        Vector3 currentPos = transform.position;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.position = Vector3.Lerp(currentPos, destination, t);
            yield return null;
        }

        //소수점 오차를 막기위한 정확한 목적지 좌표로 고정
        transform.position = destination;
    }
}
