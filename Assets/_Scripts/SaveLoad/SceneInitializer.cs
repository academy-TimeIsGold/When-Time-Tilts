using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
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
