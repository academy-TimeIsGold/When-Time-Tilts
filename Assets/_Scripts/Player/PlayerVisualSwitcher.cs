using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("캐릭터 오브젝트 연결")]
    [SerializeField] private GameObject childObject;  // -2, -1: 아이
    [SerializeField] private GameObject youthObject;  // 0, 1: 청년
    [SerializeField] private GameObject elderObject;  // 2: 노인

    // [추가] 현재 활성화된 오브젝트를 추적하기 위한 변수
    private GameObject _currentActiveObject;
    private bool _initialized = false;

    private void Start()
    {
        _initialized = true;
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.OnResourceChanged += OnResourceChanged;
        }

        // 현재 자원값으로 초기 외형 동기화
        UpdateVisual(TimeSystemManager.Instance.CurrentResource, false);
    }

    private void OnEnable()
    {
        if (!_initialized) return;
        TimeSystemManager.Instance.OnResourceChanged += OnResourceChanged;

    }

    private void OnDisable()
    {
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }


    private void OnResourceChanged(int resource)
    {
        UpdateVisual(resource, true);
    }

    // syncPosition: true면 위치를 이어받음, false면 초기화(게임 시작 시 등)
    private void UpdateVisual(int resource, bool syncPosition)
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

        if (resource <= -1)
        {
            nextObject = childObject; // 아이
        }
        else if (resource <= 1)
        {
            nextObject = youthObject; // 청년
        }
        else
        {
            nextObject = elderObject; // 노인
        }

        if (nextObject == null)
        {
            Debug.LogError("[PlayerVisualSwitcher]활성화할 오브젝트가 할당되지 않았습니다.");
            return;
        }

        // 4. 오브젝트 활성화 및 위치 동기화

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

    // [추가] 외부(SaveManager 등)에서 현재 플레이어 위치를 알기 위한 함수
    public Vector3 GetPlayerPosition()
    {
        if (_currentActiveObject != null) return _currentActiveObject.transform.position;
        return transform.position; // 활성화된 게 없으면 부모 위치 반환
    }
}