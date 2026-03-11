using UnityEngine;
using System.Collections.Generic;

// 플레이어의 상태(이동, 점프, 시간 능력 등)를 총괄하는 핵심 클래스입니다.
// 아이/청년/노인 오브젝트에 각각 부착되어, 나이대별로 다른 능력을 수행하게 합니다.
public class PlayerState : MonoBehaviour
{
    // [1] 인스펙터 설정 변수 (기획자/디자이너가 유니티 에디터에서 수정하는 값들)
    [Header("설계 수치 (인스펙터에서 수정)")]
    [SerializeField] private float moveSpeed;    // 이동 속도 (나이별로 다르게 설정)
    [SerializeField] private float jumpForce;    // 점프 힘 (노인은 0으로 설정하여 점프를 막음)
    [SerializeField] public bool canTriggerWeightPlate; // 무게 감지 발판 작동 여부 (노인만 True)
    [SerializeField] private float interactionRange = 5f; // 시간 조작(가속/회귀)이 가능한 사거리

    [Header("지면 체크 설정 (점프 판정용)")]
    [SerializeField] private Transform groundCheckPoint; // 발바닥 위치 (자식 오브젝트로 빈 GameObject를 만들어 배치해야 함)
    [SerializeField] private float groundCheckRadius = 0.2f; // 발바닥 감지 반경
    [SerializeField] private LayerMask groundLayer; // 무엇을 '땅'으로 인식할지 설정 (인스펙터에서 Layer 체크)

    [Header("시각적 효과")]
    [SerializeField] private SpriteRenderer rangeVisual; // 시간 조작 모드일 때 범위를 보여주는 원형 이미지

    // [2] 내부 로직용 변수 (코드에서만 사용)
    private PlayerMoveController _moveController; // 실제 물리 이동을 담당하는 클래스
    private InputHandler _inputHandler;           // 키보드/마우스 입력을 받아오는 핸들러
    private PlayerAnimController _animController; // 애니메이션 상태를 변경하는 컨트롤러

    private List<IFocusable> _focusedObjects = new List<IFocusable>(); // 현재 사거리 내에 들어와서 하이라이트된 오브젝트들 목록

    private float _currentInputX; // 플레이어의 좌우 입력값 (-1, 0, 1)을 저장해두는 변수
    private bool _isGrounded;     // 현재 땅에 닿아있는지 여부 (Update에서 매 프레임 갱신)

    // [3] 초기화 단계 (Awake)
    private void Awake()
    {
        _inputHandler = GetComponentInParent<InputHandler>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            _moveController = new PlayerMoveController(rb);
        }

        Animator myAnimator = GetComponent<Animator>();
        if (myAnimator != null)
        {
            _animController = new PlayerAnimController(myAnimator);
        }

