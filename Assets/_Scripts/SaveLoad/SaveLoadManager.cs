using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    // 저장
    public void SaveFile(SavePointData data, Vector3 playerPosition)
    {
        if (data == null)
        {
            Debug.LogWarning("[SaveLoadManager] SavePointData가 null입니다.");
            return;
        }

        SaveFileData saveData = new SaveFileData
        {
            sceneName = data.sceneName,
            playerPosition = playerPosition,
            startResource = data.startResource,
            startSkyState = data.startSkyState
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[SaveLoadManager] 저장 완료");
    }

    // 불러오기
    public SaveFileData LoadFile()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("[SaveLoadManager] 저장 파일 없음 — 새 게임 시작");
            return null;
        }

        string json = File.ReadAllText(savePath);
        SaveFileData saveData = JsonUtility.FromJson<SaveFileData>(json);
        Debug.Log($"[SaveLoadManager] 불러오기 완료");
        return saveData;
    }

    // 저장 파일 존재 여부 확인
    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }

    // 저장 파일 삭제 (새 게임 시작)
    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("[SaveLoadManager] 저장 파일 삭제 완료");
        }
    }
}

// JSON 직렬화용 저장 데이터 구조체
// SavePointData(SO)와 별개로 파일에 저장되는 런타임 데이터
[System.Serializable]
public class SaveFileData
{
    public string sceneName;
    public Vector3 playerPosition;
    public int startResource;
    public SkyState startSkyState;
}
