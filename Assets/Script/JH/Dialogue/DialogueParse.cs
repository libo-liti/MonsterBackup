using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParse : MonoBehaviour
{
    public static int Language = 6;
    [SerializeField]
    private TextAsset csvFile = null;

    [SerializeField]
    private TextAsset choiceFile = null;

    [SerializeField]
    List<DebugTalkData> DebugTalkDataList = new List<DebugTalkData>();

    public static Dictionary<int, TalkData[]> DialogueDictionary = new Dictionary<int, TalkData[]>();

    public static List<Dictionary<int, TalkData[]>> MultipleChoiceList
            = new List<Dictionary<int, TalkData[]>>();

    public static Dictionary<int, TalkData[]> MultipleChoiceDic
            = new Dictionary<int, TalkData[]>();
    private void Awake()
    {
        SetMultipleChoice();
        SetTalkDictionary();
        SetDebugTalkData();
    }

    public void SetMultipleChoice()
    {
        // 맨 아래 한 줄 빼기
        string csvText = choiceFile.text.Substring(0, choiceFile.text.Length - 1);
        // 줄바꿈(한 줄)을 기준으로 csv 파일을 쪼개서 string배열에 줄 순서대로 담음
        string[] rows = csvText.Split(new char[] { '\n' });

        for (int i = 1; i < rows.Length; i++)
        {
            // A, B, C열을 쪼개서 배열에 담음 (CSV파일은 ,로 데이터를 구분하기 때문에 ,를 기준으로 짜름)
            string[] rowValues = rows[i].Split(new char[] { ',' });

            // 유효한 이벤트 이름이 나올때까지 반복
            if (rowValues[0].Trim() == "end")
                continue;

            while (rowValues[0].Trim() != "end")
            {
                List<TalkData> talkDataList = new List<TalkData>();
                int choiceId = int.Parse(rowValues[2]);
                do
                {
                    // 캐릭터가 치는 대사를 담을 변수 대사의 길이를 모르므로 리스트로 선언
                    List<string> contextList = new List<string>();

                    TalkData talkData; // 구조체 선언
                    talkData.name = rowValues[3]; // 캐릭터 이름이 있는 B열
                    talkData.characterId = int.Parse(rowValues[4]);

                    do // talkData 하나를 만드는 반복문
                    {
                        contextList.Add(rowValues[Language].ToString());
                        if (++i < rows.Length) rowValues = rows[i].Split(new char[] { ',' });
                        else break;
                    } while (rowValues[3].Trim() == "" && rowValues[0] != "end");
                    talkData.mutipleChoiceId = -1;
                    talkData.contexts = contextList.ToArray();
                    talkDataList.Add(talkData);
                } while (rowValues[2].Trim() == "" && rowValues[0] != "end");
                MultipleChoiceDic.Add(choiceId, talkDataList.ToArray());
            }
            MultipleChoiceList.Add(MultipleChoiceDic);
        }
    }

    public static TalkData[] GetDialogue(int eventId)
    {
        return DialogueDictionary[eventId];
    }

    public void SetTalkDictionary()
    {
        // 맨 아래 한 줄 빼기
        string csvText = csvFile.text.Substring(0, csvFile.text.Length - 1);
        // 줄바꿈(한 줄)을 기준으로 csv 파일을 쪼개서 string배열에 줄 순서대로 담음
        string[] rows = csvText.Split(new char[] { '\n' });

        for (int i = 1; i < rows.Length; i++)
        {
            // A, B, C열을 쪼개서 배열에 담음 (CSV파일은 ,로 데이터를 구분하기 때문에 ,를 기준으로 짜름)
            string[] rowValues = rows[i].Split(new char[] { ',' });

            // 유효한 이벤트 이름이 나올때까지 반복
            if (rowValues[0].Trim() == "" || rowValues[0].Trim() == "end")
                continue;


            List<TalkData> talkDataList = new List<TalkData>();
            int eventId = int.Parse(rowValues[1]);
            while (rowValues[0].Trim() != "end") // talkDataList 하나를 만드는 반복문
            {
                // 캐릭터가 치는 대사를 담을 변수 대사의 길이를 모르므로 리스트로 선언
                List<string> contextList = new List<string>();

                TalkData talkData; // 구조체 선언
                talkData.name = rowValues[2]; // 캐릭터 이름이 있는 B열
                talkData.characterId = int.Parse(rowValues[3]);
                talkData.mutipleChoiceId = -1;

                do // talkData 하나를 만드는 반복문
                {
                    if (rowValues[4] != "")
                        talkData.mutipleChoiceId = int.Parse(rowValues[4]);
                    contextList.Add(rowValues[Language].ToString());
                    if (++i < rows.Length) rowValues = rows[i].Split(new char[] { ',' });
                    else break;
                } while (rowValues[2] == "" && rowValues[0] != "end");

                talkData.contexts = contextList.ToArray();

                talkDataList.Add(talkData);
            }
            DialogueDictionary.Add(eventId, talkDataList.ToArray());
        }
    }
    public void SetDebugTalkData()
    {
        // 딕셔너리의 키 값들을 가진 리스트
        List<int> eventIds =
                    new List<int>(DialogueDictionary.Keys);
        // 딕셔너리의 밸류 값들을 가진 리스트
        List<TalkData[]> talkDatasList =
                    new List<TalkData[]>(DialogueDictionary.Values);

        // 딕셔너리의 크기만큼 추가
        for (int i = 0; i < eventIds.Count; i++)
        {
            DebugTalkData debugTalk =
                new DebugTalkData(eventIds[i], talkDatasList[i]);

            DebugTalkDataList.Add(debugTalk);
        }
    }
}
