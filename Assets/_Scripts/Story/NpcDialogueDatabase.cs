using System.Collections.Generic;
using UnityEngine;

public class NpcDialogueDatabase : MonoBehaviour
{
    // group_id -> 해당 그룹의 대사 목록
    private readonly Dictionary<string, List<NpcDialogueDef>> groupDB
        = new Dictionary<string, List<NpcDialogueDef>>();

    // npc_id -> is_entry인 group_id 목록
    private readonly Dictionary<string, List<string>> entryGroupDB
        = new Dictionary<string, List<string>>();

    // 등록
    public void Register(NpcDialogueDef def)
    {
        // group_id 기준으로 묶기
        if (!groupDB.ContainsKey(def.groupId))
        {
            groupDB[def.groupId] = new List<NpcDialogueDef>();
        }
        groupDB[def.groupId].Add(def);

        // is_entry인 경우 npc_id 기준으로 등록
        if (def.isEntry)
        {
            if (!entryGroupDB.ContainsKey(def.npcId))
            {
                entryGroupDB[def.npcId] = new List<string>();
            }

            if (!entryGroupDB[def.npcId].Contains(def.groupId))
            {
                entryGroupDB[def.npcId].Add(def.groupId);
            }
        }
    }

    public void Clear()
    {
        groupDB.Clear();
        entryGroupDB.Clear();
    }

    // 조회
    
    // NPC의 entry 그룹에서 조건에 맞는 대사 랜덤 1개 반환 (랜덤 NPC 진입점)
    public bool TryGetEntryDialogue(string npcId, AgeState playerAge, out NpcDialogueDef result)
    {
        result = null;

        // npcId로 entry 그룹 목록 찾기
        if (!entryGroupDB.TryGetValue(npcId, out var entryGroups) || entryGroups.Count == 0)
        {
            Debug.LogWarning($"[NpcDialogueDB] entry 그룹 없음 — npcId={npcId}");
            return false;
        }

        // entry 그룹이 여러 개면 랜덤 선택 (랜덤 NPC)
        // entry 그룹이 1개면 그게 시작점 (순서 NPC)
        string groupId = entryGroups[Random.Range(0, entryGroups.Count)];

        // 선택된 그룹에서 조건에 맞는 대사 랜덤 1개 반환
        return TryGetRandom(groupId, playerAge, out result);

    }

    // 특정 groupId에서 조건에 맞는 대사 랜덤 1개 반환.
    // 순서 NPC에서 nextGroupId로 다음 대사를 이어갈 때 사용
    public bool TryGetRandom(string groupId, AgeState playerAge, out NpcDialogueDef result)
    {
        result = null;

        if (string.IsNullOrEmpty(groupId)) return false;

        if (!groupDB.TryGetValue(groupId, out var group) || group.Count == 0)
        {
            Debug.LogWarning($"[NpcDialogueDB] 그룹 없음 — groupId={groupId}");
            return false;
        }

        // MatchesCondition은 Def 안에 있으므로 DB는 판단 로직을 몰라도 됨
        var filtered = group.FindAll(d => d.MatchesCondition(playerAge));

        if (filtered.Count == 0)
        {
            Debug.LogWarning($"[NpcDialogueDB] 조건 맞는 대사 없음 — groupId={groupId}, age={playerAge}");
            return false;
        }

        result = filtered[Random.Range(0, filtered.Count)];
        return true;
    }
}
