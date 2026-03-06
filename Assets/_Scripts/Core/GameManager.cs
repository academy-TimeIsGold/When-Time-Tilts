using UnityEngine;

public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager Instance {  get; private set; }

    [Header("세이브 시스템")]
    public Vector3 lastSavePosition { get; private set; }
    private SavePoint currentSavePoint;

    private InputHandler inputHandler;

    //InputHandler에서 활용할 시네마틱 연출용 키입력 가능여부 bool 변수
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

    public void UpdateSavePoint(SavePoint newSavePoint, Vector3 newPosition)
    {
        if (currentSavePoint != null && currentSavePoint != newSavePoint)
        {
            currentSavePoint.DeactivateSavePoint();
        }

        //새로운 세이브 포인트로 변경
        currentSavePoint = newSavePoint;

        //넘겨받은 부활 좌표를 글로벌 변수에 저장
        lastSavePosition = newPosition;
    }
}
