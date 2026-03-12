using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ToggleObject : PuzzleMechanism
{
    [Header("상태별 오브젝트 설정")]
    public GameObject defaultState; //기본 상태
    public GameObject activeState;  //활성화 상태

    [Header("오브젝트 사운드 연결")]
    public AudioClip onSound;
    public AudioClip offSound;

    private AudioSource audioSource;

    private bool isToggled = false;

    private void Awake()
    {
        //오디오 소스 컴포넌트 초기화
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        //입체음향
        audioSource.spatialBlend = 1.0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetState(false);
    }

    //토글 스위치 ON
    public override void ActivateMechanism()
    {
        isToggled = true;
        SetState(true);
        PlaySound(onSound);
    }

    //토글 스위치 OFF
    public override void DeactivateMechanism()
    {
        isToggled = false;
        SetState(false);
        PlaySound(offSound);
    }

    //토글 스위치 눌렀을 때만 작동
    public void ToggleMechanism()
    {
        isToggled = !isToggled;
        SetState(isToggled);
        Debug.Log($"{gameObject.name} 스위치 작동. 현재 상태 {isToggled}");
    }

    private void SetState(bool isActive)
    {
        if (defaultState != null)
        {
            defaultState.SetActive(!isActive);            
        }

        if (activeState != null)
        {
            activeState.SetActive(isActive);            
        }
    }

    #region 오브젝트별 소리 재생 로직
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && SoundManager.Instance != null)
        {
            audioSource.volume = SoundManager.Instance.masterVolume * SoundManager.Instance.sfxVolume;
            audioSource.PlayOneShot(clip);
        }
    }
    #endregion
}
