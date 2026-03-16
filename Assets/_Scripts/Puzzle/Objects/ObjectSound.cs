using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectSound : MonoBehaviour
{
    [Header("사운드")]
    public AudioClip onAccelSound;      // 활성화 / 가속 사운드
    public AudioClip offRevertSound;    // 비할성화 / 회귀 사운드

    protected AudioSource audiotSource;

    protected virtual void Awake()
    {
        audiotSource = GetComponent<AudioSource>();
        audiotSource.playOnAwake = false;
        audiotSource.spatialBlend = 1.0f;               // 입체 음향
    }

    protected void PlaySound(AudioClip clip)
    {
        if (clip == null || SoundManager.Instance == null) return;

        audiotSource.volume = SoundManager.Instance.masterVolume * SoundManager.Instance.sfxVolume;
        audiotSource.PlayOneShot(clip);
    }
}
