using System;
using System.Collections;
using UnityEngine;

public class SkyController : TimeObject
{
    [Header("회전 연출")]
    [SerializeField] private Transform pivot;               // 회전 중심
    [SerializeField] private float rotateDuration = 1.0f;   // 회전 연출 지속 시간

    [Header("플레이어 추적")]
    [SerializeField] private float followOffsetY = 4f;
    [SerializeField] private float followSpeed = 8f;

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

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
    }

    private void LateUpdate()
    {
        if (playerTarget == null) return;

        Vector3 targetPos = new Vector2(
            playerTarget.position.x,
            playerTarget.position.y + followOffsetY
            );
        transform.position = Vector2.Lerp(transform.position, targetPos, 
            followSpeed * Time.Time.unscaledDeltaTime);
    }

    public override void Interact()
    {
        if (isAnimating) return;

        TimeMode mode = TimeSystemManager.Instance.CurrentMode;

        if (mode == TimeMode.Accelerate && currentState == TimeState.Past)
        {

        }
    }

    // 회전 연출 코루틴
    private IEnumerator SwitchRoutine(TimeState targetState)
    {
        isAnimating = true;

        // 모드 해제 (슬로우 모션 해제)
        TimeSystemManager.Instance.ClearMode();

        // 입력 잠금
        
    }
}
