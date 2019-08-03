using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameObject levelsContainer;
    [SerializeField] private Sprite slimeSprite;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject canvas;

    private GameObject[] levelsObjects;
    private Level[] levels;
    private List<List<(float, Slime)>> mobs;
    private int levelIndex;
    private float initTime;
    private Vector3 initCamPos;

    private bool swapLevel;
    private bool swapDirectionLeft;
    private float initSwapTime;
    private bool useUpgrade;

    private const float SWAP_DURATION = 1.5f;
    private const float CAMERA_MAX_OFFSET = 50;

    private enum SwapState { FADE_OUT, DISPLAY_UPGRADES, WAIT_CHOICE, FADE_IN, FINISHED};
    private SwapState swapState;

    public void UpgradeSelected()
    {

    }
    
    private void Start()
    {
        levelIndex = 0;
        initTime = Time.time;
        swapLevel = false;
        useUpgrade = false;
        swapDirectionLeft = false;
        initSwapTime = Time.time;

        swapState = SwapState.FADE_OUT;

        canvas.SetActive(false);

        FillSlimes();
        FillLevels();
        ActivateLevel();
    }

    private void FillSlimes()
    {
        mobs = new List<List<(float, Slime)>>
        {
            new List<(float, Slime)>
            {
                (2, new Slime(0, 0, 0.5f, 0, slimeSprite)),
                (4, new Slime(0, 0, 0.5f, 0, slimeSprite)),
                (6, new Slime(0, 0, 0.5f, 0, slimeSprite)),
                (8, new Slime(0, 0, 0.5f, 0, slimeSprite))
            },
            new List<(float, Slime)>
            {
                (2, new Slime(0, 0, 0.5f, 0, slimeSprite)),
                (4, new Slime(0, 0, 0.5f, 0, slimeSprite)),
                (6, new Slime(0, 0, 0.5f, 0, slimeSprite)),
                (8, new Slime(0, 0, 0.5f, 0, slimeSprite))
            }
        };
    }

    private void FillLevels()
    {
        // Lists
        levelsObjects = new GameObject[levelsContainer.transform.childCount];
        levels = new Level[levelsContainer.transform.childCount];

        for (int i = 0; i < levelsObjects.Length; i++)
        {
            // Level (Object)
            levelsObjects[i] = levelsContainer.transform.GetChild(i).gameObject;

            // Fill spawns
            GameObject spawnsObject = levelsObjects[i].transform.Find("Spawns").gameObject;
            GameObject[] spawns = new GameObject[spawnsObject.transform.childCount];
            for (int j = 0; j < spawnsObject.transform.childCount; j++)
            {
                spawns[j] = spawnsObject.transform.GetChild(j).gameObject;
            }

            // Level (Script)
            Transform slimesTransform = levelsObjects[i].transform.Find("Slimes");
            if (slimesTransform != null)
            {
                levels[i] = new Level(spawns, slimesTransform.gameObject, mobs[i]);
            }
        }
    }

    private void ActivateLevel()
    {
        for (int i = 0; i < levelsObjects.Length; i++)
        {
            levelsObjects[i].SetActive(i == levelIndex);
        }
    }

    private void Update()
    {
        if (swapLevel)
        {
            SwapLevel();
        } 
        else
        {
            HandleKeys();
            UpdateLevel();
        }
    }

    private void SwapLevel()
    {
        float timePassed = Time.time - initSwapTime;
        float factor = Mathf.Log(CAMERA_MAX_OFFSET) / Mathf.Log(SWAP_DURATION);
        float direction = swapDirectionLeft ? -1 : 1;

        switch (swapState)
        {
            case SwapState.FADE_OUT:
                float cameraOffsetOut = Mathf.Pow(timePassed, factor);
                camera.transform.position = initCamPos + direction * new Vector3(cameraOffsetOut, 0, 0);
                if (timePassed > SWAP_DURATION)
                {
                    swapState = SwapState.DISPLAY_UPGRADES;
                }
                break;
            case SwapState.DISPLAY_UPGRADES:
                levels[levelIndex].ClearSlimes();
                ActivateLevel();
                if (swapDirectionLeft)
                {
                    levelIndex--;
                    initSwapTime = Time.time;
                    swapState = SwapState.FADE_IN;
                }
                else
                {
                    canvas.SetActive(true);
                    levelIndex++;
                    swapState = SwapState.WAIT_CHOICE;
                }
                break;
            case SwapState.WAIT_CHOICE:
                canvas.SetActive(false);
                initSwapTime = Time.time;
                swapState = SwapState.FADE_IN;
                break;
            case SwapState.FADE_IN:
                if (Mathf.Abs(SWAP_DURATION - timePassed) > 0.1f)
                {
                    float cameraOffsetIn = Mathf.Pow(SWAP_DURATION - timePassed, factor);
                    camera.transform.position = initCamPos - direction * new Vector3(cameraOffsetIn, 0, 0);
                }
                if (timePassed >= SWAP_DURATION)
                {
                    swapState = SwapState.FINISHED;
                }
                break;
            case SwapState.FINISHED:
                initTime = Time.time;
                camera.transform.position = initCamPos;
                swapLevel = false;
                break;
            default:
                break;
        }
    }

    private void UpdateLevel()
    {
        Level activeLevel = levels[levelIndex];
        activeLevel.Update(Time.time - initTime);

        if (activeLevel.Ended() && levelIndex < levelsObjects.Length - 1)
        {
            ActivateSwap(false);
        }
    }

    private void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.K) && levelIndex > 0)
        {
            ActivateSwap(true);
        }
        else if (Input.GetKeyDown(KeyCode.L) && levelIndex < levelsObjects.Length - 1)
        {
            ActivateSwap(false);
        }
    }

    private void ActivateSwap(bool directionLeft)
    {
        swapLevel = true;
        swapDirectionLeft = directionLeft;
        initSwapTime = Time.time;
        initCamPos = camera.transform.position;
        swapState = SwapState.FADE_OUT;
    }
}
