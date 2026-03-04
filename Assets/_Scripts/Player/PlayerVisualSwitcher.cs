using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("시간 자원 설정 (테스트용)")]
    [SerializeField] private int currentTime = 2; // 현재 시간 게이지 (기본값 2)
    private const int _maxTime = 4;              // 최대치
    private const int _minTime = 0;              // 최소치

    [Header("캐릭터 오브젝트 연결")]
    [SerializeField] private GameObject childObject;  // 아이 (게이지 4)
    [SerializeField] private GameObject youthObject;  // 청년 (게이지 2~3)
    [SerializeField] private GameObject elderObject;  // 노인 (게이지 0~1)

    private void Start()
    {
        // 게임 시작 시 현재 수치에 맞춰 비주얼 초기화
        UpdateVisual();
    }

    private void Update()
    {
        // 1번 키: 시간 감소 (나이 듦)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AdjustTime(-1);
        }

        // 2번 키: 시간 증가 (어려짐)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AdjustTime(1);
        }
    }

    // 시간 게이지 조절 및 클램프 처리
    private void AdjustTime(int amount)
    {
        // 0 밑으로 내려가지 않고 4를 넘지 않게 제한
        currentTime = Mathf.Clamp(currentTime + amount, _minTime, _maxTime);
        Debug.Log($"현재 시간 게이지: {currentTime}");

        // 수치가 변했으므로 비주얼 갱신
        UpdateVisual();
    }

    // 게이지 수치에 따라 오브젝트 활성화/비활성화
    private void UpdateVisual()
    {
        // 우선 모든 오브젝트를 비활성화
        if (childObject != null) childObject.SetActive(false);
        if (youthObject != null) youthObject.SetActive(false);
        if (elderObject != null) elderObject.SetActive(false);

        // 현재 게이지 구간에 따라 하나만 활성화
        if (currentTime >= 4)
        {
            // 게이지 4: 아이
            if (childObject != null) childObject.SetActive(true);
        }
        else if (currentTime >= 2)
        {
            // 게이지 2, 3: 청년
            if (youthObject != null) youthObject.SetActive(true);
        }
        else
        {
            // 게이지 0, 1: 노인
            if (elderObject != null) elderObject.SetActive(true);
        }
    }
}