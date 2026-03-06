using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("시간 자원 설정 (테스트용)")]
    [SerializeField] private int currentTime = 2;
    private const int _maxTime = 4;
    private const int _minTime = 0;

    [Header("캐릭터 오브젝트 연결")]
    [SerializeField] private GameObject childObject;  // 0~1: 아이
    [SerializeField] private GameObject youthObject;  // 2~3: 청년
    [SerializeField] private GameObject elderObject;  // 4: 노인

    // [추가] 현재 활성화된 오브젝트를 추적하기 위한 변수
    private GameObject _currentActiveObject;

    private void Start()
    {
        // 게임 시작 시 초기화
        UpdateVisual(false); // 첫 실행이라 위치 동기화 안 함
    }

    private void Update()
    {
        // 1번 키: 노화
        if (Keyboard.current.digit1Key.wasPressedThisFrame) AdjustTime(-1);
        // 2번 키: 회춘
        if (Keyboard.current.digit2Key.wasPressedThisFrame) AdjustTime(1);
    }

    private void AdjustTime(int amount)
    {
        currentTime = Mathf.Clamp(currentTime + amount, _minTime, _maxTime);
        Debug.Log($"현재 시간 게이지: {currentTime}");

        // 수치가 변했으므로 위치 동기화를 포함하여 비주얼 갱신
        UpdateVisual(true);
    }

    // syncPosition: true면 위치를 이어받음, false면 초기화(게임 시작 시 등)
    private void UpdateVisual(bool syncPosition)
    {
        // 1. 변신 전, 현재 위치 기억하기 (참고한 스크립트의 로직 응용)
        Vector3 lastPosition = transform.position; // 기본값은 부모 위치

        // 만약 이전에 활성화된 자식이 있었다면 그 녀석의 위치를 가져옴
        if (syncPosition && _currentActiveObject != null)
        {
            lastPosition = _currentActiveObject.transform.position;
        }

        // 2. 모든 오브젝트 비활성화
        if (childObject != null) childObject.SetActive(false);
        if (youthObject != null) youthObject.SetActive(false);
        if (elderObject != null) elderObject.SetActive(false);

        // 3. 다음 활성화할 오브젝트 결정
        GameObject nextObject = null;

        if (currentTime <= 1)
        {
            nextObject = childObject; // 아이
        }
        else if (currentTime <= 3)
        {
            nextObject = youthObject; // 청년
        }
        else
        {
            nextObject = elderObject; // 노인
        }

        // 4. 오브젝트 활성화 및 위치 동기화
        if (nextObject != null)
        {
            // 위치 이어받기 (참고한 ToMecha 로직)
            if (syncPosition)
            {
                nextObject.transform.position = lastPosition;
            }

            // 오브젝트 켜기
            nextObject.SetActive(true);

            // [중요] 변신 직후 물리 가속도가 남아있으면 튕겨나갈 수 있으므로 속도 초기화 (참고한 WarpTo 로직)
            Rigidbody2D rb = nextObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Unity 6면 linearVelocity, 구버전이면 velocity
                rb.linearVelocity = Vector2.zero;
            }

            // 현재 활성 오브젝트 갱신
            _currentActiveObject = nextObject;

            // 카메라 매니저에게 새로운 몸통을 쳐다보라고 알려주기
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SetCameraTarget(_currentActiveObject.transform);
            }
        }
    }

    // [추가] 외부(SaveManager 등)에서 현재 플레이어 위치를 알기 위한 함수
    public Vector3 GetPlayerPosition()
    {
        if (_currentActiveObject != null) return _currentActiveObject.transform.position;
        return transform.position; // 활성화된 게 없으면 부모 위치 반환
    }
}