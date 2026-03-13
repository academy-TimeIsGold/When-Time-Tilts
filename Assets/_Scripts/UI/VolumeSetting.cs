using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public static VolumeSetting Instance;

    [Header("패널")]
    public GameObject soundPanel;

    [Header("버튼")]
    public Button closeButton;

    [Header("볼륨 슬라이더")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("볼륨 값")]
    public TextMeshProUGUI masterText;
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;

    [Header("저장 딜레이 (초)")]
    [SerializeField] private float debounceTime = 0.5f;

    private Coroutine saveCoroutine;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;            
        }

        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSoundSettings();

        //텍스트 초기화
        UpdateTexts();

        //Fill 상태 초기화
        if (masterSlider.fillRect != null) masterSlider.fillRect.gameObject.SetActive(masterSlider.value > 0);
        if (bgmSlider.fillRect != null) bgmSlider.fillRect.gameObject.SetActive(bgmSlider.value > 0);
        if (sfxSlider.fillRect != null) sfxSlider.fillRect.gameObject.SetActive(sfxSlider.value > 0);

        //이벤트 연결
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        closeButton.onClick.AddListener(OnClosePanel);
    }

    private void LoadSoundSettings()
    {
        if (SaveLoadManager.Instance == null) return;

        if (!SaveLoadManager.Instance.HasSoundSaveFile())
        {
            // 저장 파일 없으면 SoundManager 기본값으로 슬라이더 초기화
            masterSlider.value = SoundManager.Instance.masterVolume;
            bgmSlider.value = SoundManager.Instance.bgmVolume;
            sfxSlider.value = SoundManager.Instance.sfxVolume;
            return;
        }

        // 저장 파일 존재 시
        SoundSaveData data = SaveLoadManager.Instance.LoadSoundSettings();
        if (data == null) return;

        SoundManager.Instance.SetMasterVolume(data.masterVolume);
        SoundManager.Instance.SetBGMVolume(data.bgmVolume);
        SoundManager.Instance.SetSFXVolume(data.sfxVolume);

        masterSlider.value = data.masterVolume;
        bgmSlider.value = data.bgmVolume;
        sfxSlider.value = data.sfxVolume;
    }

    //마스터 볼륨 조절
    void OnMasterChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);

        if (masterSlider.fillRect != null)
        {
            //값이 0보다 크면 Fill을 활성화하고, 그렇지 않으면 비활성화
            masterSlider.fillRect.gameObject.SetActive(value > 0);
        }
        UpdateTexts();
        ScheduleSave();
    }

    //BGM 볼륨 조절
    void OnBGMChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);

        if (bgmSlider.fillRect != null)
        {
            bgmSlider.fillRect.gameObject.SetActive(value > 0);
        }
        UpdateTexts();
        ScheduleSave();
    }
    
    //SFX 볼륨 조절
    void OnSFXChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);

        if (sfxSlider.fillRect != null)
        {
            sfxSlider.fillRect.gameObject.SetActive(value > 0);
        }
        UpdateTexts();
        ScheduleSave();
    }

    // 0.5초 딜레이 후 저장
    private void ScheduleSave()
    {
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
        }
        saveCoroutine = StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay()
    {
        yield return new WaitForSecondsRealtime(debounceTime);

        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveSoundSetting(new SoundSaveData
            {
                masterVolume = SoundManager.Instance.masterVolume,
                bgmVolume = SoundManager.Instance.bgmVolume,
                sfxVolume = SoundManager.Instance.sfxVolume
            });
        }
    }

    //텍스트 초기화
    void UpdateTexts()
    {
        masterText.text = $"Master: {(masterSlider.value * 100):F0}%";
        bgmText.text = $"BGM: {(bgmSlider.value * 100):F0}%";
        sfxText.text = $"SFX: {(sfxSlider.value * 100):F0}%";
    }

    public bool OnBackPressed()
    {
        //소리 설정창이 켜져 있는 상태라면 닫기
        if (OptionButton.Instance.soundPanel.activeSelf)
        {
            OptionButton.Instance.soundPanel.SetActive(false);

            //처리 후 메뉴 유지
            return true;
        }

        //팝업 창이 없으면 메뉴 닫기
        return false;
    }

    void OnClosePanel()
    {
        if (OptionButton.Instance == null)
        {
            VolumeSetting.Instance.soundPanel.SetActive(false);
            return;
        }

        if (OptionButton.Instance.gameObject.activeSelf)
        {
            OptionButton.Instance.soundPanel.SetActive(false);
        }        
    }

    // 사운드 설정 초기화 - UI 초기화 버튼에서 호출
    public void ResetToDefault()
    {
        // SoundManager 기본값으로 슬라이더 설정
        masterSlider.value = 1f;
        bgmSlider.value = 0.7f;
        sfxSlider.value = 0.7f;

        // Fill 상태 갱신
        if (masterSlider.fillRect != null) masterSlider.fillRect.gameObject.SetActive(masterSlider.value > 0);
        if (bgmSlider.fillRect != null) bgmSlider.fillRect.gameObject.SetActive(bgmSlider.value > 0);
        if (sfxSlider.fillRect != null) sfxSlider.fillRect.gameObject.SetActive(sfxSlider.value > 0);

        UpdateTexts();

        // 슬라이더 값 변경이 OnValueChanged를 트리거하므로 ScheduleSave 자동 호출됨
        // 파일도 기본값으로 덮어씌워짐
    }

#if UNITY_EDITOR
    [ContextMenu("디버그: 사운드 설정 초기화")]
    private void Debug_ResetToDefault() => ResetToDefault();
#endif
}
