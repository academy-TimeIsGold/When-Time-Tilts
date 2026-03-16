using UnityEngine;

public class PuzzleMechanism : ObjectSound
{
    public virtual void ActivateMechanism()
    {
        Debug.Log($"[{gameObject.name}] 기본 작동 로직 (아직 아무 구현이 안 되어 있습니다.)");
    }

    public virtual void DeactivateMechanism()
    {
        Debug.Log($"[{gameObject.name}] 기본 해제 로직 (아직 아무 구현이 안 되어 있습니다.)");
    }
}
