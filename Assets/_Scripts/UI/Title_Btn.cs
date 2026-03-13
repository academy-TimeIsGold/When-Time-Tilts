using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class Title_Btn : MonoBehaviour
{
    UIDocument document;
    VisualElement panel;

    [Header("Sound Panel 연결")]    
    public GameObject soundPanel;  //소리 설정 패널

    [Header("메뉴 버튼")]
    public Button startBtn;        //새로시작
    public Button continueBtn;     //이어하기
    public Button optionBtn;       //설정
    public Button exitBtn;         //종료    

    [Header("Fade 효과")]
    float fadeDuration = 1.0f;

    public void Start()
    {
        //컴포넌트 초기화
        document = GetComponent<UIDocument>();
        panel = document.rootVisualElement;

        //버튼 초기화
        startBtn = panel.Q<Button>("StartBtn");
        continueBtn = panel.Q<Button>("ContinueBtn");
        optionBtn = panel.Q<Button>("OptionBtn");
        exitBtn = panel.Q<Button>("ExitBtn");

        //버튼 연결
        startBtn.clickable.clicked += StartWTT;
        continueBtn.clickable.clicked += OnContinueClicked;
        optionBtn.clickable.clicked += Option;
        exitBtn.clickable.clicked += Exit;

        //마우스 오버 사운드 재생
        startBtn.RegisterCallback<MouseEnterEvent>(e => PlayHoverSound());
        continueBtn.RegisterCallback<MouseEnterEvent>(e => PlayHoverSound());
        optionBtn.RegisterCallback<MouseEnterEvent>(e => PlayHoverSound());
        exitBtn.RegisterCallback<MouseEnterEvent>(e => PlayHoverSound());

        //마우스 클릭 사운드 재생
        startBtn.clickable.clicked += PlayClickSound;
        continueBtn.clickable.clicked += PlayClickSound;
        optionBtn.clickable.clicked += PlayClickSound;
        exitBtn.clickable.clicked += PlayClickSound;

        UpdateContinueButton();

        //Title Scene FadeIn 효과
        StartCoroutine(FadeUI());
    }

    private IEnumerator FadeUI()
    {
        //시작 시 UI 투명도를 0으로 설정
        panel.style.opacity = 0;

        yield return null;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            panel.style.opacity = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }      
        panel.style.opacity = 1;
    }

    private void StartWTT()
    {
        //새 게임 시작 관련 SaveManager 기존 데이터 삭제        
        if (SaveLoadManager.Instance != null) SaveLoadManager.Instance.DeleteSaveFile();
        
        //체크 포인트 초기화
        if (GameManager.Instance != null) GameManager.Instance.ClearCheckpoint();

        //Intro Scene 이동
        GameSceneManager.Instance.LoadScene(SceneNames.INTRO);
        
    }

    private void OnContinueClicked()
    {
        //저장 데이터 확인
        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSaveFile())
        {
            //이어하기      
            if (GameManager.Instance !=null) GameManager.Instance.LoadFromFile();
        }
    }

    private void Option()
    {
        //옵션 패널 ON
        if (soundPanel != null) soundPanel.SetActive(true);
    }

    void Exit()
    {
        Application.Quit();
        #if UNITY_EDITOR

        //에디터에서 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    //이어하기 버튼 활성화
    private void UpdateContinueButton()
    {
        //세이브데이터가 있으면 활성화
        if (SaveLoadManager.Instance != null)
        {
            bool hasSaveData = SaveLoadManager.Instance.HasSaveFile();
            continueBtn.SetEnabled(hasSaveData);
        }

        else continueBtn.SetEnabled(false);        
    }

    private void PlayHoverSound()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("UI", 0);        
    }

    private void PlayClickSound()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("UI", 1);
    }
}
