using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class PressurePlate : MonoBehaviour
{
    [Header("상호작용 필요 나이 조건")]
    public AgeState requiredAge = AgeState.Elder;

    [Header("발판 시각적 오브젝트")]
    [Tooltip("트리거 영역은 가만히 두고, 이 오브젝트만 위아래로 움직입니다.")]
    public Transform visualTransform;

    [Header("발판 이벤트 연결")]
    public UnityEvent OnPlantePressed;
    public UnityEvent OnPlanteReleased;

    [Header("오브젝트 사운드 연결")]
    public AudioClip onSound;
    public AudioClip offSound;

    private AudioSource audioSource;
    private bool isPressed = false;

    private void Awake()
    {
        //오디오 소스 컴포넌트 초기화
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        //입체음향
        audioSource.spatialBlend = 1.0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Tag가 Player인지 확인
        if (other.CompareTag("Player"))
        {
            AgeState currentAge = GetPlayerAgeFromManager();

            //현재 나이가 필요 나이조건에 맞으면 실행
            if (currentAge == requiredAge && !isPressed)
            {
                Press();
                if (SoundManager.Instance != null)
                {
                    //사운드 재생
                    //PlaySound(onSound);
                    Debug.Log("효과재생");
                }
            }
            else if (currentAge != requiredAge) Debug.Log($"조건이 맞지 않아 발판 작동이 안됩니다. (현재 나이: {currentAge}");
        }               
    }

    private void OnTriggerExit2D(Collider2D other)
    {   
        //Player가 Trigger를 빠져 나가면 실행
        if (other.CompareTag("Player") && isPressed) Released();
        PlaySound(offSound);
    }

    #region 발판 작동 로직    
    private void Press()
    {
        isPressed = true;
        Debug.Log($"{gameObject.name} 발판 작동");
        PlaySound(onSound);

        //Object의 Vector3의 y값을 -0.1f 만큼 이동
        if (visualTransform != null)
        {
            visualTransform.localPosition = new Vector3(visualTransform.localPosition.x, visualTransform.localPosition.y - 0.1f, visualTransform.localPosition.z);
        }
        
        //발판이 눌렸다고 알림
        OnPlantePressed?.Invoke();       
    }


    private void Released()
    {
        isPressed = false;
        Debug.Log($"{gameObject.name} 발판 초기화");

        //Object의 Vector3의 y값을 +0.1f 만큼 이동
        if (visualTransform != null)
        {
            visualTransform.localPosition = new Vector3(visualTransform.localPosition.x, visualTransform.localPosition.y + 0.1f, visualTransform.localPosition.z);
        }

        //발판이 초기화 됐다고 알림
        OnPlanteReleased?.Invoke();
    }
    #endregion


    //TimeSystemManager를 통해 현재 나이 확인
    private AgeState GetPlayerAgeFromManager()
    {               
        return TimeSystemManager.Instance.GetCurrentAgeState();
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
