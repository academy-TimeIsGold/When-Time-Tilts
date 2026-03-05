using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    private NpcDialogueDatabase database;
    private NpcDialogueLoader loader;

    public bool IsLoaded => loader != null && loader.IsLoaded;
    public bool IsPlaying { get; private set; } = false;

    private string currentGroupId;
    private AgeState currentPlayerAge;
}
