using UnityEngine;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] string nextScene;

    public void NextScene()
    {
        GameSceneManager.Instance.LoadScene(nextScene);
    }
}
