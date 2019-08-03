using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private GameObject handlerObject;

    private GameHandler handler;

    public void ButtonPressed(string buttonName)
    {
        handler.UpgradeSelected();
    }

    // Start is called before the first frame update
    void Start()
    {
        handler = handlerObject.GetComponent<GameHandler>();
    }
}
