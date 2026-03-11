using UnityEngine;

public class SkyToggleObject : TimeObject
{
    [Header("활성화 조건")]
    [SerializeField] private SkyState activeOn = SkyState.Day;

    private SkyController skyController;
    private Collider2D cd;

    protected override void Start()
    {
        base.Start();

        cd = GetComponent<Collider2D>();

        skyController = FindAnyObjectByType<SkyController>();
        if (skyController != null)
        {
            skyController.OnSkyChanged += UpdateState;
            UpdateState(skyController.CurrentSkyState);  // 즉시 동기화
        }
        
    }

    private void OnDestroy()
    {
        if (skyController != null)
        {
            skyController.OnSkyChanged -= UpdateState;
        }
    }

    private void UpdateState(SkyState state)
    {
        bool isActive = (state == activeOn);

        // 비주얼 오브젝트  활성/비활성
        if (pastState != null) pastState.SetActive(isActive && currentState == TimeState.Past);
        if (futureState != null) futureState.SetActive(isActive && currentState == TimeState.Future);

        // 비활성 시 콜라이더 꺼서 클릭/포커스 불가
        if (cd != null) cd.enabled = isActive;
    }
}
