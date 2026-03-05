using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcDialogueDef
{
    [Header("NPC / 그룹 식별")]
    public string npcId;        // NPC 고유 ID
    public string groupId;      // 그룹 ID
    public bool isEntry;        // 시작 노드 여부

    [Header("대사 내용")]
    public string speaker;      // 화자 이름
    public string textKr;       // 대사 텍스트

    [Header("흐름 제어")]
    public string nextGroupId;  // 다음 그룹 ID (비어있으면 대화 종료)

    [Header("조건")]
    public string condition;    // ALL / CHILD / YOUTH / ELDER

    [Header("대사 유형")]
    public string dialogueType; // INTERACT / AMBIENT

    [Header("표시 시간 (AMBIENT 전용")]
    public float displayTime;   // 대사 표시 시간 (초) / 0이면 글자 수 기반 자동 계산


    #region 유틸 프로퍼티

    // 상호작용 말풍선 타입인가
    public bool IsInteract => string.Equals(dialogueType, DialogueType.INTERACT, StringComparison.OrdinalIgnoreCase);
    
    // 배경 말풍선 타입인가
    public bool IsAmbient => string.Equals(dialogueType, DialogueType.AMBIENT, StringComparison.OrdinalIgnoreCase);

    #endregion

    #region 팩토리 메서드

    public static NpcDialogueDef FromRow(Dictionary<string, string> row)
    {
        if (row == null) return null;

        // 로컬 파싱 유틸
        string Get(string key, string fallback = "")
        {
            if (!row.TryGetValue(key, out var v) || v == null) return fallback;
            return v;
        }

        bool GetBool(string key, bool fallback = false)
        {
            var s = (Get(key, fallback ? "TRUE" : "FALSE") ?? "").Trim().ToUpperInvariant();
            if (s == "TRUE" || s == "1" || s == "YES") return true;
            if (s == "FALSE" || s == "0" || s == "NO") return false;
            return fallback;
        }

        float GetFloat(string key, float fallback = 0f)
        {
            var s = Get(key, "").Trim();
            if (string.IsNullOrEmpty(s)) return fallback;
            return float.TryParse(s, out var v) ? v : fallback;
        }

        var npcId = Get("Npc_id", "").Trim();
        var groupId = Get("group_id", "").Trim();

        // 필수 컬럼 유효성 검사
        // npc_id, group_id가 없으면 null 반환
        if (string.IsNullOrEmpty(npcId) || string.IsNullOrEmpty(groupId)) return null;

        // 데이터 조립
        var def = new NpcDialogueDef
        {
            npcId = npcId,
            groupId = groupId,
            isEntry = GetBool("is_entry"),
            speaker = Get("speaker", "").Trim(),
            textKr = Get("text_kr", "").Trim(),
            condition = Get("condition", DialogueCondition.ALL).Trim().ToUpper(),
            dialogueType = Get("dialogue_type", DialogueType.INTERACT).Trim().ToUpper(),
            nextGroupId = Get("next_group_id", "").Trim(),
            displayTime = GetFloat("display_time", 0f)
        };

        // 품질 경고
        if (string.IsNullOrEmpty(def.textKr))
        {
            Debug.LogWarning($"[NpcDialogueDef] 대사 텍스트 비어있음 — npcId={def.npcId}, groupId={def.groupId}");
        }

        if (def.IsAmbient && def.displayTime < 0f)
        {
            Debug.LogWarning($"[NpcDialogueDef] AMBIENT인데 display_time이 음수 — groupId={def.groupId}");
        }

        return def;
    }

    #endregion

    #region 조건 판단

    public bool MatchesCondition(AgeState playerAge)
    {
        switch (condition)
        {
            case DialogueCondition.ALL:
                return true;
            case DialogueCondition.CHILD:
                return playerAge == AgeState.Child;
            case DialogueCondition.YOUTH:
                return playerAge == AgeState.Youth;
            case DialogueCondition.ELDER:
                return playerAge == AgeState.Elder;
            default:
                Debug.LogWarning($"[NpcDialogueDef] 알 수 없는 condition: '{condition}' — groupId={groupId}");
                return false;
        }
    }

    #endregion

    #region 상수 클래스

    public static class DialogueCondition
    {
        public const string ALL = "ALL";
        public const string CHILD = "CHILD";
        public const string YOUTH = "YOUTH";
        public const string ELDER = "ELDER";
    }

    public static class DialogueType
    {
        public const string INTERACT = "INTERACT";
        public const string AMBIENT = "AMBIENT";
    }

    #endregion
}
