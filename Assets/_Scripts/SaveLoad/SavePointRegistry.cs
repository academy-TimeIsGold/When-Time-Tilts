using System.Collections.Generic;
using UnityEngine;

public class SavePointRegistry : MonoBehaviour
{
    public static SavePointRegistry Instance { get; private set; }

    [SerializeField] private List<SavePoint> savePoints;

    private Dictionary<SavePointData, SavePoint> registry;

    private void Awake()
    {
        Instance = this;
        registry = new Dictionary<SavePointData, SavePoint>();

        foreach (var sp in savePoints)
        {
            if (sp != null && sp.savePointData != null)
            {
                registry[sp.savePointData] = sp;
            }
        }
    }

    // SavePointData So로 찾기 - 씬 리로드 복원 시 사용
    public SavePoint GetSavePoint(SavePointData data)
    {
        if (data == null) return null;

        registry.TryGetValue(data, out SavePoint sp);
        return sp;
    }

    // SO 이름으로 찾기 - 이어하기 시 사용
    public SavePoint GetSavePointByName(string dataName)
    {
        if (string.IsNullOrEmpty(dataName)) return null;

        foreach(var kv in registry)
        {
            if (kv.Key.name == dataName) return kv.Value;
        }
        return null;
    }
}
