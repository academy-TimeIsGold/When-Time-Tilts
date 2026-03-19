using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    private bool isTriggered = false; //중복 실행 방지 안전장치 bool 변수

    [Header("엔딩 연출")]
    [Tooltip("엔딩에 쓸 문구 TextUI")]
    [SerializeField] private TextMeshProUGUI endingText;
    [Tooltip("페이드 인/아웃에 걸리는 시간 (초)")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("파괴되어야 하는 매니저들")]
    [SerializeField] private List<GameObject> managers = new List<GameObject>();

    private void Awake()
    {
        endingText.alpha = 0f;
        endingText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //전에 이미 작동했다면 빠져나옴
        if (isTriggered) return;

        //Tag가 Player인지 확인
        if (collision.CompareTag("Player"))
        {
            //트리거 작동
            isTriggered = true;

            StartCoroutine(EndingScreen());
        }
    }

    private IEnumerator EndingScreen()
    {
        GameManager.Instance.SetInputLock(true);
        TimeSystemManager.Instance?.ClearMode();

        if (ScreenManager.Instance != null)
        {
            yield return ScreenManager.Instance.FadeOut();
        }

        endingText.gameObject.SetActive(true);

        yield return StartCoroutine(TextFade(0f, 1f, endingText));

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(TextFade(1f, 0f, endingText));

        DestroyManagers();

        GameSceneManager.Instance.LoadScene(SceneNames.TITLE);
        
    }

    private IEnumerator TextFade(float start, float end, TextMeshProUGUI text)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            //부드럽게 장면 전환
            text.alpha = Mathf.Lerp(start, end, timer / fadeDuration);
            yield return null;
        }
        text.alpha = end;
    }

    private void DestroyManagers()
    {
        foreach (GameObject obj in managers)
        {
            Destroy(obj);
        }
    }
}
