using UnityEngine;
using Unity.Cinemachine; 
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("시네머신 연결")]
    public CinemachineCamera cam;

    // 컴포넌트들
    private CinemachineImpulseSource impulseSource;                 // 쉐이크 기능
    private CinemachineFollow follow;                               // 줌/팬/틸트 기능
    private CinemachineConfiner2D confiner;                         // 카메라 이탈 방지

    [Header("기본 설정")]
    public float defaultOrthoSize = 8f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (cam != null)
        {
            follow = cam.GetComponent<CinemachineFollow>();
            confiner = cam.GetComponent<CinemachineConfiner2D>(); 
        }
    }

    // ==========================================
    // 스테이지(방) 이동 시 카메라 가두리 변경
    // ==========================================
    public void SetBoundingBox(Collider2D newBounds)
    {
        if (confiner != null && newBounds != null)
        {
            // 1. 카메라가 못 나가는 테두리를 새로운 방의 테두리로 교체
            confiner.BoundingShape2D = newBounds;

            // 2. 시네머신에게 "테두리 바뀌었으니 다시 계산 하라고 알림
            confiner.InvalidateBoundingShapeCache();
        }
    }

    // ==========================================
    // (화면 흔들기, 줌, 팬)
    // ==========================================
    public void ShakeCamera(float force = 1f)
    {
        if (impulseSource != null) impulseSource.GenerateImpulseWithForce(force);
    }

    public void ZoomCamera(float targetSize, float duration)
    {
        StopCoroutine("ZoomRoutine");
        StartCoroutine(ZoomRoutine(targetSize, duration));
    }

    private IEnumerator ZoomRoutine(float targetSize, float duration)
    {
        float startSize = cam.Lens.OrthographicSize;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            yield return null;
        }
        cam.Lens.OrthographicSize = targetSize;
    }

    public void PanCamera(Vector2 targetOffset, float duration)
    {
        if (follow == null) return;
        StopCoroutine("PanRoutine");
        StartCoroutine(PanRoutine(targetOffset, duration));
    }

    private IEnumerator PanRoutine(Vector2 targetOffset, float duration)
    {
        // 1. 현재 카메라의 오프셋 위치를 가져옵니다.
        Vector3 startOffset = follow.FollowOffset;

        // 2. Z축은 묻지도 따지지도 않고 무조건 -10f로 강제 고정
        Vector3 target = new Vector3(targetOffset.x, targetOffset.y, -10f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 부드러운 이동 곡선 (SmoothStep)
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            // 3. Z값이 절대 0이 되지 않도록 Vector3.Lerp로 이동
            follow.FollowOffset = Vector3.Lerp(startOffset, target, t);
            yield return null;
        }

        // 도착 완료 후에도 Z축은 -10f로 고정
        follow.FollowOffset = target;
    }

    /// <summary>
    /// 카메라 리셋
    /// </summary>
    /// <param name="duration"></param>
    public void ResetCamera(float duration = 0.5f)
    {
        ZoomCamera(defaultOrthoSize, duration);
        PanCamera(Vector2.zero, duration);
    }

    // ==========================================
    // 스테이지 이동 시 카메라 가두리 교체 + 순간이동 컷
    // ==========================================
    public void SnapToNewStage(Collider2D newBounds)
    {
        // 1. 카메라가 못 나가는 테두리를 새로운 방의 테두리로 교체
        if (confiner != null && newBounds != null)
        {
            confiner.BoundingShape2D = newBounds;
            confiner.InvalidateBoundingShapeCache(); // 캐시 초기화 
        }

        // 2. 시네머신 순간이동 컷 (스무스하게 날아가는 버그 강제 차단)
        // 이전 프레임의 위치 궤적을 끊어서 플레이어의 새 위치로 즉시 텔레포트
        if (cam != null)
        {
            cam.PreviousStateIsValid = false;
        }
    }

    //CameraManager.Instance.cam.Target.TrackingTarget = 현재조종중인캐릭터Transform   <= 나중에 변경할 방식

#if UNITY_EDITOR
    [Header("--- 테스트용 세팅 (Snap 테스트용) ---")]
    [Tooltip("순간이동 시킬 플레이어 (TestYouth)")]
    public Transform testPlayerTransform;
    [Tooltip("플레이어가 텔레포트할 다음 방의 임의 좌표")]
    public Vector2 testTeleportPosition = new Vector2(20f, 0f);
    [Tooltip("다음 방의 테두리 (Stage2 Collider)")]
    public Collider2D testNextStageBounds;

    [ContextMenu("테스트 1: 화면 흔들기 (Shake)")]
    private void TestShake() => ShakeCamera(20f);

    [ContextMenu("테스트 2: 줌 인 (Zoom In)")]
    private void TestZoomIn() => ZoomCamera(4f, 1f); // 4 사이즈로 1초 동안 줌인

    [ContextMenu("테스트 3: 줌 아웃 (Zoom Out)")]
    private void TestZoomOut() => ZoomCamera(12f, 1f); // 12 사이즈로 1초 동안 줌아웃

    [ContextMenu("테스트 4: 위로 쳐다보기 (Pan Up)")]
    private void TestPanUp() => PanCamera(new Vector2(0, 4f), 1f); // 위로 4칸 시선 이동

    [ContextMenu("테스트 5: 원래대로 복구 (Reset)")]
    private void TestReset() => ResetCamera(1f);

    [ContextMenu("테스트 6: 다음 방으로 텔레포트 컷 (Snap)")]
    private void TestSnapToNewStage()
    {
        if (testPlayerTransform != null && testNextStageBounds != null)
        {
            // 1. 플레이어를 임의의 다음 방 좌표로 순간이동
            testPlayerTransform.position = testTeleportPosition;

            // 2. 가두리 교체 및 카메라 텔레포트 스냅
            SnapToNewStage(testNextStageBounds);

            Debug.Log("다음 방으로 깔끔하게 컷 완료!");
        }
        else
        {
            Debug.LogWarning("인스펙터에서 Snap 테스트용 변수(Player, Bounds)를 먼저 연결해 주세요");
        }
    }
#endif
}