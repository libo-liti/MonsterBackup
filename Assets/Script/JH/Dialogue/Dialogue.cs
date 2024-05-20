using UnityEngine;
using System;
using System.Data.Common;

[Serializable]
public struct TalkData
{
    public string name; // 대사 치는 캐릭터 이름
    public int characterId; // 대사 치는 캐릭터 아이디
    public string[] contexts; // 대사 내용
    public int mutipleChoiceId; // 대화 선택지 아이디
}

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    TalkData[] talkDatas = null;

    [SerializeField]
    int[] eventId;
    [SerializeField]
    int NPCid;

    int eventIndex = 0;
    public static bool isActive = false;

    private void Start()
    {
        // 디버그용
        // 인스펙터 창에서 볼 수 있음 굳이 없어도 됌
        talkDatas = DialogueParse.GetDialogue(eventId[eventIndex]);

        // 캐릭터 
        DialogueManager.CharacterPos.Add(NPCid, transform.position);
    }
    private void Update()
    {
        Talk();
    }

    private void Talk()
    {
        if (Input.GetKeyDown(KeyCode.E) && eventIndex < eventId.Length && isActive == false)
        {
            GameObject.Find("DialogueManager").GetComponent<DialogueManager>().SetTalkBubble(eventId[eventIndex++]);
            isActive = true;
        }
    }
}