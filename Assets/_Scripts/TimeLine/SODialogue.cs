using UnityEngine;

/// <summary>
/// 역할: 대사 데이터 그릇
/// 말풍선 대사 한 줄의 데이터 구조체 
/// 하나의 컷신 클립 안에서 이어질 대사들의 묶음 단위
/// </summary>

[System.Serializable]
public struct DialogueLine
{
    [Header("식별자")]
    [Tooltip("이 대사를 찾는 고유 키값 (예: S1_Intro_01)")]
    public string lineID; 

    [Header("화자 정보")]
    [Tooltip("누구 머리 위에 말풍선을 띄울지 결정하는 ID (예: Player, RabbitNPC 등)")]
    public string speakerID;

    [Header("대사 내용")]
    [TextArea(3, 5)]
    public string dialogueText;

    [Header("부가 연출")]
    [Tooltip("텍스트가 타이핑되는 속도 배율 (기본 1.0, 숫자가 커지면 빠름)")]
    public float typeSpeedMultiplier;
    [Tooltip("대사 출력 시 재생할 짧은 보이스나 효과음 (예: 웅나나 소리)")]
    public AudioClip voiceClip;
}


// 타임라인 연출 전용 대사 데이터 컨테이너
[CreateAssetMenu(fileName = "NewSODialogue", menuName = "WhenTimeTilts/SO Dialogue")]
public class SODialogue : ScriptableObject
{
    [Tooltip("컷신에서 순서대로 출력될 말풍선 대사 목록")]
    public DialogueLine[] dialogueLines;
}
