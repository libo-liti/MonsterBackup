using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    GameObject TalkBubble;

    Transform playerTransform;

    TalkData[] talkDatas = null;
    TalkData[] choiceDatas = null;
    public static bool isTypingEnd = false;
    public static int choiceId = 0;

    public static Dictionary<int, Vector3> CharacterPos = new Dictionary<int, Vector3>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && Dialogue.isActive == false)
        {
            DialogueParse.DialogueDictionary.Clear();
            DialogueParse.MultipleChoiceList.Clear();
            DialogueParse.MultipleChoiceDic.Clear();

            if (DialogueParse.Language == 6)
                DialogueParse.Language = 7;
            else
                DialogueParse.Language = 6;

            gameObject.GetComponent<DialogueParse>().SetMultipleChoice();
            gameObject.GetComponent<DialogueParse>().SetTalkDictionary();

            Debug.Log(DialogueParse.Language);
        }
        playerTransform = GameObject.Find("Player").transform;
    }
    public void SetTalkBubble(int eventId)
    {
        StartCoroutine(setTalkBubble(eventId));
    }
    IEnumerator setTalkBubble(int eventId)
    {
        talkDatas = DialogueParse.GetDialogue(eventId);
        Vector3 talkBubblePos = Vector3.zero;
        for (int i = 0; i < talkDatas.Length; i++)
        {
            if (talkDatas[i].characterId == 1)
                talkBubblePos = Camera.main.WorldToScreenPoint(playerTransform.position + new Vector3(0, 1.5f, 0));
            else if (talkDatas[i].name != "")
                talkBubblePos = Camera.main.WorldToScreenPoint(CharacterPos[talkDatas[i].characterId] + new Vector3(0, 1.5f, 0));

            GameObject bubble = Instantiate(TalkBubble, talkBubblePos, Quaternion.identity);
            bubble.transform.SetParent(transform);

            TypeEffect typeEffect = bubble.transform.GetChild(0).GetComponent<TypeEffect>();
            for (int j = 0; j < talkDatas[i].contexts.Length; j++)
            {
                typeEffect.SetMsg(talkDatas[i].contexts[j]);
                yield return new WaitUntil(() => isTypingEnd);
                isTypingEnd = false;
                yield return new WaitForSeconds(1f);
            }
            Destroy(bubble);
            yield return new WaitForSeconds(1f);
            if (talkDatas[i].mutipleChoiceId != -1)
                yield return StartCoroutine(SetMultipleChoiceBubble(talkDatas[i].mutipleChoiceId));
        }
        Dialogue.isActive = false;
    }
    IEnumerator SetMultipleChoiceBubble(int id)
    {
        Dictionary<int, TalkData[]> choice = DialogueParse.MultipleChoiceList[id];
        GameObject panel;
        if (choice.Count == 2)
            panel = transform.GetChild(0).gameObject;
        else
            panel = transform.GetChild(1).gameObject;

        panel.SetActive(true);
        for (int i = 1; i <= choice.Count; i++)
        {
            panel.transform.GetChild(i - 1).GetChild(0).GetComponent<TMP_Text>().text = choice[i][0].contexts[0];
        }
        yield return new WaitUntil(() => choiceId != 0);
        panel.SetActive(false);
        choiceDatas = choice[choiceId];
        Vector3 talkBubblePos = Vector3.zero;
        for (int i = 0; i < choiceDatas.Length; i++)
        {
            if (choiceDatas[i].characterId == 1)
                talkBubblePos = Camera.main.WorldToScreenPoint(playerTransform.position + new Vector3(0, 1.5f, 0));
            else if (talkDatas[i].name != "")
                talkBubblePos = Camera.main.WorldToScreenPoint(CharacterPos[choiceDatas[i].characterId] + new Vector3(0, 1.5f, 0));

            GameObject bubble = Instantiate(TalkBubble, talkBubblePos, Quaternion.identity);
            bubble.transform.SetParent(transform);

            TypeEffect typeEffect = bubble.transform.GetChild(0).GetComponent<TypeEffect>();
            for (int j = 0; j < choiceDatas[i].contexts.Length; j++)
            {
                typeEffect.SetMsg(choiceDatas[i].contexts[j]);
                yield return new WaitUntil(() => isTypingEnd);
                isTypingEnd = false;
                yield return new WaitForSeconds(1f);
            }
            Destroy(bubble);
            yield return new WaitForSeconds(1f);
        }
    }
}