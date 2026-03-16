using System;
using UnityEngine;
using UnityEngine.Playables;
using static DialogueLine;

/// <summary>
/// [대사 클립의 실제 행동 로직]
/// 역할: 대사 클립의 실제 행동
/// </summary>
public class DialogueBehaviour : PlayableBehaviour
{
    public SODialogue dialogueData;
    public string lineID;

    // UI 매니저에게 말풍선을 켜라고/끄라고 전달할 이벤트
    // true면 켜기, false면 끄기. 그리고 대사 데이터 한 줄을 통째로 보냅니다.
    public static Action<bool, DialogueLine> OnBubbleStateChanged;

    // 타임라인 재생 바가 이 클립 구간에 진입할 때 실행
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log($"[Timeline] 클립 재생 시도! 데이터 여부: {dialogueData != null}");

        if (Application.isPlaying && dialogueData != null && !string.IsNullOrEmpty(lineID))
        {
            // 1. SO의 배열에서 lineID가 일치하는 대사를 찾는다.
            DialogueLine? foundLine = null;
            foreach (var line in dialogueData.dialogueLines)
            {
                if (line.lineID == lineID)
                {
                    foundLine = line;
                    break;
                }
            }

            // 2. 대사를 찾았다면 UI쪽에 방송
            if (foundLine.HasValue)
            {
                OnBubbleStateChanged?.Invoke(true, foundLine.Value);
            }
            else
            {
                // 오타를 냈을 경우를 대비한 경고 
                Debug.LogWarning($"[Timeline] '{lineID}' ID를 가진 대사를 찾을 수 없습니다! SO 데이터를 확인해주세요.");
            }
        }
    }

    // 타임라인 재생 바가 이 클립 구간을 완전히 빠져나갈 때 실행
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (Application.isPlaying)
        {
            // UI쪽에 "말풍선 꺼줘!" 라고 방송 (빈 데이터 전송)
            OnBubbleStateChanged?.Invoke(false, new DialogueLine());
        }
    }

}