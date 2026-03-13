using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [Tooltip("체크 해제 시 클릭 소리가 나지 않습니다 (슬라이더 등에 유용)")]
    public bool playClickSound = true;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //마우스 오버 시 소리 재생
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("UI", 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //마우스 클릭 시 소리 재생
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("UI", 1);
    }
}
