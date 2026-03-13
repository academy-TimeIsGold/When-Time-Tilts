using UnityEngine;

public class HourglassUI : MonoBehaviour
{
    private Animator anim;
    private int previousResource;

    private void Start()
    {
        anim = GetComponent<Animator>();

        if (TimeSystemManager.Instance == null)
        {
            Debug.LogWarning("[HourglassUI] TimeSystemManager 없음");
            return;
        }

        // 초기 자원값으로 Idle 상태 설정
        previousResource = TimeSystemManager.Instance.CurrentResource;
        Debug.Log($"[HourglassUI] 초기 자원: {previousResource}, State: {GetIdleStateName(previousResource)}"); 
        anim.Play(GetIdleStateName(previousResource));

        // 이벤트 구독
        TimeSystemManager.Instance.OnResourceChanged += OnResourceChanged;
        Debug.Log("[HourglassUI] 이벤트 구독 완료");
    }

    private void OnDestroy()
    {
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

    private void OnResourceChanged(int newResource)
    {
        int diff = Mathf.Abs(newResource - previousResource);

        if (diff == 1)
        {
            string transitionState = GetTransitionStateName(previousResource, newResource);
            Debug.Log($"[HourglassUI] 자원 변경: {previousResource} → {newResource}, State: {transitionState}");
            anim.Play(transitionState);
        }
        else
        {
            // 2칸 이상 변화 - 바로 Idle 상태로
            string idleState = GetIdleStateName(newResource);
            Debug.Log($"[HourglassUI] 자원 점프: {previousResource} → {newResource}, State: {idleState}");
           anim.Play(idleState);
        }
        previousResource = newResource;
    }

    // 자원값에 해당하는 Idle State 이름 반환
    private string GetIdleStateName(int resource)
    {
        return resource switch
        {
            -2 => "Down2",
            -1 => "Down1",
            0 => "DefualtIdle",
            1 => "Up1",
            2 => "Up2",
            _ => "DefualtIdle"
        };
    }

    // 이전값 -> 현재값 전환 애니메이션 State 이름 반환
    private string GetTransitionStateName(int from, int to)
    {
        // 감소
        if (to < from)
        {
            return from switch
            {
                2 => "GrowUp2_Rev",     // 2 -> 1
                1 => "GrowUp1_Rev",     // 1 -> 0
                0 => "GrowDown1",       // 0 -> -1
                -1 => "GrowDown2",      // -1 -> -2
                _ => GetIdleStateName(to)
            };
        }
        // 증가
        else
        {
            return from switch
            {
                -2 => "GrowDown2_Rev",  // -2 -> -1
                -1 => "GrowDown1_Rev",  // -1 -> 0
                0 => "GrowUp1",         // 0 -> 1
                1 => "GrowUp2",         // 1 -> 2
                _ => GetIdleStateName(to)
            };
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);  
    }
}
