using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Image = UnityEngine.UI.Image;

public class BootIntro : MonoBehaviour
{

    [Header("인트로 오브젝트")]
    public Image intro_1;
    public Image intro_2;
    public Image intro_3;

    private WTTInput input;

    private void Awake()
    {
        input = new WTTInput();
    }

    //스킵 구독
    private void OnEnable()
    {
        input.Enable();
        input.Player.ESC.performed += SkipIntro;
    }

    //스킵 구독 해제
    private void OnDisable()
    {
        input.Player.ESC.performed -= SkipIntro;
        input.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(1.5f, intro_1, intro_2, intro_3));
    }

    //스킵 함수
    private void SkipIntro(InputAction.CallbackContext context)
    {
        GameSceneManager.Instance.LoadScene(SceneNames.TITLE);
    }  

    //로고 FadeIn, Out 루틴
    public IEnumerator FadeTextToFullAlpha(float t, Image i, Image j, Image k)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        j.color = new Color(j.color.r, j.color.g, j.color.b, 0);
        k.color = new Color(k.color.r, k.color.g, k.color.b, 0);

        //1번 이미지 출력
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }

        //2번 이미지 출력
        while (j.color.a < 1.0f)
        {
            j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a + (Time.deltaTime / t));
            yield return null;
        }
        j.color = new Color(j.color.r, j.color.g, j.color.b, 1);
        while (j.color.a > 0.0f)
        {
            j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a - (Time.deltaTime / t));
            yield return null;
        }

        //3번 이미ㅣ 출력
        while (k.color.a < 1.0f)
        {
            k.color = new Color(k.color.r, k.color.g, k.color.b, k.color.a + (Time.deltaTime / t));
            yield return null;
        }
        k.color = new Color(k.color.r, k.color.g, k.color.b, 1);
        while (k.color.a > 0.0f)
        {
            k.color = new Color(k.color.r, k.color.g, k.color.b, k.color.a - (Time.deltaTime / t));
            yield return null;
        }

        GameSceneManager.Instance.LoadScene(SceneNames.TITLE);
    }
}