using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TimeObject : MonoBehaviour, IInteractable
{
    [Header("상태별 외형 설정")]
    public GameObject pastState;       //과거
    public GameObject futureState;     //미래

    [Header("초기 상태 설정")]
    public TimeState currentState = TimeState.Future;   //처음 배치될 때 기본 상태

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVisual();
    }

    [ContextMenu("테스트: 상호작용 실행")]
    public void Interact()
    {
        //오브젝트의 현 상태에 따라 변경
        if (currentState == TimeState.Past) Accelerate();
        else Revert();            
    }

    public void Accelerate()
    {
        if (currentState == TimeState.Future) return;

        currentState = TimeState.Future;
        UpdateVisual();        
    }

    public void Revert()
    {
        if (currentState == TimeState.Past) return;

        currentState = TimeState.Past;
        UpdateVisual();        
    }

    private void UpdateVisual()
    {
        if (pastState != null) pastState.SetActive(currentState == TimeState.Past);
        if (futureState != null) futureState.SetActive(currentState == TimeState.Future);
    }
}
