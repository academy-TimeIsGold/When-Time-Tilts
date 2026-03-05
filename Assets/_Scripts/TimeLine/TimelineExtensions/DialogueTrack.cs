using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// [타임라인 대사 전용 트랙]
/// 역할: 타임라인 전용 선로
/// 타임라인 창에서 빈 공간 우클릭 -> WhenTimeTilts -> Dialogue Track 으로 추가 가능
/// </summary>
[TrackColor(0.85f, 0.86f, 0.34f)] // 트랙을 눈에 띄는 노란색 계열로 설정
[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{
    // C# Action(이벤트)로 통신할 것이므로 별도의 바인딩(Binding)이 필요 없습니다.
    // 아주 가볍고 충돌 없는 구조입니다.
}