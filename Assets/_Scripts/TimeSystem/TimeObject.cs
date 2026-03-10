using UnityEngine;

public class TimeObject : MonoBehaviour, IInteractable, IFocusable
{
    [Header("상태별 외형 설정")]
    public GameObject pastState;       //과거
    public GameObject futureState;     //미래

    [Header("초기 상태 설정")]
    public TimeState currentState = TimeState.Future;   //처음 배치될 때 기본 상태

    [Header("포커스 시각 효과")]
    [SerializeField] private SpriteRenderer outlineRenderer;

    public bool isInteractable = true;   //상호작용 가능 여부 (SkyReactor 등에서 사용)
    
    protected virtual void Start()
    {
        UpdateVisual();
    }

    [ContextMenu("테스트: 상호작용 실행")]
    public virtual void Interact()
    {
        //오브젝트의 현 상태에 따라 변경
        if (currentState == TimeState.Past) Accelerate();
        else Revert();            
    }

    public virtual void SetFocus(bool isFocused)
    {
        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = isFocused;
        }
    }

    public virtual void Accelerate()
    {
        if (currentState == TimeState.Future) return;

        currentState = TimeState.Future;
        UpdateVisual();        
    }

    public virtual void Revert()
    {
        if (currentState == TimeState.Past) return;

        currentState = TimeState.Past;
        UpdateVisual();        
    }

    protected virtual void UpdateVisual()
    {
        if (pastState != null) pastState.SetActive(currentState == TimeState.Past);
        if (futureState != null) futureState.SetActive(currentState == TimeState.Future);
    }
}
