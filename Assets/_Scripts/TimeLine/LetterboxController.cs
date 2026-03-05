using UnityEngine;
using System.Collections;

/// <summary>
/// 역할: 레터박스 애니메이터
/// CinematicManager의 연출 시작/종료 이벤트를 듣고 레터박스를 관리하는 스크립트
/// </summary>
public class LetterboxController : MonoBehaviour
{
    [Header("UI연결")]
    [SerializeField] RectTransform topBar;
    [SerializeField] RectTransform bottomBar;

    [Header("설정")]
    [Tooltip("레터박스가 나타나고 사라지는 데 걸리는 시간")]
    [SerializeField] float animationDuration = 0.5f;
    [Tooltip("레터박스의 높이 (화면 밖으로 숨길 때 사용)")]
    [SerializeField] float barHeight = 150f;

    private void OnEnable()
    {
        CinematicManager.OnCinematicStateChanged += HandleLetterbox;
    }

    private void OnDisable()
    {
        CinematicManager.OnCinematicStateChanged -= HandleLetterbox;
    }

    private void HandleLetterbox(bool isStarting)
    {
        StopAllCoroutines(); // 진행 중인 애니메이션 멈춤

        if (isStarting)
        {
            // 연출 시작: 화면 안(Y축 0)으로 슬라이드
            StartCoroutine(AnimateBars(0f));
        }
        else
        {
            // 연출 종료: 화면 밖(Y축 높이만큼)으로 슬라이드
            StartCoroutine(AnimateBars(barHeight));
        }
    }

    private IEnumerator AnimateBars(float targetY)
    {
        float elapsedTime = 0f;

        // 현재 위치 저장
        Vector2 topStartPos = topBar.anchoredPosition;
        Vector2 bottomStartPos = bottomBar.anchoredPosition;

        Vector2 topTargetPos = new Vector2(0, targetY);
        Vector2 bottomTargetPos = new Vector2(0, -targetY);

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            // 시간에 따른 보간(Lerp) 비율 계산
            float t = elapsedTime / animationDuration;

            // SmoothStep을 적용하면 시작과 끝이 더 자연스러워진다고해서 사용함
            t = t * t * (3f - 2f * t);

            topBar.anchoredPosition = Vector2.Lerp(topStartPos, topTargetPos, t);
            bottomBar.anchoredPosition = Vector2.Lerp(bottomStartPos, bottomTargetPos, t);

            yield return null; 
        }

        topBar.anchoredPosition = topTargetPos;
        bottomBar.anchoredPosition = bottomTargetPos;
    }
}

