using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject eventHandlerObject;

    private EventHandler eventHandler;

    public void OnPointerExit(PointerEventData eventData)
    {
        eventHandler.ClearDescription();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventHandler.DisplayDescription(GetComponent<Button>());
    }

    public void SetSprite(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        
        eventHandler = eventHandlerObject.GetComponent<EventHandler>();
    }

    private void OnClick()
    {
        eventHandler.ButtonPressed(GetComponent<Button>());
    }
}
