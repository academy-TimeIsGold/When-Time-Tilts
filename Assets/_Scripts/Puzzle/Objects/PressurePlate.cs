using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PressurePlate : MonoBehaviour
{
    [Header("상호작용 필요 나이 조건")]
    public AgeState requiredAge = AgeState.Elder;

    [Header("발판 이벤트 연결")]
    public UnityEvent OnPlantePressed;
    public UnityEvent OnPlanteReleased;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Tag가 Player인지 확인
        if (other.CompareTag("Player"))
        {
            AgeState currentAge = GetPlayerAgeFromManager();

            //현재 나이가 필요 나이조건에 맞으면 실행
            if (currentAge == requiredAge) Press();
            else Debug.Log($"조건이 맞지 않아 발판 작동이 안됩니다. (현재 나이: {currentAge}");
        }               
    }

    private void OnTriggerExit2D(Collider2D other)
    {   
        //Player가 Trigger를 빠져 나가면 실행
        if (other.CompareTag("Player")) Released();
    }

    #region 발판 작동 로직    
    private void Press()
    {
        Debug.Log($"{gameObject.name} 발판 작동");
        
        //Object의 Vector3의 y값을 -0.1f 만큼 이동
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
        
        //발판이 눌렸다고 알림
        OnPlantePressed.Invoke();
    }


    private void Released()
    {
        Debug.Log($"{gameObject.name} 발판 초기화");

        //Object의 Vector3의 y값을 +0.1f 만큼 이동
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);

        //발판이 초기화 됐다고 알림
        OnPlanteReleased.Invoke();
    }
    #endregion


    //임시로 제작한 현 플레이어 나이
    private AgeState GetPlayerAgeFromManager()
    {
        //무조건 Elder 나이로 반환
        return AgeState.Elder;
    }
}
