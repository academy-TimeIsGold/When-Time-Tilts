using UnityEngine;

public class TimelinePlaySound : MonoBehaviour
{
    public void PlayTimelineSFX(string scene, int index)
    {
        SoundManager.Instance.PlaySFX(scene, index);
    }
}
