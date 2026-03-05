using UnityEngine;

public class ActorProfile : MonoBehaviour
{
    public string myID = "Rabbit"; 
    public Transform bubblePoint; // 말풍선이 뜰 머리 위 빈 오브젝트 위치

    private void Start()
    {
        // 씬이 시작되면 UI 매니저의 사전에 내 ID와 머리 위치를 등록
        DialogueUIManager.RegisterActor(myID, bubblePoint);
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴되거나 씬이 넘어갈 때 명부에서 내 이름 지우기 (메모리 관리)
        DialogueUIManager.UnregisterActor(myID);
    }
}