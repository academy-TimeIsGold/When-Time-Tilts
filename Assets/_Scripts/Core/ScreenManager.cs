using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [Header("페이드 연출 설정")]
    [Tooltip("투명도를 조절할 까만색 패널의 CanvasGroup")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [Tooltip("페이드 인/아웃에 걸리는 시간 (초)")]
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {        
        if (fadeCanvasGroup != null)
        {
            //게임 시작 시 화면이 보이게 투명도 0
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }       

    public IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null) yield break;

        //UI를 클릭하지 Ray끄기
        fadeCanvasGroup.blocksRaycasts = true;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            
            //부드럽게 장면 전환
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        //완전 불투명으로 고정
        fadeCanvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        //완전 투명하게 고정
        fadeCanvasGroup.alpha = 0f; 

        //레이 활성화
        fadeCanvasGroup.blocksRaycasts = false; 
    }
}
