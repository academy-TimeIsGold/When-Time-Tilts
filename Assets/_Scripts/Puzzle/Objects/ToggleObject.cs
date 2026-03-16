using UnityEngine;

public class ToggleObject : PuzzleMechanism
{
    [Header("상태별 오브젝트 설정")]
    public GameObject defaultState; //기본 상태
    public GameObject activeState;  //활성화 상태

    private bool isToggled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetState(false);
    }

    //토글 스위치 ON
    public override void ActivateMechanism()
    {
        isToggled = true;
        SetState(true);
        PlaySound(onAccelSound);
    }

    //토글 스위치 OFF
    public override void DeactivateMechanism()
    {
        isToggled = false;
        SetState(false);
        PlaySound(offRevertSound);
    }

    //토글 스위치 눌렀을 때만 작동
    public void ToggleMechanism()
    {
        isToggled = !isToggled;
        SetState(isToggled);
        Debug.Log($"{gameObject.name} 스위치 작동. 현재 상태 {isToggled}");
    }

    private void SetState(bool isActive)
    {
        if (defaultState != null)
        {
            defaultState.SetActive(!isActive);            
        }

        if (activeState != null)
        {
            activeState.SetActive(isActive);            
        }
    }
}
