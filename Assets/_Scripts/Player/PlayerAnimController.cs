using UnityEngine;

public class PlayerAnimController
{
    private Animator _anim;

    // --- 해시값 캐싱 (성능 최적화) ---
    // 파라미터 이름은 나중에 애니메이터 만들 때 똑같이 맞춰주면 돼
    private readonly int _hashIsMoving = Animator.StringToHash("isMoving");     // 걷기/대기 판별 (Bool)
    private readonly int _hashYVelocity = Animator.StringToHash("yVelocity");   // 점프/낙하 블렌딩 (Float)
    private readonly int _hashIsGrounded = Animator.StringToHash("isGrounded"); // 땅에 닿았는지 (Bool)
    private readonly int _hashInteract = Animator.StringToHash("Interact");     // 상호작용 트리거 (Trigger)
    private readonly int _hsahAction = Animator.StringToHash("Action");         // 시간모드 진입이나 초기화시 출력할 애니메이션(Trigger)

    // 생성자: 애니메이터를 받아옴
    public PlayerAnimController(Animator anim)
    {
        _anim = anim;
    }

    // --- 이동 관련 ---
    public void PlayMove(bool isMoving)
    {
        // 애니메이터가 없을 경우를 대비한 방어 코드
        if (_anim == null) return;
        _anim.SetBool(_hashIsMoving, isMoving);
    }

    // --- 공중 동작 (점프/낙하) ---
    public void UpdateAerialState(bool isGrounded, float yVelocity)
    {
        if (_anim == null) return;
        _anim.SetBool(_hashIsGrounded, isGrounded);
        _anim.SetFloat(_hashYVelocity, yVelocity);
    }

    // --- 상호작용 (레버 당기기 등) ---
    public void PlayInteract()
    {
        if (_anim == null) return;
        _anim.SetTrigger(_hashInteract);
    }

    //액션 애니메이션
    public void PlayAction()
    {
        if( _anim == null) return;
        _anim.SetTrigger(_hsahAction);
    }
}