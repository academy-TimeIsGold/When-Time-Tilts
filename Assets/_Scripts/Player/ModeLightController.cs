using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ModeLightController : MonoBehaviour
{
    [Header("Light 연결")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D playerLight;

    [Header("Global Light 설정")]
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private float modeIntensity = 0.35f;

    [Header("Player Light 설정")]
    [SerializeField] private float playerLightIntensity = 1f;
    [SerializeField] private float playerLightRadius = 5f;

    private readonly Color accelColor = new Color(0.2f, 0.4f, 1f, 1f);  // 가속 (파랑)
    private readonly Color revertColor = new Color(1f, 0.2f, 0.1f, 1f); // 회귀 (빨강)

    private void Start()
    {
        // Player Light 초기화
        if (playerLight != null)
        {
            // Freeform Light는 Scale로 크기 조절
            playerLight.transform.localScale = Vector3.one * playerLightRadius * 2f;
            playerLight.shapeLightFalloffSize = 0f;     // 경계 선명하게
            playerLight.intensity = playerLightIntensity;
            playerLight.gameObject.SetActive(false);
        }

        // GlobalLight 초기화
        if (globalLight != null)
        {
            globalLight.intensity = normalIntensity;
        }

        // 이벤트 구독
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.OnModeChanged += OnModeChanged;

            // 씬 리로드 후 현재 모드 상태로 동기화
            OnModeChanged(TimeSystemManager.Instance.CurrentMode);
        }
    }

    private void OnDestroy()
    {
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.OnModeChanged -= OnModeChanged;
        }
    }

    private void OnModeChanged(TimeMode mode)
    {
        // 모드 해제
        if (mode == TimeMode.None)
        {
            if (globalLight != null) globalLight.intensity = normalIntensity;
            if (playerLight != null) playerLight.gameObject.SetActive(false);
            return;
        }

        // 모드 진입
        if (globalLight != null) globalLight.intensity = modeIntensity;

        if (playerLight != null)
        {
            playerLight.gameObject.SetActive(true);
            playerLight.color = mode == TimeMode.Accelerate ? accelColor : revertColor;
        }
    }
}
