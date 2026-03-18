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

        Debug.Log($"[HourglassUI] 자원 변경: {previousResource} → {newResource}");
        anim.Play(GetIdleStateName(newResource));
        

        previousResource = newResource;
    }

    // 자원값에 해당하는 Idle State 이름 반환
    private string GetIdleStateName(int resource)
    {
        return resource switch
        {
            -2 => "Child2",
            -1 => "Child1",
            0 => "Youth1",
            1 => "Youth2",
            2 => "Elder",
            _ => "Youth1"
        };
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
