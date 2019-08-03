using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private GameObject HandlerObject;
    [SerializeField] private List<Sprite> Upgrades;
    [SerializeField] private List<GameObject> UpgradesButton;

    private GameHandler handler;
    private List<Button> buttons;

    public void ButtonPressed(string buttonName)
    {
        handler.UpgradeSelected();
    }

    // Start is called before the first frame update
    void Start()
    {
        handler = HandlerObject.GetComponent<GameHandler>();

        buttons = new List<Button>();
        foreach (GameObject buttonObject in UpgradesButton)
        {
            buttons.Add(buttonObject.GetComponent<Button>());
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            Button button = buttons[i];

            button.GetComponent<Image>().sprite = Upgrades[i];
        }
    }
}
