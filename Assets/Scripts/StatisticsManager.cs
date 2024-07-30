using ChartAndGraph;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// TODO
// 1. 통계 메인 화면에서 게임 기록 버튼 클릭 시 게임 기록 패널로 이동 - 완
// 2. Record 패널에 기록 된 게임 불러오기 - 완
// 3. Progress 패널에 스테이지별 진행도 표시

// start에서 데이터들 불러오고 저장 해둔다

public class StatisticsManager : MonoBehaviour
{
    // 씬의 오브젝트들을 저장할 변수
    // 각 패널들을 저장 할 변수
    public GameObject panel_statistics;
    public GameObject panel_record;
    public GameObject panel_section;
    public GameObject panel_progress;

    // RecordPanel의 오브젝트들
    public GameObject Content;
    public GameObject Content_template;

    // SectionPanel의 오브젝트들
    public GameObject btn_section_back;
    public GameObject btn_section_next;
    public GameObject btn_section_prev;
    public GameObject top_graph_vm;
    public GameObject top_graph_pc;
    public GameObject top_graph_sp;
    public GameObject top_graph_sr;
    public GameObject top_graph_fg;
    public GraphChart graph_vm;
    public GraphChart graph_pc;
    public GraphChart graph_sp;
    public GraphChart graph_sr;
    public GraphChart graph_fg;
    public GameObject text_data;

    private int section_index = 0;

    // ProgressPanel의 오브젝트들
    public GameObject btn_next;
    public GameObject btn_prev;
    public GameObject graph_playtime;
    public GameObject graph_accuracy;
    public GameObject graph_progress;
    private int page_index = 0;

    public GameObject top_graph_playtime;
    public GameObject top_graph_accuracy;
    public GameObject top_graph_progress;

    // 로컬 JSON에서 불러온 데이터들을 저장할 배열
    private List<GameSession> gameRecords;
    private List<GameSession> vm;
    private List<GameSession> sp;
    private List<GameSession> fg;
    private List<GameSession> pc;
    private List<GameSession> sr;

    int showlvl;

    void Start()
    {
        // Load data
        vm = LocalDataManager.Instance.GetGameSessions("vm");
        sp = LocalDataManager.Instance.GetGameSessions("sp");
        fg = LocalDataManager.Instance.GetGameSessions("fg");
        pc = LocalDataManager.Instance.GetGameSessions("pc");
        sr = LocalDataManager.Instance.GetGameSessions("sr");

        // Aggregate all game sessions
        gameRecords = new List<GameSession>();
        gameRecords.AddRange(vm);
        gameRecords.AddRange(sp);
        gameRecords.AddRange(fg);
        gameRecords.AddRange(pc);
        gameRecords.AddRange(sr);

        // Render panels
        RenderRecordPanel();
        RenderSectionPanel();
        RenderProgressPanel();

        // Initial graph display
        ProgressRadarGraphRender(page_index);
    }

    void RenderRecordPanel()
    {
        // Ensure the template is active for instantiation
        Content_template.SetActive(true);

        foreach (Transform child in Content.transform)
        {
            if (child.name != "GameData_template")
            {
                Destroy(child.gameObject);
            }
        }

        void RenderGameRecords(List<GameSession> gameSessions, string gameName)
        {
            for (int i = 0; i < gameSessions.Count; i++)
            {
                GameObject newContent = Instantiate(Content_template, Content_template.transform.parent);
                newContent.name = "GameData_" + gameName + "_" + (i + 1);
                
                string date_day = gameSessions[i].date[..10];
                string date_second = gameSessions[i].date.Substring(11, 8);

                newContent.transform.Find("text_date_day").GetComponent<Text>().text = date_day;
                newContent.transform.Find("text_date_second").GetComponent<Text>().text = date_second;
                showlvl = gameSessions[i].lvl + 1;
                newContent.transform.Find("text_others").GetComponent<Text>().text = " " + gameName + " / Lv " + showlvl + " / " + gameSessions[i].prog + " / " + gameSessions[i].corr + " % / " + gameSessions[i].time + "s";

                newContent.SetActive(true);
            }
        }

        // Render records for each game
        RenderGameRecords(vm, "vm");
        RenderGameRecords(sp, "sp");
        RenderGameRecords(fg, "fg");
        RenderGameRecords(pc, "pc");
        RenderGameRecords(sr, "sr");

        // Deactivate template
        Content_template.SetActive(false);
    }

