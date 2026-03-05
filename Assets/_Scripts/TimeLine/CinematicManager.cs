using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using System;
using System.Collections;

/// <summary>
/// 역할: 컷신 총괄 매니저
/// 타임라인 연출 총괄 매니저
/// </summary>
public class CinematicManager : MonoBehaviour
{
    // 조작 제어용 이벤트 (true: 연출 시작/조작 잠금, false: 연출 종료/조작 풀림)
    public static Action<bool> OnCinematicStateChanged;
    public static Action<string> OnDialogueRequested;

    [Header("현재 실행 중인 타임라인")]
    public PlayableDirector currentDirector;

    [Header("설정")]
    [Tooltip("레터박스가 다 내려올 때까지 기다릴 시간")]
    public float playDelay = 0.5f;

    [Header("컷신 종료 후 실행할 이벤트")]
    [Tooltip("인트로 씬이면 씬 로드 함수를, 인게임이면 비워두거나 필요한 연출을 연결")]
    public UnityEvent onCutsceneFinished;

    // 버튼 클릭(인트로) 또는 트리거(인게임)에서 호출
    public void PlayCutscene(PlayableDirector director)
    {
        currentDirector = director;

        // 1. 레터박스 애니메이션 시작!
        OnCinematicStateChanged?.Invoke(true);

        // 2. 타임라인 종료 이벤트 구독
        currentDirector.stopped += EndCutscene;

        // 3. 바로 Play() 하지 않고 대기 코루틴 실행!
        StartCoroutine(WaitAndPlayTimeline());
    }

    // 타임라인 재생이 끝났을 때 자동 호출
    private void EndCutscene(PlayableDirector director)
    {
        // 1. 이벤트 구독 해제
        director.stopped -= EndCutscene;

        // 2. 조작 원상복구
        OnCinematicStateChanged?.Invoke(false);
        currentDirector = null;

        // 3. 인스펙터에서 연결된 후속 작업 실행 (ex. 씬 이동)
        onCutsceneFinished?.Invoke();
    }

    private IEnumerator WaitAndPlayTimeline()
    {
        // playDelay(예: 0.5초) 만큼 멈춰서 기다림
        yield return new WaitForSeconds(playDelay);

        // 시간이 다 지나면 그제야 진짜로 타임라인 재생!
        if (currentDirector != null)
        {
            currentDirector.Play();
        }
    }
}
