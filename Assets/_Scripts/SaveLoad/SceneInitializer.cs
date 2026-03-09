using System.Collections;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    private IEnumerator Start()
    {
        // 한 프레임 대기 - 씬의 모든 오브젝트 초기화 완료 후 복원
        yield return null;

        // FadIn
        if(ScreenManager.Instance != null)
        {
            yield return ScreenManager.Instance.FadeIn();
        }

        // GameManager에 저장된 SavePointData로 복원
        if (GameManager.Instance != null && GameManager.Instance.currentSavePointData != null)
        {
            GameManager.Instance.RestoreFromSavePoint();
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