    void RenderSectionPanel()
    {
        // x축 날짜 포맷 설정
        graph_vm.CustomDateTimeFormat = (date) => { return date.ToString("yyyy\nMM/dd"); };
        graph_pc.CustomDateTimeFormat = (date) => { return date.ToString("yyyy\nMM/dd"); };
        graph_sp.CustomDateTimeFormat = (date) => { return date.ToString("yyyy\nMM/dd"); };
        graph_sr.CustomDateTimeFormat = (date) => { return date.ToString("yyyy\nMM/dd"); };
        graph_fg.CustomDateTimeFormat = (date) => { return date.ToString("yyyy\nMM/dd"); };

        // 그래프에 데이터를 추가하는 함수
        void AddDataToGraph(List<GameSession> gameSessions, string graphName, GraphChart graph)
        {
            if (gameSessions.Count != 0)
            {
                graph.DataSource.StartBatch();
                if (graphName != "vm")
                {
                    graph.DataSource.ClearCategory("Accuracy");
                }
                graph.DataSource.ClearCategory("Time");
                graph.DataSource.ClearCategory("AttentionScore");
                graph.DataSource.ClearCategory("ProgressScore");

                DateTime date;

                foreach (var session in gameSessions)
                {
                    if (TryParseCustomDateTime(session.date, out date))
                    {
                        graph.DataSource.AddPointToCategory("Time", date, float.Parse(session.time.ToString()));
                        if (graphName != "vm")
                        {
                            graph.DataSource.ClearCategory("Accuracy");
                        }
                        graph.DataSource.AddPointToCategory("AttentionScore", date, float.Parse(session.conc.ToString()));
                        graph.DataSource.AddPointToCategory("ProgressScore", date, float.Parse(session.prog.ToString()));
                    }
                }

                graph.DataSource.EndBatch();
            }
        }

        // 각 게임에 대한 데이터를 그래프에 추가
        AddDataToGraph(vm, "vm", graph_vm);
        AddDataToGraph(pc, "pc", graph_pc);
        AddDataToGraph(sp, "sp", graph_sp);
        AddDataToGraph(sr, "sr", graph_sr);
        AddDataToGraph(fg, "fg", graph_fg);

        // 초기 그래프 설정
        top_graph_vm.SetActive(true);
        top_graph_pc.SetActive(false);
        top_graph_sp.SetActive(false);
        top_graph_sr.SetActive(false);
        top_graph_fg.SetActive(false);

        // 각 버튼에 이벤트 추가
        btn_section_next.GetComponent<Button>().onClick.AddListener(() =>
        {
            section_index++;
            if (section_index > 4)
            {
                section_index = 0;
            }
            SectionGraphRender(section_index);
        });
        btn_section_prev.GetComponent<Button>().onClick.AddListener(() =>
        {
            section_index--;
            if (section_index < 0)
            {
                section_index = 4;
            }
            SectionGraphRender(section_index);
        });

        void SectionGraphRender(int index)
        {
            switch (index)
            {
                case 0:
                    top_graph_vm.SetActive(true);
                    top_graph_pc.SetActive(false);
                    top_graph_sp.SetActive(false);
                    top_graph_sr.SetActive(false);
                    top_graph_fg.SetActive(false);
                    break;
                case 1:
                    top_graph_vm.SetActive(false);
                    top_graph_pc.SetActive(true);
                    top_graph_sp.SetActive(false);
                    top_graph_sr.SetActive(false);
                    top_graph_fg.SetActive(false);
                    break;
                case 2:
                    top_graph_vm.SetActive(false);
                    top_graph_pc.SetActive(false);
                    top_graph_sp.SetActive(true);
                    top_graph_sr.SetActive(false);
                    top_graph_fg.SetActive(false);
                    break;
                case 3:
                    top_graph_vm.SetActive(false);
                    top_graph_pc.SetActive(false);
                    top_graph_sp.SetActive(false);
                    top_graph_sr.SetActive(true);
                    top_graph_fg.SetActive(false);
                    break;
                case 4:
                    top_graph_vm.SetActive(false);
                    top_graph_pc.SetActive(false);
                    top_graph_sp.SetActive(false);
                    top_graph_sr.SetActive(false);
                    top_graph_fg.SetActive(true);
                    break;
            }
        }
    }

