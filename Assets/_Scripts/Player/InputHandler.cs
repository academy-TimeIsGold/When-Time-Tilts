using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    WTTInput input;

    //플레이어 캐릭터 이동등 이벤트 정의
    public event Action<Vector2> OnMove;        //기본 좌우 이동 및 아이상태 기어오르기
    public event Action OnJump;                 //점프
    public event Action OnInteract;             //근접 NPC상호작용 등
    public event Action OnRevert;               //시간 조절 모드 과거
    public event Action OnAccel;                //시간 조절 모드 미래
    public event Action OnEscape;               //일시정지? 및 설정창

    //시간 조절 모드시 마우스 이벤트
    public event Action<Vector2> OnMousePoint;
    public event Action OnMouseLeftClick;
    public event Action OnMouseMiddleClick;
    public event Action OnMouseRightClick;

    //UI 이벤트
    public event Action<Vector2> OnUIMousePoint;
    public event Action OnUIMouseLeftClick;
    public event Action OnUIMouseMiddleClick;
    public event Action OnUIMouseRightClick;
    public event Action OnUIEscape;

    public Vector2 moveInput;
    public Vector2 mouseInput;
    public Vector2 uiMouseInput;

    private void Awake()
    {
        input = new WTTInput();
    }

    private void OnEnable()
    {
        input.Player.Enable();
        input.UI.Disable();

        //플레이어
        input.Player.Move.performed += MoveCtx;
        input.Player.Move.canceled += MoveCtx;
        input.Player.Jump.performed += JumpCtx;
        input.Player.Interact.performed += InteractCtx;
        input.Player.Revert.performed += RevertCtx;
        input.Player.Accel.performed += AccelCtx;
        input.Player.MousePoint.performed += MousePointCtx;
        input.Player.MousePoint.canceled += MousePointCtx;
        input.Player.LeftClick.performed += LeftClickCtx;
        input.Player.MiddleClick.performed += MiddleClickCtx;
        input.Player.RightClick.performed += RightClickCtx;
        input.Player.ESC.performed += ESCCtx;

        //UI
        input.UI.MousePoint.performed += UIMousePointCtx;
        input.UI.MousePoint.canceled += UIMousePointCtx;
        input.UI.LeftClick.performed += UILeftClickCtx;
        input.UI.MiddleClick.performed += UIMiddleClickCtx;
        input.UI.RightClick.performed += UIRightClickCtx;
        input.UI.ESC.performed += UIESCCtx;

        //컷신
        CinematicManager.OnCinematicStateChanged += HandleCinematicState;
    }

    private void OnDisable()
    {
        //플레이어
        input.Player.Move.performed -= MoveCtx;
        input.Player.Move.canceled -= MoveCtx;
        input.Player.Jump.performed -= JumpCtx;
        input.Player.Interact.performed -= InteractCtx;
        input.Player.Revert.performed -= RevertCtx;
        input.Player.Accel.performed -= AccelCtx;
        input.Player.MousePoint.performed -= MousePointCtx;
        input.Player.MousePoint.canceled -= MousePointCtx;
        input.Player.LeftClick.performed -= LeftClickCtx;
        input.Player.MiddleClick.performed -= MiddleClickCtx;
        input.Player.RightClick.performed -= RightClickCtx;
        input.Player.ESC.performed -= ESCCtx;

        //UI
        input.UI.MousePoint.performed -= UIMousePointCtx;
        input.UI.MousePoint.canceled -= UIMousePointCtx;
        input.UI.LeftClick.performed -= UILeftClickCtx;
        input.UI.MiddleClick.performed -= UIMiddleClickCtx;
        input.UI.RightClick.performed -= UIRightClickCtx;
        input.UI.ESC.performed -= UIESCCtx;

        //컷신
        CinematicManager.OnCinematicStateChanged -= HandleCinematicState;

        input.Disable();
    }

    private void HandleCinematicState(bool isCinematicPlaying)
    {
        if (isCinematicPlaying)
        {
            // 1. 플레이어의 모든 조작(이동, 점프, 스킬 등)을 차단
            input.Player.Disable();
            input.UI.Enable();

            moveInput = Vector2.zero;
            OnMove?.Invoke(Vector2.zero);
        }
        else
        {
            // 컷신이 끝나면 다시 조작을 활성화합니다.
            input.Player.Enable();
            input.UI.Disable();
        }
    }

    // 입력모드 전환
    public void OpenUI(bool openUI)
    {
        if (openUI)
        {
            input.Player.Disable();
            input.UI.Enable();
        }
        else
        {
            input.UI.Disable();
            input.Player.Enable();
        }
    }

    // 모든 입력 막기
    public void AllDis(bool allDis)
    {
        if (allDis)
        {
            input.Player.Disable();
            input.UI.Disable();
        }
        else
        {
            input.UI.Disable();
            input.Player.Enable();
        }
    }


    //콜백 메서드
    void MoveCtx(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        OnMove?.Invoke(moveInput);
    }
    void JumpCtx(InputAction.CallbackContext ctx)
    {
        OnJump?.Invoke();
    }
    void InteractCtx(InputAction.CallbackContext ctx)
    {
        OnInteract?.Invoke();
    }
    void RevertCtx(InputAction.CallbackContext ctx)
    {
        OnRevert?.Invoke();
    }
    void AccelCtx(InputAction.CallbackContext ctx)
    {
        OnAccel?.Invoke();
    }
    void MousePointCtx(InputAction.CallbackContext ctx)
    {
        mouseInput = ctx.ReadValue<Vector2>();
        OnMousePoint?.Invoke(mouseInput);
    }
    void LeftClickCtx(InputAction.CallbackContext ctx)
    {
        OnMouseLeftClick?.Invoke();
    }
    void MiddleClickCtx(InputAction.CallbackContext ctx)
    {
        OnMouseMiddleClick?.Invoke();
    }
    void RightClickCtx(InputAction.CallbackContext ctx)
    {
        OnMouseRightClick?.Invoke();  
    }
    void ESCCtx(InputAction.CallbackContext ctx)
    {
        OnEscape?.Invoke();

        //옵션 싱글톤 찾아 메뉴 켜기
        if (OptionButton.Instance != null) OptionButton.Instance.ToggleMenu();
    }
    void UIMousePointCtx(InputAction.CallbackContext ctx)
    {
        uiMouseInput = ctx.ReadValue<Vector2>();
        OnUIMousePoint?.Invoke(uiMouseInput);
    }
    void UILeftClickCtx(InputAction.CallbackContext ctx)
    {
        OnUIMouseLeftClick?.Invoke();
    }
    void UIMiddleClickCtx(InputAction.CallbackContext ctx)
    {
        OnUIMouseMiddleClick?.Invoke();
    }
    void UIRightClickCtx(InputAction.CallbackContext ctx)
    {
        OnUIMouseRightClick?.Invoke();
    }
    void UIESCCtx(InputAction.CallbackContext context)
    {
        OnUIEscape?.Invoke();
        if (OptionButton.Instance != null) OptionButton.Instance.OnBackInput();
    }

}
