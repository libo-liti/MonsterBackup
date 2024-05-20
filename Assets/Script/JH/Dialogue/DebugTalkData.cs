[System.Serializable]
public class DebugTalkData
{
    public int eventId;
    public TalkData[] talkDatas;

    public DebugTalkData(int id, TalkData[] td)
    {
        eventId = id;
        talkDatas = td;
    }
}