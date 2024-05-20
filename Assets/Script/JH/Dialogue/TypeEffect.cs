using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypeEffect : MonoBehaviour
{
    public int charPerSeconds;
    public bool isActive;

    AudioSource audioSource;
    TMP_Text msgText;

    string targetMsg;
    float talkDelay;
    float typingInterval;
    int targetMsgIndex;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        msgText = GetComponent<TMP_Text>();
        isActive = false;
        talkDelay = 2f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isActive)
            SetMsg("");

    }
    public void SetMsg(string msg)
    {
        if (isActive)   // skip the converstation
        {
            msgText.text = targetMsg;
            CancelInvoke();
            EffectEnd();
        }
        else
        {
            targetMsg = msg;
            EffectStart();
        }
    }
    void EffectStart()
    {
        // msgText reset
        msgText.text = "";
        targetMsgIndex = 0;

        // typing speed
        typingInterval = 1.0f / charPerSeconds;

        isActive = true;
        Invoke("Effecting", typingInterval);
    }
    void Effecting()
    {
        // escape recursive function    
        if (targetMsgIndex == targetMsg.Length)
        {
            EffectEnd();
            return;
        }
        // sound
        if (targetMsg[targetMsgIndex] != ' ')
            audioSource.Play();

        // take one letter at a time
        if (targetMsg[targetMsgIndex] == '\\')      // enter
        {
            msgText.text += "\n";
            targetMsgIndex++;
        }
        else if (targetMsg[targetMsgIndex] == '`')  // talkDelay
        {
            targetMsgIndex++;
        }
        else
        {
            msgText.text += targetMsg[targetMsgIndex];
            targetMsgIndex++;
        }

        // recursive
        if (targetMsg[targetMsgIndex - 1] == '`')   // talkDelay
            Invoke("Effecting", typingInterval + talkDelay);
        else
            Invoke("Effecting", typingInterval);
    }
    void EffectEnd()
    {
        isActive = false;
        DialogueManager.isTypingEnd = true;
    }

    void TypingEnd()
    {
        DialogueManager.isTypingEnd = true;
    }
}
