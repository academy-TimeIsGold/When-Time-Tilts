using UnityEngine;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour
{
    [Header("설계 수치 (인스펙터에서 수정)")]
    [SerializeField] private float moveSpeed;    // 이동 속도 (나이별 변동)
    [SerializeField] private float jumpForce;    // 점프 힘 (노인은 0으로 설정하여 점프 차단)
    [SerializeField] public bool canTriggerWeightPlate; // 노인 상태일 때 감압판 작동 여부
    [SerializeField] private float interactionRange = 5f; // 시간 조작 가능 사거리

    [Header("시각적 효과")]
    [SerializeField] private SpriteRenderer rangeVisual; // 인게임 범위 표시용 Sprite

    private PlayerMoveController _moveController; // 물리 이동 제어기
    private InputHandler _inputHandler;           // 입력 이벤트 핸들러
    private TimeMode _currentMode = TimeMode.None; // 현재 시간 조작 모드 (가속/회귀/없음)

    // 현재 범위 내에 들어와 하이라이트된 오브젝트 리스트
    private List<ITimeable> _focusedObjects = new List<ITimeable>();

    private void Awake()
    {
        // 내 오브젝트의 Rigidbody2D를 사용하는 컨트롤러 생성
        _moveController = new PlayerMoveController(GetComponent<Rigidbody2D>());
        // 부모 오브젝트(PlayerManager)에서 입력 핸들러를 가져옴
        _inputHandler = GetComponentInParent<InputHandler>();

        // 시작할 때는 범위 시각화 끄기
        if (rangeVisual != null) rangeVisual.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 가속 또는 회귀 모드일 때만 주변 상호작용 대상을 실시간 스캔
        if (_currentMode != TimeMode.None)
        {
            ScanForTimeableObjects();
        }
        else if (_focusedObjects.Count > 0)
        {
            // 모드가 None이면 기존 포커스 효과를 모두 지움
            ClearFocus();
        }
    }

    private void OnEnable()
    {
        if (_inputHandler == null) return;

        // 명시적 메서드 연결 (람다식 미사용)
        _inputHandler.OnMove += HandleMove;
        _inputHandler.OnJump += HandleJump;
        _inputHandler.OnAccel += OnAccelInput;
        _inputHandler.OnRevert += OnRevertInput;
        _inputHandler.OnMouseLeftClick += TryManipulateTime;
    }

    private void OnDisable()
    {
        if (_inputHandler == null) return;

        // 오브젝트가 꺼질 때 구독 해제 (메모리 누수 및 중복 실행 방지)
        _inputHandler.OnMove -= HandleMove;
        _inputHandler.OnJump -= HandleJump;
        _inputHandler.OnAccel -= OnAccelInput;
        _inputHandler.OnRevert -= OnRevertInput;
        _inputHandler.OnMouseLeftClick -= TryManipulateTime;

        ClearFocus();
    }

    // --- 입력 처리 메서드 ---

    private void HandleMove(Vector2 direction)
    {
        // 입력받은 X 방향으로 이동 처리
        _moveController.Move(direction.x, moveSpeed);
    }

    private void HandleJump()
    {
        // jumpForce가 0인 상태(노인)라면 점프가 실행되지 않음
        if (jumpForce > 0)
        {
            _moveController.Jump(jumpForce);
        }
    }

    private void OnAccelInput()
    {
        // 가속 모드 토글 (한 번 더 누르면 해제)
        SetTimeMode(TimeMode.Accelerate);
    }

    private void OnRevertInput()
    {
        // 회귀 모드 토글 (한 번 더 누르면 해제)
        SetTimeMode(TimeMode.Revert);
    }

    private void SetTimeMode(TimeMode mode)
    {
        _currentMode = (_currentMode == mode) ? TimeMode.None : mode;

        // 인게임 비주얼 제어: 모드가 None이 아니면 범위 표시 활성화
        if (rangeVisual != null)
        {
            rangeVisual.gameObject.SetActive(_currentMode != TimeMode.None);
            // 가속/회귀 모드에 따라 색깔을 다르게 주면 더 좋음
            rangeVisual.color = (_currentMode == TimeMode.Accelerate) ? Color.red : Color.blue;
            // 스프라이트 크기를 사거리 수치에 맞게 조절 (원형 스프라이트 기본 크기 기준)
            rangeVisual.transform.localScale = new Vector3(interactionRange * 2, interactionRange * 2, 1);
        }

        if (_currentMode == TimeMode.None) ClearFocus();
    }

    private void TryManipulateTime()
    {
        // 모드가 None이 아닐 때만 마우스 클릭 상호작용 작동
        if (_currentMode == TimeMode.None) return;

        // 마우스 위치의 오브젝트 검출
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(_inputHandler.mouseInput);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            ITimeable target = hit.collider.GetComponent<ITimeable>();
            // ITimeable 인터페이스가 있고 사거리 이내일 때만 시간 조작 실행
            if (target != null && Vector2.Distance(transform.position, hit.transform.position) <= interactionRange)
            {
                if (_currentMode == TimeMode.Accelerate) target.Accelerate();
                else if (_currentMode == TimeMode.Revert) target.Revert();
            }
        }
    }

    // 주변의 시간 조작 가능 오브젝트를 찾아 포커스(하이라이트) 효과 부여
    private void ScanForTimeableObjects()
    {
        ClearFocus();
        // interactionRange 반경 내의 모든 콜라이더 검출
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (Collider2D hit in hits)
        {
            ITimeable target = hit.GetComponent<ITimeable>();
            if (target != null)
            {
                target.SetFocus(true); // 오브젝트에 포커스 켜기 알림
                _focusedObjects.Add(target);
            }
        }
    }

    private void ClearFocus()
    {
        foreach (ITimeable obj in _focusedObjects)
        {
            obj.SetFocus(false); // 모든 오브젝트의 포커스 효과 끄기
        }
        _focusedObjects.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        // 노란색 원으로 상호작용 범위를 그려줌
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}