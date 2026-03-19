using System.Collections;
using UnityEngine;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] string nextScene;

    public void NextScene()
    {
        StartCoroutine(ScreenFade());
    }

    private IEnumerator ScreenFade()
    {
        yield return StartCoroutine(ScreenManager.Instance.FadeOut());
        GameSceneManager.Instance.LoadScene(nextScene);
    }
}
