using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class Title_Btn : MonoBehaviour
{
    UIDocument document;
    VisualElement panel;

    Button startBtn;        //새로시작
    Button continueBtn;     //이어하기
    Button optionBtn;       //설정
    Button exitBtn;         //종료    

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
        //TODO: "기존 세이브가 있습니다. 덮어쓰시겠습니까?" 팝업 표시
        //TODO: 새 게임 시작 관련 SaveManager 기존 데이터 삭제 로직 추가        

        GameSceneManager.Instance.LoadScene("INTRO");
    }

    private void OnContinueClicked()
    {
        //TODO: Save 데이터 슬롯 창 띄우기
        //TODO: "저장된 데이터가 없습니다." 팝업 표시        

        //우선 Stage01로 이동
        GameSceneManager.Instance.LoadScene("Stage01");
    }

    private void Option()
    {
        //옵션 패널 ON
        //optionPanel.gameObject.SetActive(true);
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
        //bool hasSaveData = SaveManager.HasSaveData();
        //continueBtn.SetEnabled(hasSaveData);
    }
}
