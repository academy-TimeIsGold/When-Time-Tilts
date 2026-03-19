using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private string bgmKey;
    [SerializeField] private int bgmIndex;

    private void Start()
    {
        SoundManager.Instance?.PlayBGM(bgmKey, bgmIndex);
    }
}
