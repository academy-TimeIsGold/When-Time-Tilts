using System.Collections;
using UnityEngine;

public class SkyReactor : TimeObject
{
    [Header("하늘 반응 설정")]
    [Tooltip("반응할 SkyState 조건")]
    [SerializeField] private SkyState activeOn = SkyState.Day;

    [Tooltip("조건 충족 후 상태 전환까지 대기 시간")]
    [SerializeField] private float triggerDelay = 3f;

    [Tooltip("전환 완료 후 Revert() 차단 여부, true = 상태 영구 유지")]
    [SerializeField] private bool lockAfterTransition = false;

    [Tooltip("전환 완료 후 활성화할 오브젝트. 없으면 교체 안함")]
    [SerializeField] private TimeObject afterObject;

    private Coroutine triggerCoroutine;
    private SkyController skyController;
    private float accumulatedTime = 0f;     // 누적 조건 충족 시간
    private bool isTransitioned = false;    // 전환 완료 여부

    protected override void Start()
    {
        isInteractable = false;     // 플레이어 상호작용 불가

        base.Start();

        if (afterObject != null)
        {
            afterObject.gameObject.SetActive(false); // 초기에는 비활성화
        }

        skyController = FindAnyObjectByType<SkyController>();
        if (skyController != null)
        {
            skyController.OnSkyChanged += OnSkyChanged;
            OnSkyChanged(skyController.CurrentSkyState);
        }
        else
        {
            Debug.LogWarning($"[SkyReactor] {gameObject.name}: SkyController를 찾을 수 없습니다.");
        }
    }

    private void OnDisable()
    {
        if (skyController != null)
        {
            skyController.OnSkyChanged -= OnSkyChanged;
        }
    }

    private void OnSkyChanged(SkyState state)
    {
        // 전환 완료 + 잠금 상태면 이후 SkyState 변화 무시
        if (isTransitioned && lockAfterTransition) return;

        if (state == activeOn)
        {
            if (triggerCoroutine != null) return;   // 이미 코루틴이 실행 중이면 중복 시작 방지
            if (isTransitioned) return;             // 전환 완료 후 코루틴 시작 차단
            triggerCoroutine = StartCoroutine(TriggerAfterDelay());
        }
        else
        {
            // 조건 해제: 타이머 중단, 누적 시간 유지
            if (triggerCoroutine != null)
            {
                StopCoroutine(triggerCoroutine);
                triggerCoroutine = null;
            }

            // lockAfterTransition = false일 때만 Revert()
            if (!lockAfterTransition)
            {
                base.Revert();
            }
        }

    }

    private IEnumerator TriggerAfterDelay()
    {
        while (accumulatedTime < triggerDelay)
        {
            accumulatedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        triggerCoroutine = null;
        isInteractable = true;
        base.Accelerate();

        // afterObject 연결 시 오브젝트 교체
        if (afterObject != null)
        {
            gameObject.SetActive(false);
            afterObject.gameObject.SetActive(true);
        }
    }
    // 플레이어 조작 불가
    public override void Interact() { }
    public override void SetFocus(bool isFocused) { }
}
