using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] GameObject eventHandlerObject;

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
