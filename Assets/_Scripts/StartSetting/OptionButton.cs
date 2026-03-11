using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    public static OptionButton Instance;

    [Header("UI 캔버스 및 패널")]
    public GameObject menuCanvas;       //매뉴 캔버스
    public GameObject soundPanel;       //소리 설정 패널

    [Header("메뉴 버튼")]    
    public Button savePointButton;      //세이브 포인트
    public Button checkpointButton;     //체크 포인트
    public Button soundButton;          //소리 설정
    public Button titleButton;          //타이틀 화면
    public Button cancelButton;         //취소 버튼    
        
    private bool isMenuOpen = false;    //매뉴가 열려있는지 확인

    private InputHandler inputHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        //시작시 매뉴창 끄기
        if (menuCanvas != null) menuCanvas.SetActive(false);
        if (soundPanel != null) soundPanel.SetActive(false);

        //버튼 클릭 시 실행될 함수 연결
        if (savePointButton != null) savePointButton.onClick.AddListener(OnResetSavePoint);
        if (checkpointButton != null) checkpointButton.onClick.AddListener(OnResetCheckpoint);
        if (soundButton != null) soundButton.onClick.AddListener(OpenSoundSetting);
        if (titleButton != null) titleButton.onClick.AddListener(OnGoToTitle);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnBackInput);
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        menuCanvas.SetActive(isMenuOpen);

        //게임 정지
        if (isMenuOpen)
        {
            Time.timeScale = 0;
            InputHandler input = FindAnyObjectByType<InputHandler>();
            if (input != null) input.OpenUI(true);

            //체크포인트 여부에 따라 버튼 활성화/비활성화
            if (GameManager.Instance != null && checkpointButton != null)
            {
                checkpointButton.interactable = GameManager.Instance.HasCheckpoint;
            }
        }

        //게임 재개
        else
        {
            Time.timeScale = 1f;
            InputHandler input = FindAnyObjectByType<InputHandler>();

            //메뉴가 완전히 닫히면 플레이어 입력 복구
            if (input != null) input.OpenUI(false);

            //메뉴 닫을 때 열려있던 하위 창들도 다 정리
            if (soundPanel != null) soundPanel.SetActive(false);
        }
    }
    public void OnResetSavePoint()
    {
        ToggleMenu();
        if (GameManager.Instance != null) GameManager.Instance.ResetToSavePointForced();
    }

    public void OnResetCheckpoint()
    {
        ToggleMenu();
        if (GameManager.Instance != null) GameManager.Instance.ResetToCheckpointForced();
    }

    //소리 설정 열기
    public void OpenSoundSetting()
    {
        if (soundPanel != null) soundPanel.SetActive(true);  //켜기        
        Debug.Log("소리 설정 탭 활성화");
    }

    public void OnGoToTitle()
    {
        ToggleMenu();
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadScene(SceneNames.TITLE);
        }
    }

    public void OnBackInput()
    {
        //메뉴창이 열려 있지 않으면 빠져나옴
        if (!isMenuOpen) return;

        //소리 설정창이 열려 있으면 소리창만 닫고 종료
        if (soundPanel != null && soundPanel.activeSelf)
        {
            soundPanel.SetActive(false);
        }

        //열려있는 하위 창이 없으면 메뉴 자체를 종료
        else
        {
            ToggleMenu();
        }
    }
}
