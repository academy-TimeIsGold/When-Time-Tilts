using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string savePath;
    private string soundSavePath;

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
        soundSavePath = Path.Combine(Application.persistentDataPath, "sound.json");
    }

    #region 게임 저장

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
            startSkyState = data.startSkyState,
            savePointDataName = data.name
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

    #endregion

    #region 사운드 저장

    // 사운드 설정 저장
    public void SaveSoundSetting(SoundSaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[SaveLoadManager] SoundSaveData가 null입니다.");
            return;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText (soundSavePath, json);
        Debug.Log("[SaveLoadManager] 사운드 설정 저장 완료");
    }

    // 사운드 설정 불러오기
    public SoundSaveData LoadSoundSettings()
    {
        if (!File.Exists(soundSavePath))
        {
            Debug.Log("[SaveLoadManager] 사운드 설정 파일 없음 — 기본값 사용");
            return null;
        }

        string json = File.ReadAllText(soundSavePath);
        SoundSaveData data = JsonUtility.FromJson<SoundSaveData>(json);
        Debug.Log("[SaveLoadManager] 사운드 설정 불러오기 완료");
        return data;
    }

    // 사운드 설정 파일 존재 여부 확인
    public bool HasSoundSaveFile()
    {
        return File.Exists(soundSavePath);
    }

    // 사운드 설정 파일 삭제
    public void DeleteSoundSaveFile()
    {
        if (File.Exists(soundSavePath))
        {
            File.Delete(soundSavePath);
            Debug.Log("[SaveLoadManager] 사운드 설정 파일 삭제 완료");
        }
    }

#if UNITY_EDITOR
    [ContextMenu("디버그: 사운드 설정 파일 삭제")]
    private void Debug_DeleteSoundSaveFile() => DeleteSoundSaveFile();
#endif
    #endregion
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
    public string savePointDataName;
}

// 사운드 설장 저장 데이터
[System.Serializable]
public class SoundSaveData
{
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;
}
