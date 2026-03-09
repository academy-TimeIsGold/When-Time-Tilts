using UnityEngine;

public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager Instance {  get; private set; }

    [Header("세이브 시스템")]
    // 현재 활성화된 세이브 포인트 데이터 (씬 리로드 후에도 유지)
    public SavePointData currentSavePointData { get; private set; }
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

    // 입력 잠금/해제
    public void SetInputLock(bool isLocked)
    {
        if (inputHandler != null)
        {
            inputHandler.AllDis(isLocked);
        }
    }

    // 세이브 포인트 도달 시 호출
    public void UpdateSavePoint(SavePoint newSavePoint, SavePointData data)
    {
        // 이전 세이브 포인트 비활성화
        if (currentSavePoint != null && currentSavePoint != newSavePoint)
        {
            currentSavePoint.DeactivateSavePoint();
        }

        //새로운 세이브 포인트로 변경
        currentSavePoint = newSavePoint;

        //넘겨받은 부활 좌표를 글로벌 변수에 저장
        currentSavePointData = data;

        // 파일 저장
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveFile(data);
        }
    }

    // 씬 로드 후 호출 - 저장된 데이터로 게임 상태 복원
    public void RestoreFromSavePoint()
    {
        if (currentSavePointData == null)
        {
            Debug.LogWarning("[GameManager] 복원할 SavePointData 없음");
            return;
        }

        // 플레이어 위치 복원
        PlayerVisualSwitcher player = FindAnyObjectByType<PlayerVisualSwitcher>();
        if (player != null)
        {
            player.transform.position = currentSavePointData.playerPosition;
        }

        // 시간 자원 복원
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.ResetResource(currentSavePointData.startResource);
        }

        // 해/달 상태 복원
        SkyController skyController = FindAnyObjectByType<SkyController>();
        if (skyController != null)
        {
            skyController.SetSkyState(currentSavePointData.startSkyState);
        }
    }

    // 씬 리로드 (DangerZone에서 호출)
    public void ResetToSavePoint()
    {
        if (GameManager.Instance != null)
        {
            GameSceneManager.Instance.ReloadCurrentScene();
        }
    }

    // 저장 파일로부터 이어하기 (타이틀에서 호출)
    public void LoadFromFile()
    {
        if (SaveLoadManager.Instance == null) return;

        SaveFileData saveData = SaveLoadManager.Instance.LoadFile();
        if (saveData == null) return;

        // 저장된 씬으로 이동
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadScene(saveData.sceneName);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("디버그: 리셋 테스트")]
    private void Debug_Reset()
    {
        ResetToSavePoint();
    }

    [ContextMenu("디버그: 현재 SavePointData 확인")]
    private void Debug_PrintSavePointData()
    {
        if (currentSavePointData == null)
        {
            Debug.Log("[GameManager] currentSavePointData: null (세이브 포인트 미도달)");
            return;
        }
        Debug.Log($"[GameManager] 현재 SavePointData: {currentSavePointData.name}\n" +
                  $"  씬: {currentSavePointData.sceneName}\n" +
                  $"  위치: {currentSavePointData.playerPosition}\n" +
                  $"  자원: {currentSavePointData.startResource}\n" +
                  $"  하늘: {currentSavePointData.startSkyState}");
    }

    [ContextMenu("디버그: 저장 파일 삭제")]
    private void Debug_DeleteSaveFile()
    {
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.DeleteSaveFile();
    }
#endif
}

