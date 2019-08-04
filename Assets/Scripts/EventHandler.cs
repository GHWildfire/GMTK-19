using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class EventHandler : MonoBehaviour
{
    public enum UpgradeType
    {
        BULLET_SPEED,
        BULLET_TIME,
        BULLET_DAMAGES,
        BULLET_SCALE,
        PLAYER_SPEED,
        PLAYER_LIFE,
        ENNEMY_SPEED
    }

    [SerializeField] private GameObject HandlerObject;

    [Header("Upgrades specification")]
    [SerializeField] private List<Button> UpgradesButton;
    [SerializeField] private Button HealButton;
    [SerializeField] private List<UpgradeClass> Upgrades;

    [System.Serializable]
    public class UpgradeClass
    {
        public Sprite Sprite = null;
        public string Description = "No description";
        public UpgradeType UpgradeType = UpgradeType.BULLET_SPEED;
        public int CurrentLevel = 0;
        public int LevelMax = 1;
    }

    private GameHandler handler;
    private List<Button> buttons;

    private delegate void updateParameters();
    private List<updateParameters> parameterFunctions;

    private List<string> upgradeDescriptions;

    private List<UpgradeClass> upgradesCopy;

    public void ButtonPressed(Button buttonPressed)
    {
        int index = UpgradesButton.IndexOf(buttonPressed);
        if (index >= 0)
        {
            string description = upgradeDescriptions[index];

            foreach (UpgradeClass upgrade in Upgrades)
            {
                if (upgrade.Description.Equals(description))
                {
                    upgrade.CurrentLevel++;
                    ApplyUpgrade(upgrade.UpgradeType);
                }
            }
        }

        if (buttonPressed == HealButton)
        {
            UpgradeParameters.DidPlayerHeal = true;
        }

        handler.UpgradeSelected();
    }

    public void DisplayDescription(Button buttonHovered)
    {
        handler.UpdateDescription(buttonHovered.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text);
    }

    public void ClearDescription()
    {
        handler.UpdateDescription("");
    }

    public void SelectNextUpgrades()
    {
        upgradeDescriptions = new List<string>();
        upgradesCopy = UpgradesDeepCopy(Upgrades);

        for (int i = 0; i < UpgradesButton.Count; i++)
        {
            UpgradeClass upgrade = GetNextUpgrade();

            if (upgrade != null)
            {
                upgradeDescriptions.Add(upgrade.Description);

                UpgradesButton[i].GetComponent<Image>().sprite = upgrade.Sprite;
                UpgradesButton[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = upgrade.Description;
            }
            else
            {
                // Not enough upgrades, cancel operation
                handler.UpgradeSelected();
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        handler = HandlerObject.GetComponent<GameHandler>();

        HealButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Heal 33% of your maximum life points";
    }

    private List<UpgradeClass> UpgradesDeepCopy(List<UpgradeClass> source)
    {
        List<UpgradeClass> copy = new List<UpgradeClass>();
        foreach (UpgradeClass upgrade in source)
        {
            copy.Add(new UpgradeClass
            {
                Sprite = upgrade.Sprite,
                CurrentLevel = upgrade.CurrentLevel,
                LevelMax = upgrade.LevelMax,
                Description = upgrade.Description,
                UpgradeType = upgrade.UpgradeType
            });
        }

        return copy;
    }

    private UpgradeClass GetNextUpgrade()
    {
        Dictionary<UpgradeClass, float> probabilities = ComputeProbabilities();

        float rand = Random.Range(0f, 1f);

        UpgradeClass upgrade = null;
        foreach (var pair in probabilities)
        {
            if (rand <= pair.Value)
            {
                upgrade = pair.Key;
                upgrade.CurrentLevel++;
                break;
            }
        }
        
        return upgrade;
    }

    private Dictionary<UpgradeClass, float> ComputeProbabilities()
    {
        Dictionary<UpgradeClass, float> probabilities = new Dictionary<UpgradeClass, float>();

        // Set weight according to levels left
        foreach (UpgradeClass upgrade in upgradesCopy)
        {
            float weight = upgrade.LevelMax - upgrade.CurrentLevel;
            if (weight > 0)
            {
                probabilities.Add(upgrade, weight);
            }
        }

        // Convert it into a probability
        Dictionary<UpgradeClass, float> updatedProbabilities = new Dictionary<UpgradeClass, float>();
        float sum = probabilities.Sum(x => x.Value);
        foreach (var pair in probabilities)
        {
            updatedProbabilities.Add(pair.Key, pair.Value / sum);
        }

        // Sum them to get an accumulative distribution
        Dictionary<UpgradeClass, float> cumulatedProbabilities = new Dictionary<UpgradeClass, float>();
        float cumulativeSum = 0f;
        foreach (var pair in updatedProbabilities)
        {
            cumulativeSum += pair.Value;
            cumulatedProbabilities.Add(pair.Key, cumulativeSum);
        }

        return cumulatedProbabilities;
    }

    private void ApplyUpgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.BULLET_SPEED:
                UpgradeParameters.BulletSpeed *= UpgradeParameters.BulletSpeedMult;
                break;
            case UpgradeType.BULLET_TIME:
                UpgradeParameters.BulletTime *= UpgradeParameters.BulletTimeMult;
                break;
            case UpgradeType.BULLET_DAMAGES:
                UpgradeParameters.BulletDamages *= UpgradeParameters.BulletDamagesMult;
                break;
            case UpgradeType.BULLET_SCALE:
                UpgradeParameters.BulletScale *= UpgradeParameters.BulletScaleMult;
                break;
            case UpgradeType.PLAYER_SPEED:
                UpgradeParameters.PlayerSpeed *= UpgradeParameters.PlayerSpeedMult;
                break;
            case UpgradeType.PLAYER_LIFE:
                UpgradeParameters.PlayerLife *= UpgradeParameters.PlayerLifeMult;
                break;
            case UpgradeType.ENNEMY_SPEED:
                UpgradeParameters.EnnemySpeed *= UpgradeParameters.EnnemySpeedMult;
                break;
            default:
                Debug.Log("Upgrade not implemented yet (" + upgradeType + ")");
                break;
        }
    }
}
