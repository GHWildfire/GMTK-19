using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject eventHandlerObject;

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void SetUpgrade(Sprite sprite, string description)
    {

    }

    // Start is called before the first frame update
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        EventHandler eventHandler = eventHandlerObject.GetComponent<EventHandler>();
        eventHandler.ButtonPressed(transform.name);
    }
}