    void RenderProgressPanel()
    {
        float[] playtime = new float[5];
        float[] accuracy = new float[5];
        float[] progress = new float[5];
        int[] gameCount = new int[5];

        void AggregateGameData(List<GameSession> gameSessions, int index)
        {
            foreach (var session in gameSessions)
            {
                // Debug.Log("Session time : " + session.time);
                // Debug.Log("Session corr : " + session.corr);
                // Debug.Log("Session prog : " + session.prog);
                playtime[index] += session.time;
                accuracy[index] += session.corr;
                progress[index] += session.prog;
                gameCount[index]++;
            }

            if (gameCount[index] != 0)
            {
                accuracy[index] /= gameCount[index];
            }
        }

        // Aggregate data for each game
        AggregateGameData(vm, 0);
        AggregateGameData(pc, 1);
        AggregateGameData(fg, 2);
        AggregateGameData(sr, 3);
        AggregateGameData(sp, 4);

        // Insert data into graphs
        if (graph_playtime != null && graph_accuracy != null && graph_progress != null)
        {
            RadarChart playtimeChart = graph_playtime.GetComponent<RadarChart>();
            RadarChart accuracyChart = graph_accuracy.GetComponent<RadarChart>();
            RadarChart progressChart = graph_progress.GetComponent<RadarChart>();

            playtimeChart.DataSource.SetValue("Player 1", "VM", playtime[0]);
            playtimeChart.DataSource.SetValue("Player 1", "PC", playtime[1]);
            playtimeChart.DataSource.SetValue("Player 1", "FG", playtime[2]);
            playtimeChart.DataSource.SetValue("Player 1", "SR", playtime[3]);
            playtimeChart.DataSource.SetValue("Player 1", "SP", playtime[4]);

            accuracyChart.DataSource.SetValue("Player 1", "VM", accuracy[0]);
            accuracyChart.DataSource.SetValue("Player 1", "PC", accuracy[1]);
            accuracyChart.DataSource.SetValue("Player 1", "FG", accuracy[2]);
            accuracyChart.DataSource.SetValue("Player 1", "SR", accuracy[3]);
            accuracyChart.DataSource.SetValue("Player 1", "SP", accuracy[4]);

            progressChart.DataSource.SetValue("Player 1", "VM", progress[0]);
            progressChart.DataSource.SetValue("Player 1", "PC", progress[1]);
            progressChart.DataSource.SetValue("Player 1", "FG", progress[2]);
            progressChart.DataSource.SetValue("Player 1", "SR", progress[3]);
            progressChart.DataSource.SetValue("Player 1", "SP", progress[4]);
        }
    }
    
    public void next()
    {
        page_index++;
        if (page_index > 2)
        {
            page_index = 0;
        }
        ProgressRadarGraphRender(page_index);
    }

    public void prev()
    {
        page_index--;
        if (page_index < 0)
        {
            page_index = 2;
        }
        ProgressRadarGraphRender(page_index);
    }

    void ProgressRadarGraphRender(int index)
    {
        switch (index)
        {
            case 0:
                top_graph_playtime.SetActive(true);
                top_graph_accuracy.SetActive(false);
                top_graph_progress.SetActive(false);
                break;
            case 1:
                top_graph_playtime.SetActive(false);
                top_graph_accuracy.SetActive(true);
                top_graph_progress.SetActive(false);
                break;
            case 2:
                top_graph_playtime.SetActive(false);
                top_graph_accuracy.SetActive(false);
                top_graph_progress.SetActive(true);
                break;
        }
    }
        static bool TryParseCustomDateTime(string dateString, out DateTime dateTime)
    {
        dateTime = DateTime.MinValue; // 기본값 설정

        // 하이픈으로 날짜 부분과 시간 부분을 분리
        string[] dateParts = dateString.Split('-');
        if (dateParts.Length == 4) // 연도, 월, 일, 시간 부분이 올바른지 확인
        {
            int year = int.Parse(dateParts[0]); // 연도
            int month = int.Parse(dateParts[1]); // 월
            int day = int.Parse(dateParts[2]); // 일

            // 시간 부분을 콜론으로 분리
            string[] timeParts = dateParts[3].Split(':');
            if (timeParts.Length == 3) // 시, 분, 초 부분 확인
            {
                int hour = int.Parse(timeParts[0]); // 시
                int minute = int.Parse(timeParts[1]); // 분
                int second = int.Parse(timeParts[2]); // 초

                // DateTime 객체 생성
                dateTime = new DateTime(year, month, day, hour, minute, second);
                return true; // 변환 성공
            }
        }
        return false; // 변환 실패
    }

    public void OnPointHovered( RadarChart.RadarEventArgs args )
    {
        if( text_data == null)
        {
            return;
        }
        // text_data의 데이터 변경
        text_data.GetComponent<Text>().text = args.Group + " : " + args.Value.ToString();
    }

    public void NonHovered()
    {
        text_data.GetComponent<Text>().text = "-";
    }
}