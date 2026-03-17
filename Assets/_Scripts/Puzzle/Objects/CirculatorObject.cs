using UnityEngine;

public class CirculatorObject : MonoBehaviour
{
    [Header("회전 설정")]
    [Tooltip("회전 속도 (양수:반시계, 음수:시계")]
    public float rotationSpeed = 30f;

    [Header("자동 배치")]
    [Tooltip("오브젝트용 원의 반지름")]
    public float radius = 5f;
  
    private void LateUpdate()
    {
        //부모 오브젝트를 지정 속도만큼 돌림
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        //자식 오브젝트의 각도를 0으로 고정
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child != null)
            {
                //(0, 0, 0)으로 고정
                child.rotation = Quaternion.identity;
            }
        }
    }

    [ContextMenu("발판 둥글게 자동 배치하기 (Auto Arrange)")]
    private void ArrangePlatformsInCircle()
    {
        int childCount = transform.childCount;
        
        if (childCount == 0)
        {
            Debug.Log("배치한 오브젝트가 없습니다.");
            return;
        }

        //360도를 자식 오브젝트의 수만큼 나눠 계산
        float angleStep = 360f / (float)childCount;

        for (int i = 0; i < childCount; i++)
        {
            //각도를 라디안으로 변환
            float angle = i * angleStep * Mathf.Deg2Rad;

            //삼각함수를 이용해 원형 좌표 계산
            Vector3 newPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

            //자식 오브젝트의 위치를 원형 좌표 계산 값으로 이동
            transform.GetChild(i).localPosition = newPos;
        }

        Debug.Log($"{gameObject.name}] {childCount}개의 발판을 원형으로 배치했습니다.");
    }
}
