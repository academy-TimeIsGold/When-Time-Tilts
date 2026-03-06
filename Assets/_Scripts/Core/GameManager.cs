using UnityEngine;

public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager Instance {  get; private set; }

    private InputHandler inputHandler;

    public bool isCinematicPlaying { get; private set; } = false;

    //씬 이동 시 파괴 방지
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        //inputHandler 초기화
        inputHandler = FindAnyObjectByType<InputHandler>();                   
    }

    //이벤트 연출 시작 시 호출
    public void StartStageClearSequence(PuzzleTrigger trigger, GameObject player)
    {
        Debug.Log("[GameManager] 이벤트 수신, 키 입력을 차단합니다.");

        isCinematicPlaying = true;

        //모든 키 입력 막기
        if (inputHandler != null)
        {
            inputHandler.AllDis(true);
        }

        //PuzzleTrigger 실행
        if (trigger != null)
        {
            trigger.StartCinematicWalk(player);
        }
    }

    //이벤트 연출 종료 시 호출
    public void EndStageClearSequence()
    {
        isCinematicPlaying = false;
        if (inputHandler != null) inputHandler.AllDis(false);
    }
}
