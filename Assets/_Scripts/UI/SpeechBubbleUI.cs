using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechBubbleUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject bubbleRoot;     // 말풍선 전체 오브젝트 (켜고/끄기용)
    [SerializeField] private TMP_Text speakerText;      // 화자 이름 (없으면 비워둬도 됨)
    [SerializeField] private TMP_Text dialogueText;     // 대사 텍스트

    [Header("페이드 설정")]
    [SerializeField] private float fadeOutDuration = 0.3f;  // 페이드 아웃 시간
    [SerializeField] private float fadeInDuration = 0.3f;   // 페이드 인 시간

    private Coroutine displayCoroutine;

    private void Awake()
    {
        if (bubbleRoot != null)
        {
            bubbleRoot.SetActive(false);
        }
    }

    // 말풍선 표시. duration초 후 자동으로 숨김.
    public void ShowDialogue(string speaker, string text, float duration)
    {
        // 화자 이름 설정 (항상 표시, 페이드 없음)
        if (speakerText != null) speakerText.text = speaker;

        // 진행 중인 코루틴 취소 후 새로 시작
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);

        displayCoroutine = StartCoroutine(DisplayRoutine(text, duration));
    }

    // 말풍선 즉시 숨김.
    public void Hide()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        if (bubbleRoot != null)
        {
            bubbleRoot.SetActive(false);
        }
    }

    private IEnumerator DisplayRoutine(string text, float duration)
    {
        // 말풍선이 이미 켜져 있으면 페이드 아웃 먼저
        if (bubbleRoot != null && bubbleRoot.activeSelf)
        {
            yield return StartCoroutine(FadeText(dialogueText, 1f, 0f, fadeOutDuration));
        }
        else
        {
            // 처음 표시라면 그냥 켜기
            if (bubbleRoot != null)
            {
                bubbleRoot.SetActive(true);
            }

            SetTextAlpha(dialogueText, 0f);
        }

        // 텍스트 교체
        if (dialogueText != null) dialogueText.text = text;

        // 페이드 인
        yield return StartCoroutine(FadeText(dialogueText, 0f, 1f, fadeInDuration));

        // 표시 유지
        yield return new WaitForSeconds(duration);

        // 페이드 아웃 후 숨김
        yield return StartCoroutine(FadeText(dialogueText, 1f, 0f, fadeOutDuration));

        if (bubbleRoot != null)
        {
            bubbleRoot.SetActive(false);
        }

        displayCoroutine = null;
    }

    // 페이드 효과
    private IEnumerator FadeText(TMP_Text target, float from, float to, float duration)
    {
        if (target == null) yield break;

        float elapsed = 0f;
        SetTextAlpha(target, from);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            SetTextAlpha(target, alpha);
            yield return null;
        }

        SetTextAlpha(target, to);
    }

    private void SetTextAlpha(TMP_Text target, float alpha)
    {
        if (target == null) return;
        Color c = target.color;
        c.a = alpha;
        target.color = c;
    }
}
