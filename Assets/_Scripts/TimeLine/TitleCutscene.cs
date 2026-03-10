using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 역할:컷신 센서
/// </summary>
public class TItleCutscene : MonoBehaviour
{
    [Header("재생할 타임라인")]
    [SerializeField] PlayableDirector timelineToPlay;

    private void Start()
    {
        timelineToPlay.Play();
    }
}
