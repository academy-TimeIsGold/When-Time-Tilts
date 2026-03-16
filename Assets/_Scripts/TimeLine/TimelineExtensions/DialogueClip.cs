using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static DialogueLine;

/// <summary>
/// [타임라인 대사 클립]
/// 역할: 타임라인 대사 블록
/// DialogueTrack 위에 우클릭으로 생성하여 인스펙터에 SODialogue를 연결
/// </summary>
public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    [Tooltip("이 클립에서 재생할 대사 데이터(SO)")]
    public SODialogue dialogueData;

    [Tooltip("출력할 대사의 고유 ID를 적어주세요 (예: S1_Intro_01)")]
    public string lineID = ""; 

    // 클립이 섞이거나(블렌딩) 루프되지 않도록 설정
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        DialogueBehaviour clone = playable.GetBehaviour();

        //데이터 넘겨주기
        clone.dialogueData = dialogueData;
        clone.lineID = lineID;
        return playable;
    }
}