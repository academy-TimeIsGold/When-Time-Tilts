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
    public event Action OnRevert;                 //시간 조절 모드 과거
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

    public Vector2 moveInput;
    public Vector2 mouseInput;
    public Vector2 uiMouseInput;

    private void Awake()
    {
        input = new WTTInput();
    }

    private void OnEnable()
    {
        input.Enable();

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

        input.Disable();
    }

    // --- 입력 모드 전환 ---
    public void OpenUI(bool isOpen)
    {
        if (isOpen)
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
}
