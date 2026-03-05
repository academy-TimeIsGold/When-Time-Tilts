using System.Collections.Generic;
using UnityEngine;

public class NpcDialogueDatabase : MonoBehaviour
{
    // group_id -> 해당 그룹의 대사 목록
    private readonly Dictionary<string, List<NpcDialogueDef>> _groupDB
        = new Dictionary<string, List<NpcDialogueDef>>();

    // npc_id -> is_entry인 group_id 목록
    private readonly Dictionary<string, List<string>> _entryGroupDB
        = new Dictionary<string, List<string>>();

    // 등록
    public void Register(NpcDialogueDef def)
    {
        // group_id 기준으로 묶기
        if (!_groupDB.ContainsKey(def.groupId))
        {
            _groupDB[def.groupId] = new List<NpcDialogueDef>();
        }
        _groupDB[def.groupId].Add(def);

        // is_entry인 경우 npc_id 기준으로 등록
        if (def.isEntry)
        {
            if (!_entryGroupDB.ContainsKey(def.npcId))
            {
                _entryGroupDB[def.npcId] = new List<string>();
            }

            if (!_entryGroupDB[def.npcId].Contains(def.groupId))
            {
                _entryGroupDB[def.npcId].Add(def.groupId);
            }
        }
    }

    public void Clear()
    {
        _groupDB.Clear();
        _entryGroupDB.Clear();
    }

    // 조회
    
    // NPC의 entry 그룹에서 조건에 맞는 대사 랜덤 1개 반환
    public bool TryGetEntryDialogue(string groupId, AgeState playerAge, out NpcDialogueDef result)
    {
        result = null;

        if (string.IsNullOrEmpty(groupId)) return false;
        
        if (!_groupDB.TryGetValue(groupId, out var group) || group.Count == 0)
        {
            Debug.LogWarning($"[NpcDialogueDB] 그룹 없음: {groupId}");
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
