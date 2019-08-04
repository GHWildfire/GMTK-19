using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameHandler : MonoBehaviour
{
    private enum SwapState
    {
        FADE_OUT,
        DISPLAY_UPGRADES,
        WAIT_CHOICE,
        FADE_IN,
        FINISHED
    };

    public enum GameState
    {
        INIT,
        START,
        PAUSE,
        SWAP_LEVEL,
        END
    }

    [SerializeField] private GameObject levelsContainer;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private Sprite slimeSprite;
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private EventHandler eventHandler;

    [Header("Canvas")]
    [SerializeField] private Canvas upgardesCanvas;
    [SerializeField] private Canvas inGameCanvas;

    [Header("Slimes Models")]
    [SerializeField] private GameObject standardSlimeModel;
    [SerializeField] private GameObject fastSlimeModel;
    [SerializeField] private GameObject slowSlimeModel;

    public static Camera SharedCam;

    private GameObject[] levelsObjects;
    private Level[] levels;
    private List<List<(float, SlimeManager.SpawmSlime)>> mobs;
    private int levelIndex;
    private float initTime;
    private Vector3 initCamPos;

    private bool swapLevel;
    private bool swapDirectionLeft;
    private float initSwapTime;
    private bool useUpgrade;

    private const float SWAP_DURATION = 1.5f;
    private const float CAMERA_MAX_OFFSET = 50;
    
    private SwapState swapState;
    private GameState currentGameState;

    private SlimeManager slimeManager;

    private GameObject currentPlayer;

    public void UpgradeSelected()
    {
        if (swapState == SwapState.WAIT_CHOICE)
        {
            canvas.SetActive(false);
            initSwapTime = Time.time;
            swapState = SwapState.FADE_IN;
            currentPlayer.GetComponent<PlayerController>().Init();
        }
    }

    public void UpdateDescription(string description)
    {
        if (swapState == SwapState.WAIT_CHOICE)
        {
            Description.text = description;
        }
    }

    private void Awake()
    {
        SharedCam = mainCam;

        levelIndex = 0;
        initTime = Time.time;
        swapLevel = false;
        useUpgrade = false;
        swapDirectionLeft = false;
        initSwapTime = Time.time;
        swapState = SwapState.FADE_OUT;
        currentGameState = GameState.INIT;

        upgardesCanvas.gameObject.SetActive(false);
        inGameCanvas.gameObject.SetActive(false);

        Description.text = "";

        canvas.SetActive(false);

        currentPlayer = Instantiate(playerModel);

        slimeManager = new SlimeManager(standardSlimeModel, fastSlimeModel, slowSlimeModel, currentPlayer);

        FillSlimes();
        FillLevels();
        ActivateLevel();

        currentPlayer.GetComponent<PlayerController>().Init();
    }

    private void OnEnable()
    {
        InGameMenuManager.OnResumeGameEvent += ResumeOrPauseGame;
        InGameMenuManager.OnRestartGameEvent += RestartGame;
    }

    private void OnDisable()
    {
        InGameMenuManager.OnResumeGameEvent -= ResumeOrPauseGame;
        InGameMenuManager.OnRestartGameEvent -= RestartGame;
    }

    private void Start()
    {
        currentGameState = GameState.START;
    }

    private void Update()
    {
        if (swapLevel)
        {
            SwapLevel();
        }
        else
        {
            CheckInputs();
            CheckingPlayerAlive();

            if (currentGameState == GameState.START)
            {
                HandleKeys();
                UpdateLevel();
                slimeManager.Update();
            }
        }
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeOrPauseGame();
        }
    }

    private void ResumeOrPauseGame()
    {
        if (currentGameState == GameState.START)
        {
            currentGameState = GameState.PAUSE;

            inGameCanvas.gameObject.SetActive(true);
        }
        else if (currentGameState == GameState.PAUSE)
        {
            currentGameState = GameState.START;

            inGameCanvas.gameObject.SetActive(false);
        }

        // TODO
        // currentPlayer.getCom<playerCon>().ChangeGameState(curentGameState);
        // Same for slimes
    }

    private void RestartGame()
    {
        Debug.Log("RESTARTED");
    }

    private void CheckingPlayerAlive()
    {
        if (currentPlayer == null)
        {
            Debug.Log("YOU LOST !");
            currentGameState = GameState.END;
        }
    }

    private void FillSlimes()
    {
        mobs = new List<List<(float, SlimeManager.SpawmSlime)>>
        {
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnStandard),
                (4, slimeManager.SpawnStandard),
                (6, slimeManager.SpawnStandard),
                (8, slimeManager.SpawnSlow)
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnSlow),
                (4, slimeManager.SpawnStandard),
                (6, slimeManager.SpawnSlow),
                (8, slimeManager.SpawnStandard)
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

        slimeManager.ChangeLevel(
            levelsObjects[levelIndex].transform.Find("Spawns"),
            levelsObjects[levelIndex].transform.Find("Slimes")
        );
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
                mainCam.transform.position = initCamPos + direction * new Vector3(cameraOffsetOut, 0, 0);
                if (timePassed > SWAP_DURATION)
                {
                    swapState = SwapState.DISPLAY_UPGRADES;
                }
                break;

            case SwapState.DISPLAY_UPGRADES:
                currentGameState = GameState.SWAP_LEVEL;

                levels[levelIndex].ClearSlimes();

                if (swapDirectionLeft)
                {
                    levelIndex--;
                    initSwapTime = Time.time;
                    swapState = SwapState.FADE_IN;
                }
                else
                {
                    eventHandler.SelectNextUpgrades();
                    canvas.SetActive(true);
                    levelIndex++;
                    swapState = SwapState.WAIT_CHOICE;
                }
                ActivateLevel();
                break;

            case SwapState.WAIT_CHOICE:
                break;

            case SwapState.FADE_IN:
                if (Mathf.Abs(SWAP_DURATION - timePassed) > 0.1f)
                {
                    float cameraOffsetIn = Mathf.Pow(SWAP_DURATION - timePassed, factor);
                    mainCam.transform.position = initCamPos - direction * new Vector3(cameraOffsetIn, 0, 0);
                }
                if (timePassed >= SWAP_DURATION)
                {
                    swapState = SwapState.FINISHED;
                }
                break;

            case SwapState.FINISHED:
                initTime = Time.time;
                mainCam.transform.position = initCamPos;
                swapLevel = false;
                Description.text = "";
                currentGameState = GameState.START;
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
        initCamPos = mainCam.transform.position;
        swapState = SwapState.FADE_OUT;
    }
}
