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
        //슬라이더 초기화
        masterSlider.value = SoundManager.Instance.masterVolume;
        bgmSlider.value = SoundManager.Instance.bgmVolume;
        sfxSlider.value = SoundManager.Instance.sfxVolume;

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

        else if (OptionButton.Instance.gameObject.activeSelf)
        {
            OptionButton.Instance.soundPanel.SetActive(false);
        }        
    }
}
