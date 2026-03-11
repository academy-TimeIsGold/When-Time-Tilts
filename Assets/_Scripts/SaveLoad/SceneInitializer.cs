using System.Collections;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    private IEnumerator Start()
    {
        // 한 프레임 대기 - 씬의 모든 오브젝트 초기화 완료 후 복원
        yield return null;

        // 씬 리로드 시 체크포인트 초기화
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClearCheckpoint();
        }

        // GameManager에 저장된 SavePointData로 복원
        if (GameManager.Instance != null && GameManager.Instance.currentSavePointData != null)
        {
            GameManager.Instance.RestoreFromSavePoint();

            // 카메라 즉시 스냅
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SnapToNewStage(
                    GameManager.Instance.CurrentSavePoint != null ? GameManager.Instance.CurrentSavePoint.roomBounds : null
                );
            }
        }
        // 저장된 파일이 있으면 파일에서 복원 (이어하기)
        else if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSaveFile())
        {
            SaveFileData saveData = SaveLoadManager.Instance.LoadFile();
            if (saveData != null)
            {
                RestoreFromFileData(saveData);
            }
        }

        GameManager.Instance?.SetInputLock(true);
        yield return new WaitForSeconds(1f);

        // FadIn
        if (ScreenManager.Instance != null)
        {
            yield return ScreenManager.Instance.FadeIn();
        }

        GameManager.Instance?.EndStageClearSequence();
    }

    // 파일 데이터로 직접 복원 (이어하기)
    private void RestoreFromFileData(SaveFileData data)
    {
        // 플레이어 위치
        PlayerVisualSwitcher player = FindAnyObjectByType<PlayerVisualSwitcher>();
        if (player != null)
        {
            player.transform.position = data.playerPosition;
        }

        // 시간 자원
        if (TimeSystemManager.Instance != null)
        {
            TimeSystemManager.Instance.ResetResource(data.startResource);
        }

        // 해/달 상태
        SkyController skyController = FindAnyObjectByType<SkyController>();
        if (skyController != null)
        {
            skyController.SetSkyState(data.startSkyState);
        }
    }
}