        // 4. 시작할 때는 범위 시각화(파란/빨간 원)를 꺼둠
        if (rangeVisual != null) rangeVisual.gameObject.SetActive(false);
    }

    // [4] 메인 루프 (Update) - 매 프레임 실행되는 로직
    private void Update()
    {
        // 1. 환경 분석: 땅에 닿았는지 체크하고, 그에 맞춰 애니메이션을 갱신
        _isGrounded = CheckGround();
        UpdateAnimation();

        // 2. 시간 조작 모드 로직
        // 가속 또는 회귀 모드가 켜져 있을 때만 주변 오브젝트를 탐색(Scan)
        if (TimeSystemManager.Instance.CurrentMode != TimeMode.None)
        {
            ScanForTimeableObjects();
        }
        // 모드가 꺼졌는데 아직 하이라이트된 오브젝트가 리스트에 남아있다면 정리(Clear)
        else if (_focusedObjects.Count > 0)
        {
            ClearFocus();
        }

        if (rangeVisual != null && rangeVisual.gameObject.activeSelf)
            rangeVisual.transform.position = transform.position;
    }

    // [5] 물리 루프 (FixedUpdate) - 0.02초마다 실행되는 물리 연산 전용
    private void FixedUpdate()
    {
        // [중요] 이동 로직을 여기서 실행하는 이유:
        // Update에서 힘을 주면 프레임 드랍 시 캐릭터가 버벅이거나, 물리 엔진의 마찰력 때문에 속도가 줄어듭니다.
        // 여기서 매번 속도를 갱신해줘야 부드럽게 움직입니다.
        if (GameManager.Instance != null && GameManager.Instance.isCinematicPlaying)
        {
            return;
        }

        _moveController.Move(_currentInputX, moveSpeed);
    }

    // [6] 활성화/비활성화 처리 (OnEnable/OnDisable) - 캐릭터 교체 시 매우 중요
    private void OnEnable()
    {
        // 캐릭터가 켜질 때(변신 직후) 이전 캐릭터의 관성(속도)이 남아 미끄러지는 것을 방지
        if (_moveController != null) _moveController.Stop();

        // 입력값 초기화 (이전에 키를 누르고 있었다는 기억을 삭제)
        _currentInputX = 0f;

        // rangeVisual을 OnModeChanged 이벤트에 연결
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.OnModeChanged += UpdateRangeVisual;
        }

        if (_inputHandler == null) return;

        // 입력 이벤트 구독 (키를 누르면 이 함수들을 실행해라! 라고 연결)
        _inputHandler.OnMove += HandleMove;
        _inputHandler.OnJump += HandleJump;
        _inputHandler.OnAccel += OnAccelInput;
        _inputHandler.OnRevert += OnRevertInput;
        _inputHandler.OnMouseLeftClick += TryManipulateTime;
        _inputHandler.OnInteract += TryInteract;
    }

    private void OnDisable()
    {
        // 캐릭터가 꺼질 때도 안전하게 정지
        if (_moveController != null) _moveController.Stop();

        // 꺼질 때 입력값 초기화 (이 상태가 저장되어 다음 변신 때 제멋대로 움직이는 버그 방지)
        _currentInputX = 0f;

        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.OnModeChanged -= UpdateRangeVisual;
        }

        if (_inputHandler == null) return;

        // 입력 이벤트 구독 해제 (중복 실행 및 메모리 누수 방지)
        _inputHandler.OnMove -= HandleMove;
        _inputHandler.OnJump -= HandleJump;
        _inputHandler.OnAccel -= OnAccelInput;
        _inputHandler.OnRevert -= OnRevertInput;
        _inputHandler.OnMouseLeftClick -= TryManipulateTime;
        _inputHandler.OnInteract -= TryInteract;

        // 포커스 효과 정리
        ClearFocus();
    }

    // [7] 내부 로직 함수들 (지면 체크, 애니메이션)

    // 발바닥 위치(groundCheckPoint)에 원을 그려서 땅(Ground Layer)이 닿았는지 확인
    private bool CheckGround()
    {
        if (groundCheckPoint == null) return false;
        // Physics2D.OverlapCircle: 원형 범위 내에 특정 레이어의 콜라이더가 있는지 검사 (있으면 true)
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    // 현재 상태에 맞춰 애니메이터에게 파라미터를 전달
    private void UpdateAnimation()
    {
        if (_animController == null) return;

        // 입력값(_currentInputX)이 0.01보다 크면 움직이는 것으로 간주
        bool isMoving = Mathf.Abs(_currentInputX) > 0.01f;
        _animController.PlayMove(isMoving);

        // Y축 속도를 가져와서 점프 중인지 낙하 중인지 애니메이션 블렌딩
        float yVel = GetComponent<Rigidbody2D>().linearVelocity.y;
        _animController.UpdateAerialState(_isGrounded, yVel);
    }

    // [8] 입력 처리 핸들러 (InputHandler에서 신호를 보내면 실행됨)

    // 이동 키 입력 처리
    private void HandleMove(Vector2 direction)
    {
        // [중요] 여기서 바로 이동시키는 게 아니라, 입력값만 저장해두고 실제 이동은 FixedUpdate에서 함
        _currentInputX = direction.x;

        // 캐릭터 좌우 반전 처리 (스프라이트 뒤집기)
        if (direction.x != 0)
        {
            // 현재 크기(Scale)의 절댓값을 구해서, 방향에 따라 부호만 바꿔줌
            float xSize = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(direction.x > 0 ? xSize : -xSize, transform.localScale.y, 1);
        }
    }

    // 점프 키 입력 처리
    private void HandleJump()
    {
        // 1. 노인 캐릭터인지 확인 (jumpForce가 0이면 점프 불가)
        // 2. 땅에 닿아있는지 확인 (_isGrounded가 true여야 점프 가능)
        if (jumpForce > 0 && _isGrounded)
        {
            _moveController.Jump(jumpForce);
        }
    }

    // 시간 가속(1번) 키 입력
    private void OnAccelInput()
    {
        _animController.PlayAction();
        TimeSystemManager.Instance.SetMode(TimeMode.Accelerate);
        
    }

    // 시간 회귀(2번) 키 입력
    private void OnRevertInput()
    {
        _animController.PlayAction();
        TimeSystemManager.Instance.SetMode(TimeMode.Revert);
        
    }

    // 모드 변경 로직 (토글 방식)
    private void UpdateRangeVisual(TimeMode mode)
    {
        // 시각적 효과(범위 원) 처리
        if (rangeVisual != null)
        {
            // 모드가 None이 아닐 때만 켬
            rangeVisual.gameObject.SetActive(mode != TimeMode.None);
            // 가속은 빨강, 회귀는 파랑으로 색상 변경
            rangeVisual.color = (mode == TimeMode.Accelerate) ? new Color(1f, 0f, 0f, 0.5f): new Color(0f, 0f, 1f, 0.5f);
            // 사거리(interactionRange)에 맞춰 원의 크기 조절 (지름 = 반지름 * 2)
            rangeVisual.transform.localScale = new Vector3(interactionRange * 2, interactionRange * 2, 1);
        }

        // 모드가 꺼지면 포커스도 즉시 해제
        if (mode == TimeMode.None) ClearFocus();
    }

    // 마우스 왼쪽 클릭 시 상호작용 시도
    // 시간 모드일 때만 동작
    private void TryManipulateTime()
    {
        // 마우스 화면 좌표를 월드 좌표로 변환
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(_inputHandler.mouseInput);

        // 마우스 위치에 있는 오브젝트를 레이캐스트로 검출
        //RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        Collider2D col = Physics2D.OverlapPoint(mousePos);

        if (col == null) return;

        // 자식 콜라이더 클릭 시 부모 TimeObject 탐색
        TimeObject timeObj = col.GetComponentInParent<TimeObject>();
        if (timeObj == null) return;

        // SkyController 클릭
        // is: timeObj가 SkyController이면 SkyController로 변환하여 참조. (주로 상속 관계에서 사용)
        if (timeObj is SkyController skyController)
        {
            if (!skyController.IsAnimating) skyController.Interact();
            return;
        }

        // 대상이 존재하고, 플레이어와의 거리가 사거리 이내일 때만 실행
        if (Vector2.Distance(transform.position, col.transform.position) <= interactionRange)
        {
            if (!timeObj.isInteractable) return;    //조작 불가 오브젝트 차단
            bool success = TimeSystemManager.Instance.TryInteract(timeObj);
            if (success) TimeSystemManager.Instance.ClearMode();
        }
        
    }

    // 시간 모드 외의 일반 상호작용
    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        IInteractable closest = null;
        float closestDist = float.MaxValue;

        foreach ( Collider2D hit in hits)
        {
            // IFcousable도 있으면 F키 대상에서 제외
            if (hit.GetComponent<IFocusable>() != null) continue;
            
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = interactable;
                }
            }
        }

        closest?.Interact();
    }

    // [9] 주변 스캔 및 하이라이트 (시각적 피드백)
    private void ScanForTimeableObjects()
    {
        // 일단 기존 포커스를 다 지움 (매 프레임 갱신)
        ClearFocus();

        // 플레이어 주변 반경 interactionRange 내의 모든 콜라이더 검출
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (Collider2D hit in hits)
        {
            // 범위 내 오브젝트 중 ITimeable이 있는 것만 골라냄
            IFocusable focusable = hit.GetComponentInParent<IFocusable>();
            if (focusable == null) continue;

            TimeObject timeObj = hit.GetComponentInParent<TimeObject>();
            if (timeObj != null && !timeObj.isInteractable) continue;
            if (timeObj != null && !TimeSystemManager.Instance.IsCompatible
                (TimeSystemManager.Instance.CurrentMode, timeObj.currentState)) continue;

            focusable.SetFocus(true);
            _focusedObjects.Add(focusable);
        }

        SkyController skyController = FindAnyObjectByType<SkyController>();
        if (skyController != null && !skyController.IsAnimating)
        {
            TimeMode mode = TimeSystemManager.Instance.CurrentMode;
            bool canInteract =
                (mode == TimeMode.Accelerate && skyController.currentState == TimeState.Past) ||
                (mode == TimeMode.Revert && skyController.currentState == TimeState.Future);

            if (canInteract)
            {
                skyController.SetFocus(true);
                _focusedObjects.Add(skyController);
            }
        }
    }

    // 모든 포커스 해제
    private void ClearFocus()
    {
        foreach (IFocusable obj in _focusedObjects)
        {
            obj.SetFocus(false); // 포커스 끄기
        }
        _focusedObjects.Clear(); // 리스트 비우기
    }

    private void OnDrawGizmosSelected()
    {
        // 1. 시간 조작 범위 표시 (노란색 원)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // 2. 지면 체크 범위 표시 (빨간색 원) - 발바닥 위치 확인용
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}