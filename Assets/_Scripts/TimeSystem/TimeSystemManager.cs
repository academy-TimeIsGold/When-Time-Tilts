using System;
using UnityEngine;

/// <summary>
/// 시간 자원 및 모드 전역 관리 singleton
/// </summary>
public class TimeSystemManager : MonoBehaviour
{
    public static TimeSystemManager Instance { get; private set; }

    [Header("시간 자원 설정")]
    [SerializeField] private int maxResource = 2;           // 최대 시간 자원
    [SerializeField] private int minResource = -2;          // 최소 시간 자원
    [SerializeField] private int defaultResource = 0;       // 시작 및 리셋 기본값

    [Header("조작 1회 소모량")]
    [SerializeField] private int costPerInteract = 1;

    [Header("시간 모드 슬로우 설정")]
    [SerializeField] private float slowTimeScale = 0.3f;    // 모드 진입 배속
    [SerializeField] private float normalTimeScale = 1f;    // 기본 배속

    // 이벤트
    public event Action<int> OnResourceChanged;             // 자원 변경 시 이벤트
    public event Action<TimeMode> OnModeChanged;            // 모드 전환 시 이벤트

    // 현재 플레이어 상태
    private int currentResource;                            // 현재 시간 자원
    private TimeMode currentMode = TimeMode.None;           // 현재 시간 모드

    // 프로퍼티
    public int CurrentResource => currentResource;          // 현재 시간 자원
    public int MaxResource => maxResource;                  // 최대 시간 자원
    public int MinResource => minResource;                  // 최소 시간 자원
    public TimeMode CurrentMode => currentMode;             // 현재 시간 모드

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        currentResource = defaultResource;
    }

    #region 자원 관리

    // 시간 자원 증가
    public bool AddResource(int amount)
    {
        if (amount <= 0) return false;
        
        // 이미 최대치인 경우
        if (currentResource >= maxResource)
        {
            Debug.Log($"자원 최대치 - 현재: {currentResource}, 최대: {maxResource}");
            return false;
        }

        currentResource = Mathf.Min(currentResource + amount, maxResource);
        Debug.Log($"자원 증가 - 현재: {currentResource}");
        NotifyResourceChanged();
        return true;
    }

    // 시간 자원 소모
    public bool ConsumeResource(int amount)
    {
        if (amount <= 0) return false;

        // 자원이 부족한 경우
        if (currentResource - amount < minResource)
        {
            Debug.Log($"자원 부족 - 현재: {currentResource}, 필요: {amount}");
            return false;
        }

        currentResource -= amount;
        Debug.Log($"자원 감소 - 현재: {currentResource}");
        NotifyResourceChanged();
        return true;
    }

    // 시간 자원을 세이브 포인트 저장값으로 초기화
    // 세이브 지점의 자원 초기화 값이 다를 수 있으므로 매개변수로 받음
    public void ResetResource(int savedValue)
    {
        currentResource = Mathf.Clamp(savedValue, 0, maxResource);
        Debug.Log($"자원 초기화 - 디폴트 시간 자원: {savedValue}, 현재 자원: {currentResource}");
        NotifyResourceChanged();
    }

    #endregion

    #region 모드 관리

    // 모드 전환 => Player에서 입력 발생 시 호출
    public void SetMode(TimeMode mode)
    {
        TimeMode newMode = (currentMode == mode) ? TimeMode.None : mode; // 같은 모드 선택 시 해제
        if (currentMode == newMode) return;

        currentMode = newMode;
        Debug.Log($"[TimeSystem] 모드 전환: {currentMode}");
        ApplyTimeScale(currentMode != TimeMode.None);
        OnModeChanged?.Invoke(currentMode);
    }

    // 모드를 None으로 강제 해제
    public void ClearMode()
    {
        if (currentMode == TimeMode.None) return;
        currentMode = TimeMode.None;
        
        Debug.Log($"[TimeSystem] 모드 해제");
        ApplyTimeScale(false);  // 모드 해제 시 슬로우 모션 해제
        OnModeChanged?.Invoke(currentMode);
    }

    // 슬로우 모션 적용/해제
    private void ApplyTimeScale(bool slow)
    {
        Time.timeScale = slow ? slowTimeScale : normalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 물리 업데이트도 시간에 맞춰 조정
    }

    #endregion

    #region TimeObject 조작 중계

    // 플레이어가 TimeObject를 클릭 했을 때 호출
    public bool TryInteract(TimeObject target)
    {
        // 모드 확인
        if (currentMode == TimeMode.None)
        {
            Debug.Log("[TimeSystem] 모드 미선택");
            return false;
        }

        // 모드와 오브젝트 상태 확인
        if (!IsCompatible(currentMode, target.currentState))
        {
            Debug.Log($"[TimeSystem] 모드({currentMode})와 오브젝트 상태({target.currentState}) 불일치");
            return false;
        }

        bool resourceChanged = currentMode == TimeMode.Accelerate
            ? ConsumeResource(costPerInteract)
            : AddResource(costPerInteract);

        if (!resourceChanged)
        {
            Debug.Log("[TimeSystem] 자원 한계");
            return false;
        }

            target.Interact();
        Debug.Log($"[TimeSystem] 조작 성공 — {target.name} / 남은 자원: {currentResource}");
        return true;
    }

    // 현재 모드에서 해당 오브젝트 상태가 선택 가능한지 반환
    // Player 모드에서 오브젝트 하이라이트 필터링에 사용
    public bool IsCompatible(TimeMode mode, TimeState objectState)
    {
        return mode switch
        {
            TimeMode.Accelerate => objectState == TimeState.Past,
            TimeMode.Revert => objectState == TimeState.Future,
            _ => false
        };
    }

    #endregion

    #region Player 상태 변환 연동

    // 현재 자원값을 AgeState로 변환해서 반환
    public AgeState GetCurrentAgeState()
    {
        if (currentResource <= -1) return AgeState.Child;
        if (currentResource <= 1) return AgeState.Youth;
        return AgeState.Elder;
    }

    #endregion

    private void NotifyResourceChanged()
    {
        OnResourceChanged?.Invoke(currentResource);
    }

#if UNITY_EDITOR
    [ContextMenu("디버그: 자원 +1")]
    private void Debug_Add() => AddResource(1);

    [ContextMenu("디버그: 자원 -1")]
    private void Debug_Consume() => ConsumeResource(1);

    [ContextMenu("디버그: 자원 초기화")]
    private void Debug_Reset() => ResetResource(defaultResource);

    [ContextMenu("디버그: 가속 모드")]
    private void Debug_Accel() => SetMode(TimeMode.Accelerate);

    [ContextMenu("디버그: 회귀 모드")]
    private void Debug_Revert() => SetMode(TimeMode.Revert);

    [ContextMenu("디버그: 모드 해제")]
    private void Debug_Clear() => ClearMode();
#endif
}
