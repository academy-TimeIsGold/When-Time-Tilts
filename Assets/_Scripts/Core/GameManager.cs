using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager Instance {  get; private set; }

    [Header("세이브 시스템")]
    // 현재 활성화된 세이브 포인트 데이터 (씬 리로드 후에도 유지)
    public SavePointData currentSavePointData { get; private set; }
    private SavePoint currentSavePoint;
    private Vector3 currentSpawnPosition;    // 씬 리로드 후에도 유지되는 스폰 위치

    private SavePoint currentCheckpoint;    // 중간 체크 포인트
    private Vector3 currentCheckpointPosition;  // 체크포인트 위치

    public SavePoint CurrentSavePoint => currentSavePoint;     // SceneInitializer에서 roomBounds 접근용
    public bool HasCheckpoint => currentCheckpoint != null;     // 설정 UI에서 체크포인트 버튼 비활성화/활성화 여부

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

        // 씬 로드 후 파괴될 수 있으므로 재탐색
        if (inputHandler == null)
        {
            inputHandler = FindAnyObjectByType<InputHandler>();
        }

        if (inputHandler != null) inputHandler.AllDis(false);
    }

    // 입력 잠금/해제
    public void SetInputLock(bool isLocked)
    {
        if (inputHandler == null)
            inputHandler = FindAnyObjectByType<InputHandler>();

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

        // 새 스테이지 세이브 포인트 등록 시 체크포인트 초기화
        ClearCheckpoint();

        // 위치를 Vector3로 미리 저장
        if (newSavePoint != null)
        {
             currentSpawnPosition = newSavePoint.transform.position;
        }

        // 파일 저장
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveFile(data, currentSpawnPosition);
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
            player.transform.position = currentSpawnPosition;

            foreach (Transform child in player.transform)
                child.localPosition = Vector3.zero;
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

        Debug.LogWarning("[GameManager] 복원 완료");
    }

    // 씬 리로드 (DangerZone에서 호출)
    public void ResetToSavePoint()
    {
        if (currentCheckpoint != null)
        {
            StartCoroutine(ResetToCheckpointRoutine());
        }
        else
        {
            StartCoroutine(ResetToSavePointRoutine());
        }
    }

    // 스테이지 세이브 포인트 리셋 — FadeOut 후 씬 리로드, FadeIn은 SceneInitializer 담당
    private IEnumerator ResetToSavePointRoutine()
    {
        SetInputLock(true);
        TimeSystemManager.Instance?.ClearMode();

        if (ScreenManager.Instance != null)
        {
            yield return ScreenManager.Instance.FadeOut();
        }

        GameSceneManager.Instance.ReloadCurrentScene();
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

    // 중간 체크포인트 등록 (파일 저장 없이 위치만 기억)
    public void RegisterCheckpoint(SavePoint checkpoint)
    {
        currentCheckpoint = checkpoint;
        currentCheckpointPosition = checkpoint.transform.position;
    }

    // 체크포인트 리셋
    private IEnumerator ResetToCheckpointRoutine()
    {
        SetInputLock(true);
        TimeSystemManager.Instance?.ClearMode();

        // FadeOut
        if (ScreenManager.Instance != null)
        {
            yield return ScreenManager.Instance.FadeOut();
        }

        // 플레이어 위치만 체크포인트로 이동
        PlayerVisualSwitcher player = FindAnyObjectByType<PlayerVisualSwitcher>();
        if (player != null)
        {
            player.transform.position = currentCheckpointPosition;

            foreach (Transform child in player.transform)
                child.localPosition = Vector3.zero;
        }

        // 카메라 스냅
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SnapToNewStage(
                currentSavePoint != null ? currentSavePoint.roomBounds : null
            );
        }

        // 잠깐 대기
        yield return new WaitForSeconds(1f);

        // FadeIn
        if (ScreenManager.Instance != null)
        {
            yield return ScreenManager.Instance.FadeIn();
        }

        if (player != null)
        {
            Rigidbody2D rb = player.GetComponentInChildren<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = true;
            }
        }

        SetInputLock(false);
    }

    // 씬 전환 시 체크포인트 초기화
    public void ClearCheckpoint()
    {
        currentCheckpoint = null;
        currentCheckpointPosition = Vector3.zero;
    }

    #region 설정 호출 용 명시적 리셋

    // 설정에서 호출 — 체크포인트 무시하고 항상 세이브포인트로 리셋
    public void ResetToSavePointForced()
    {
        ClearCheckpoint();
        StartCoroutine(ResetToSavePointRoutine());
    }

    // 설정에서 호출 — 체크포인트 있으면 체크포인트로, 없으면 세이브포인트로 리셋
    public void ResetToCheckpointForced()
    {
        if (currentCheckpoint != null)
            StartCoroutine(ResetToCheckpointRoutine());
        else
        {
            Debug.LogWarning("[GameManager] 체크 포인트 없음");
        }
    }

    #endregion



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

