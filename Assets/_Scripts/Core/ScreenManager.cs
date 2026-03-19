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
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }       

    public IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null) yield break;

        //UI를 클릭하지 Ray끄기
        fadeCanvasGroup.blocksRaycasts = true;

        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("SoundManager없음");
        }
        // FadeOut 시작 시 현재 BGM 볼륨 저장
        float startBGMVolume = SoundManager.Instance != null ? SoundManager.Instance.bgmSource.volume : 0f;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            //부드럽게 장면 전환
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            //BGM 볼륨도 같이 줄이기
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.bgmSource.volume = Mathf.Lerp(startBGMVolume, 0f, t);
            }
             
            yield return null;
        }

        //완전 불투명으로 고정
        fadeCanvasGroup.alpha = 1f;

        //BGM 완전 끄고 정지
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.bgmSource.volume = 0f;
            SoundManager.Instance.StopBGM();
        }
    }

    public IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        //FadeIn 시작 시 BGM 볼륨 복구
        float targetBGMVolume = SoundManager.Instance != null
            ? SoundManager.Instance.masterVolume * SoundManager.Instance.bgmVolume 
            : 0f;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            //BGM 볼륨도 같이 올리기
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.bgmSource.volume = Mathf.Lerp(0f, targetBGMVolume, t);
            }

            yield return null;
        }

        //완전 투명하게 고정
        fadeCanvasGroup.alpha = 0f; 

        //레이 활성화
        fadeCanvasGroup.blocksRaycasts = false; 

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.bgmSource.volume = targetBGMVolume;
        }
    }
}
