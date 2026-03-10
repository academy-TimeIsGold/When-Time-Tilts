using UnityEngine;

[CreateAssetMenu(fileName = "SavePointData", menuName = "Game/SavePointData")]
public class SavePointData : ScriptableObject
{
    [Header("씬 정보")]
    [Tooltip("이 세이브 포인트가 속한 씬 이름")]
    public string sceneName;

    [Header("시간 자원")]
    [Tooltip("이 세이브 포인트에서 시작할 시간 자원량")]
    public int startResource = 0;

    [Header("해/달 상태")]
    [Tooltip("이 세이브 포인트에서 시작할 하늘 상태")]
    public SkyState startSkyState = SkyState.Day;
}
