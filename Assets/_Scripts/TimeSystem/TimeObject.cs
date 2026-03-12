using System.Collections;
using UnityEngine;

public class TimeObject : MonoBehaviour, IInteractable, IFocusable
{
    [Header("상태별 외형 설정")]
    public GameObject pastState;       //과거
    public GameObject futureState;     //미래

    [Header("초기 상태 설정")]
    public TimeState currentState = TimeState.Future;   //처음 배치될 때 기본 상태

    [Header("Cross-Fade 효과")]
    [Tooltip("상태가 변할 때 이미지가 교차되는 시간 (초)")]
    public float fadeDuration = 0.5f;

    [Header("포커스 시각 효과")]
    [SerializeField] private SpriteRenderer outlineRenderer;

    // 모드별 색상
    private static readonly Color accelColor = new Color(0.2f, 0.4f, 1f, 1f);   // 가속 (파랑)
    private static readonly Color revertColor = new Color(1f, 0.2f, 0.1f, 1f);  // 회귀 (빨강)

    public bool isInteractable = true;   //상호작용 가능 여부 (SkyReactor 등에서 사용)

    private Coroutine fadeCoroutine;
    
    //모드 상태에 따라 Alpha값 변경
    protected virtual void Start()
    {
        //UpdateVisual();

        if (pastState != null)
        {
            pastState.SetActive(currentState == TimeState.Past);
            SetAlpha(pastState, currentState == TimeState.Past ? 1.0f : 0.0f);
        }

        if (futureState != null)
        {
            futureState.SetActive(currentState == TimeState.Future);
            SetAlpha(futureState, currentState == TimeState.Future ? 1.0f : 0.0f);
        }

        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = false;
        }
    }

    [ContextMenu("테스트: 상호작용 실행")]
    public virtual void Interact()
    {
        //오브젝트의 현 상태에 따라 변경
        if (currentState == TimeState.Past) Accelerate();
        else Revert();            
    }

    public virtual void SetFocus(bool isFocused)
    {
        if (outlineRenderer == null) return;

        outlineRenderer.enabled = isFocused;

        if (!isFocused) return;

        // 활성화된 자식 오브젝트 위치로 이동
        GameObject activeState = currentState == TimeState.Past ? pastState : futureState;
        if (activeState != null)
        {
            outlineRenderer.transform.position = activeState.transform.position;
        }

        // 현재 모드에 따라 색상 변경
        if (TimeSystemManager.Instance != null)
        {
            outlineRenderer.color = TimeSystemManager.Instance.CurrentMode == TimeMode.Accelerate
                ? accelColor
                : revertColor;
        }
    }

    public virtual void Accelerate()
    {
        if (currentState == TimeState.Future) return;

        currentState = TimeState.Future;
        UpdateVisual();        
    }

    public virtual void Revert()
    {
        if (currentState == TimeState.Past) return;

        currentState = TimeState.Past;
        UpdateVisual();        
    }

    protected virtual void UpdateVisual()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(CrossFadeRoutine());
    }

    private IEnumerator CrossFadeRoutine()
    {
        //현재 상태를 기준으로 바뀔 to와 from을 자동 계산
        GameObject toObject = (currentState == TimeState.Past) ? pastState : futureState;
        GameObject fromObject = (currentState == TimeState.Past) ? futureState : pastState;

        //이미지 교체를 위한 오브젝트 활성화
        if (fromObject != null) fromObject.SetActive(true);
        if (toObject != null) toObject.SetActive(true);

        //변화되는 Object의 Collider 상태 변환
        ToggleColliders(fromObject, false);
        ToggleColliders(toObject, true);

        //Alpha값 변경 타이머
        float timer = 0f;

        //CrossFade
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t= timer / fadeDuration;

            if (fromObject != null) SetAlpha(fromObject, 1f - t);
            if (toObject != null) SetAlpha(toObject, t);

            yield return null;
        }

        //CrossFade 종료 시 투명도 고정 및 이전 오브젝트 비활성화
        if (fromObject != null)
        {
            SetAlpha(fromObject, 0f);
            fromObject.SetActive(false);

            //추후 모드 변경될 때를 대비해 미리 Collider 준비
            ToggleColliders(fromObject, true);
        }

        if (toObject != null) SetAlpha(toObject, 1.0f);
    }

    //TimeObject 자식들 알파값 조정
    private void SetAlpha(GameObject gameObject, float alpha)
    {
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in spriteRenderers)
        {
            Color c =sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    //자식들에 있는 Collider만 끄고 킴
    private void ToggleColliders(GameObject gameObject, bool isEnabled)
    {
        if (gameObject == null) return;
        Collider2D[] colliders = gameObject.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = isEnabled;
        }
    }
}
