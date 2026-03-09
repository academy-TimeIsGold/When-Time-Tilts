using System;
using System.Collections;
using UnityEngine;

public class SkyController : TimeObject
{
    [Header("회전 연출")]
    [SerializeField] private Transform pivot;               // 회전 중심
    [SerializeField] private float rotateDuration = 1.0f;   // 회전 연출 지속 시간

    [Header("플레이어 연결")]
    [SerializeField] private Transform playerTarget;

    public event Action<SkyState> OnSkyChanged;

    public SkyState CurrentSkyState => (currentState == TimeState.Past) ? SkyState.Day : SkyState.Night;

    private bool isAnimating = false;                       // 연출 중인지
    public bool IsAnimating => isAnimating;

    protected override void Start()
    {
        base.Start();
        SetFocus(false);
    }

    public override void Interact()
    {
        if (isAnimating) return;

        TimeMode mode = TimeSystemManager.Instance.CurrentMode;

        if (mode == TimeMode.Accelerate && currentState == TimeState.Past)
        {
            if (!TimeSystemManager.Instance.ConsumeResource(1)) return;
            StartCoroutine(SwitchRoutine(TimeState.Future));
        }
        else if (mode == TimeMode.Revert && currentState == TimeState.Future)
        {
            if (!TimeSystemManager.Instance.AddResource(1)) return;
            StartCoroutine(SwitchRoutine(TimeState.Past));
        }
    }

    // 회전 연출 코루틴
    private IEnumerator SwitchRoutine(TimeState targetState)
    {
        isAnimating = true;

        // 모드 해제 (슬로우 모션 해제)
        TimeSystemManager.Instance.ClearMode();

        // 입력 잠금 + 시간 정지
        GameManager.Instance.SetInputLock(true);
        Time.timeScale = 0f;

        // 해/달 둘 다 켜기
        if (pastState != null) pastState.SetActive(true);
        if (futureState != null) futureState.SetActive(true);

        // Pivot 180도 회전
        float elapsed = 0f;
        Quaternion startRot = pivot.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 180f);

        while (elapsed < rotateDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            // 가속도가 붙었다가 천천히 멈추는 느낌을 주기 위함
            float t = Mathf.SmoothStep(0f, 1f, elapsed / rotateDuration);
            pivot.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
        pivot.rotation = endRot;

        // 상태 전환 + 비주얼 갱신
        currentState = targetState;
        UpdateVisual();

        // SkyState 이벤트 발행
        OnSkyChanged?.Invoke(CurrentSkyState);

        // 6. 시간 복구 + 입력 잠금 해제
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        GameManager.Instance.SetInputLock(false);

        isAnimating = false;
    }
}
