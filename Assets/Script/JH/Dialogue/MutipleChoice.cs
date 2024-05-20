using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MutipleChoice : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    Outline outline;
    public int id;
    private void Awake()
    {
        outline = GetComponent<Outline>();
    }
    // UI 클릭할때
    public void OnPointerClick(PointerEventData eventData)
    {
        DialogueManager.choiceId = id;  // 선택한 선택지 아이디를 매니저한테 전달 
    }

    // 마우스가 UI 위에 올라갈때
    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    // 마우스가 UI 위에서 벗어날때
    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    }
}
