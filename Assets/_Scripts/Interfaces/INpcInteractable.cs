using UnityEngine;

public interface INpcInteractable
{
    // F키 빙력 시 호출
    void InteractNpc();

    // 현재 플레이어 나이 상태로 대화 가능한지 반환
    bool CanInteractNpc(AgeState ageState);
}
