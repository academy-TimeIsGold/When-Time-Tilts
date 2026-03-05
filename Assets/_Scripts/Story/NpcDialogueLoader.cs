using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NpcDialogueLoader : MonoBehaviour
{
    [Header("구글 스프레드시트 CSV URL")]
    [SerializeField]
    private string sheetUrl =
        "https://docs.google.com/spreadsheets/d/e/" +
        "2PACX-1vRNoVmL65swzBMJ-c4VjV6Xx4yUc_-JtajcRJjoVIEzimdWIy2izlcwjtFUJEASch2Pn6klA7WE7JG_" +
        "/pubhtml";

    [Header("캐시 우회 - 수정 즉시 반영")]
    // URL에 현재 시각을 붙여 캐시를 무시하고 항상 최신 데이터 반영
    [SerializeField] private bool bypassCache = true;

    // 이벤트
    public Action onLoadComplete;
    public Action<string> onLoadFailed;
    public bool IsLoaded { get; private set; } = false;

    // DB 참조
    private NpcDialogueDatabase dialogueDb;

    // 초기화
    public void Initialize(NpcDialogueDatabase db)
    {
        dialogueDb = db;
        StartCoroutine(LoadRoutine());
    }

    // 로드 코루틴
    private IEnumerator LoadRoutine()
    {
        IsLoaded = false;

        // bypassCache On이면 URL에 timestamp 붙여서 캐시 무시
        // => 빌드가 된 후면 Off로 바꿔야함.
        string url = bypassCache
            ? sheetUrl + "&t=" + DateTime.Now.Ticks
            : sheetUrl;

        Debug.Log("[NpcDialogueLoader] 대사 데이터 로드 시작...");

        using var request = UnityWebRequest.Get(url);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // UTF-8 직접 디코딩 - 한글 깨짐 방지
            string csvText = Encoding.UTF8.GetString(request.downloadHandler.data);
            int count = ParseAndRegister(csvText);

            IsLoaded = true;
            Debug.Log($"[NpcDialogueLoader] 로드 완료 — {count}개 대사 등록");
            onLoadComplete?.Invoke();
        }
        else
        {
            Debug.LogError($"[NpcDialogueLoader] 로드 실패: {request.error}\nURL: {url}");
            onLoadFailed?.Invoke(request.error);
        }
    }

    // 파싱
    private int ParseAndRegister(string csvText)
    {
        dialogueDb.Clear();

        // 줄바꿈 토일 후 분리
        string[] lines = csvText
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Split('\n');

        if (lines.Length == 0) return 0;

        // 첫 줄 헤더 파싱 -> 컬럼명: 인덱스 딕셔너리
        // 컬럼 순서가 바뀌어도 헤더 이름 기준으로 매핑
        string[] headers = SplitLine(lines[0]);
        var headerMap = new Dictionary<string, int>();
        for (int i = 0; i < headers.Length; i++)
        {
            headerMap[headers[i].Trim()] = i;
        }

        int count = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue; // 빈 줄 무시

            string[] cols = SplitLine(line);

            // 헤더 이름 기준으로 Dictionary 구성 -> NpcDialogueDef.FromRow에 전달
            var row = new Dictionary<string, string>();
            foreach (var kv in headerMap)
            {
                row[kv.Key] = kv.Value < cols.Length ? cols[kv.Value] : "";
            }

            var def = NpcDialogueDef.FromRow(row);
            if (def == null) continue;

            dialogueDb.Register(def);
            count++;
        }

        return count;
    }

    // CSV 한 줄 파싱
    // 큰 따옴표로 감싼 셀 내부의 쉼표를 구분자로 인식하지 않도록 처리
    // ex) NPC_A, G001, 농부, "안녕, 반가워", ALL, TRUE, AMBIENT
    private string[] SplitLine(string Line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in Line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes; // 큰 따옴표 안팎 전환
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString()); // 쉼표 구분자, 현재까지의 셀 추가
                current.Clear();
            }
            else
            {
                current.Append(c); // 일반 문자, 현재 셀에 추가
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }
}
