using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameHandler : MonoBehaviour
{
    public delegate void EndGameEvent(bool isWinner, float elapsedTimeValue, int reachedLevelValue);
    public static event EndGameEvent OnEndGameEvent;

    public delegate void PauseResumeGameEvent(bool isPaused);
    public static event PauseResumeGameEvent OnPauseResumeGameEvent;

    public delegate void RestartGameEvent();
    public static event RestartGameEvent OnRestartGameEvent;

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
    [SerializeField] private Canvas endGameCanvas;

    [Header("Slimes Models")]
    [SerializeField] private GameObject standardSlimeModel;
    [SerializeField] private GameObject fastSlimeModel;
    [SerializeField] private GameObject slowSlimeModel;
    [SerializeField] private GameObject boss1SlimeModel;
    [SerializeField] private GameObject boss2SlimeModel;
    [SerializeField] private GameObject boss3SlimeModel;
    [SerializeField] private GameObject finalBossSlimeModel;

    [Header("Audio")]
    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioClip SlimeDeathSound;
    [SerializeField] private AudioClip GunShot;
    [SerializeField] private AudioClip EmptyGunShot;

    public static Camera SharedCam;

    private const float SWAP_DURATION = 1.5f;
    private const float CAMERA_MAX_OFFSET = 100;

    private GameObject[] levelsObjects;
    private Level[] levels;
    private List<List<(float, SlimeManager.SpawmSlime)>> mobs;
    private Vector3 initCamPos;

    private int levelIndex;

    private bool swapLevel;
    private bool swapDirectionLeft;
    private bool useUpgrade;

    private float initSwapTime;
    private float elapsedTime;
    
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
            currentPlayer.GetComponent<PlayerController>().Init(levelsObjects[levelIndex], Audio, GunShot, EmptyGunShot);
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

        Init();
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
                elapsedTime += Time.deltaTime;

                HandleKeys();
                UpdateLevel();
                slimeManager.Update();
            }
        }
    }

    private void Init()
    {
        UpgradeParameters.Init();

        levelIndex = 0;
        elapsedTime = 0;
        swapLevel = false;
        useUpgrade = false;
        swapDirectionLeft = false;
        initSwapTime = Time.time;
        swapState = SwapState.FADE_OUT;
        currentGameState = GameState.INIT;

        upgardesCanvas.gameObject.SetActive(false);
        inGameCanvas.gameObject.SetActive(false);
        endGameCanvas.gameObject.SetActive(false);

        Description.text = "";

        canvas.SetActive(false);

        currentPlayer = Instantiate(playerModel);

        slimeManager = new SlimeManager(standardSlimeModel, fastSlimeModel, slowSlimeModel,
            boss1SlimeModel, boss2SlimeModel, boss3SlimeModel, finalBossSlimeModel, currentPlayer,
            Audio, SlimeDeathSound);

        FillSlimes();
        FillLevels();
        ActivateLevel();

        currentPlayer.GetComponent<PlayerController>().Init(levelsObjects[levelIndex], Audio, GunShot, EmptyGunShot);

        currentGameState = GameState.START;
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

        OnPauseResumeGameEvent(currentGameState == GameState.START);
    }

    private void RestartGame()
    {
        currentGameState = GameState.INIT;

        OnRestartGameEvent();

        foreach (List<(float, SlimeManager.SpawmSlime)> item in mobs)
        {
            item.Clear();
        }
        mobs.Clear();

        levels[levelIndex].ClearSlimes();

        Init();
    }

    private void CheckingPlayerAlive()
    {
        if (currentPlayer == null &&
            currentGameState == GameState.START)
        {
            endGameCanvas.gameObject.SetActive(true);
            currentGameState = GameState.END;

            OnEndGameEvent(false, elapsedTime, levelIndex + 1);
        }
    }

    private void FillSlimes()
    {
        mobs = new List<List<(float, SlimeManager.SpawmSlime)>>
        {
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnStandard),
                (8, slimeManager.SpawnStandard),
                (15, slimeManager.SpawnStandard),
                (15, slimeManager.SpawnStandard)
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnStandard),
                (8, slimeManager.SpawnSlow),
                (15, slimeManager.SpawnStandard),
                (22, slimeManager.SpawnStandard),
                (22, slimeManager.SpawnSlow)
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnFast),
                (8, slimeManager.SpawnFast),
                (14, slimeManager.SpawnSlow),
                (14, slimeManager.SpawnSlow),
                (25, slimeManager.SpawnSlow),
                (25, slimeManager.SpawnFast),
                (25, slimeManager.SpawnStandard)
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnSlow),
                (2, slimeManager.SpawnSlow),
                (8, slimeManager.SpawnFast),
                (25, slimeManager.SpawnStandard),
                (25, slimeManager.SpawnStandard),
                (25, slimeManager.SpawnStandard),
                (40, slimeManager.SpawnStandard),
                (40, slimeManager.SpawnStandard),
                (40, slimeManager.SpawnStandard)
            },
            // BOSS 1
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss1),
                (2, slimeManager.SpawnBoss1)
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnStandard),
                (2, slimeManager.SpawnStandard),
                (8, slimeManager.SpawnStandard),
                (8, slimeManager.SpawnStandard),
                (14, slimeManager.SpawnStandard),
                (14, slimeManager.SpawnStandard),
                (20, slimeManager.SpawnStandard),
                (20, slimeManager.SpawnStandard),
                (26, slimeManager.SpawnSlow),
                (26, slimeManager.SpawnSlow),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnFast),
                (8, slimeManager.SpawnFast),
                (14, slimeManager.SpawnFast),
                (20, slimeManager.SpawnFast),
                (26, slimeManager.SpawnSlow),
                (26, slimeManager.SpawnSlow),
                (35, slimeManager.SpawnStandard),
                (35, slimeManager.SpawnStandard),
                (35, slimeManager.SpawnStandard),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnSlow),
                (2, slimeManager.SpawnSlow),
                (2, slimeManager.SpawnSlow),
                (2, slimeManager.SpawnSlow),
                (2, slimeManager.SpawnSlow),
                (2, slimeManager.SpawnSlow),
                (30, slimeManager.SpawnSlow),
                (30, slimeManager.SpawnSlow),
                (30, slimeManager.SpawnSlow),
                (30, slimeManager.SpawnSlow),
                (30, slimeManager.SpawnSlow),
                (30, slimeManager.SpawnSlow),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (1, slimeManager.SpawnFast),
                (2, slimeManager.SpawnFast),
                (3, slimeManager.SpawnFast),
                (4, slimeManager.SpawnFast),
                (5, slimeManager.SpawnFast),
                (6, slimeManager.SpawnFast),
                (7, slimeManager.SpawnFast),
                (8, slimeManager.SpawnFast),
                (9, slimeManager.SpawnFast),
                (10, slimeManager.SpawnFast),
            },
            // BOSS 2
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss2)
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnStandard),
                (2, slimeManager.SpawnFast),
                (2, slimeManager.SpawnSlow),
                (10, slimeManager.SpawnStandard),
                (10, slimeManager.SpawnFast),
                (10, slimeManager.SpawnSlow),
                (17, slimeManager.SpawnStandard),
                (17, slimeManager.SpawnFast),
                (17, slimeManager.SpawnSlow),
                (23, slimeManager.SpawnStandard),
                (23, slimeManager.SpawnFast),
                (23, slimeManager.SpawnSlow),
                (28, slimeManager.SpawnStandard),
                (28, slimeManager.SpawnFast),
                (28, slimeManager.SpawnSlow),
                (32, slimeManager.SpawnStandard),
                (32, slimeManager.SpawnFast),
                (32, slimeManager.SpawnSlow),
                (35, slimeManager.SpawnStandard),
                (35, slimeManager.SpawnFast),
                (35, slimeManager.SpawnSlow),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnStandard),
                (3, slimeManager.SpawnStandard),
                (4, slimeManager.SpawnStandard),
                (5, slimeManager.SpawnSlow),
                (6, slimeManager.SpawnStandard),
                (7, slimeManager.SpawnStandard),
                (8, slimeManager.SpawnStandard),
                (9, slimeManager.SpawnStandard),
                (10, slimeManager.SpawnSlow),
                (11, slimeManager.SpawnStandard),
                (12, slimeManager.SpawnStandard),
                (13, slimeManager.SpawnStandard),
                (14, slimeManager.SpawnStandard),
                (15, slimeManager.SpawnSlow),
                (16, slimeManager.SpawnStandard),
                (17, slimeManager.SpawnStandard),
                (18, slimeManager.SpawnStandard),
                (19, slimeManager.SpawnStandard),
                (20, slimeManager.SpawnSlow),
                (21, slimeManager.SpawnStandard),
                (22, slimeManager.SpawnStandard),
                (23, slimeManager.SpawnStandard),
                (24, slimeManager.SpawnStandard),
                (25, slimeManager.SpawnSlow),
                (26, slimeManager.SpawnStandard),
                (27, slimeManager.SpawnStandard),
                (28, slimeManager.SpawnStandard),
                (29, slimeManager.SpawnStandard),
                (30, slimeManager.SpawnSlow),
                (31, slimeManager.SpawnStandard),
                (32, slimeManager.SpawnStandard),
                (33, slimeManager.SpawnStandard),
                (34, slimeManager.SpawnStandard),
                (35, slimeManager.SpawnSlow),
                (36, slimeManager.SpawnStandard),
                (37, slimeManager.SpawnStandard),
                (38, slimeManager.SpawnStandard),
                (39, slimeManager.SpawnStandard),
                (40, slimeManager.SpawnSlow),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnSlow),
                (3, slimeManager.SpawnSlow),
                (4, slimeManager.SpawnSlow),
                (5, slimeManager.SpawnSlow),
                (6, slimeManager.SpawnSlow),
                (7, slimeManager.SpawnSlow),
                (8, slimeManager.SpawnSlow),
                (9, slimeManager.SpawnSlow),
                (10, slimeManager.SpawnFast),
                (11, slimeManager.SpawnSlow),
                (12, slimeManager.SpawnSlow),
                (13, slimeManager.SpawnSlow),
                (14, slimeManager.SpawnSlow),
                (15, slimeManager.SpawnSlow),
                (16, slimeManager.SpawnSlow),
                (17, slimeManager.SpawnSlow),
                (18, slimeManager.SpawnSlow),
                (19, slimeManager.SpawnSlow),
                (20, slimeManager.SpawnFast),
                (21, slimeManager.SpawnSlow),
                (22, slimeManager.SpawnSlow),
                (23, slimeManager.SpawnSlow),
                (24, slimeManager.SpawnSlow),
                (25, slimeManager.SpawnSlow),
                (26, slimeManager.SpawnSlow),
                (27, slimeManager.SpawnSlow),
                (28, slimeManager.SpawnSlow),
                (29, slimeManager.SpawnSlow),
                (30, slimeManager.SpawnFast),
                (31, slimeManager.SpawnSlow),
                (32, slimeManager.SpawnSlow),
                (33, slimeManager.SpawnSlow),
                (34, slimeManager.SpawnSlow),
                (35, slimeManager.SpawnSlow),
                (36, slimeManager.SpawnSlow),
                (37, slimeManager.SpawnSlow),
                (38, slimeManager.SpawnSlow),
                (39, slimeManager.SpawnSlow),
                (40, slimeManager.SpawnFast),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnStandard),
                (2, slimeManager.SpawnStandard),
                (2, slimeManager.SpawnStandard),
                (2, slimeManager.SpawnStandard),
                (10, slimeManager.SpawnFast),
                (10, slimeManager.SpawnFast),
                (10, slimeManager.SpawnFast),
                (10, slimeManager.SpawnFast),
                (18, slimeManager.SpawnSlow),
                (18, slimeManager.SpawnSlow),
                (18, slimeManager.SpawnSlow),
                (18, slimeManager.SpawnSlow),
                (26, slimeManager.SpawnStandard),
                (26, slimeManager.SpawnStandard),
                (26, slimeManager.SpawnStandard),
                (26, slimeManager.SpawnStandard),
                (34, slimeManager.SpawnFast),
                (34, slimeManager.SpawnFast),
                (34, slimeManager.SpawnFast),
                (34, slimeManager.SpawnFast),
                (42, slimeManager.SpawnSlow),
                (42, slimeManager.SpawnSlow),
                (42, slimeManager.SpawnSlow),
                (42, slimeManager.SpawnSlow),
            },
            // BOSS 3
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss3)
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss2),
                (2, slimeManager.SpawnBoss2),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss3),
                (2, slimeManager.SpawnBoss3),
            },
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss1),
                (2, slimeManager.SpawnBoss1),
                (2, slimeManager.SpawnBoss1),
            },
            // ALL BOSSES
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss1),
                (2, slimeManager.SpawnBoss1),
                (2, slimeManager.SpawnBoss3)
            },
            // Final BOSS
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnFinalBoss)
            },
            // Oups level
            new List<(float, SlimeManager.SpawmSlime)>
            {
                (2, slimeManager.SpawnBoss1)
            }
        };

        // Resolve slime manager slimes spawning
        foreach (List<(float, SlimeManager.SpawmSlime)> item in mobs)
        {
            item.Reverse();
        }
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
                spawns[j].SetActive(false);
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
                    if (levelIndex >= levels.Length - 1)
                    {
                        initSwapTime = Time.time;
                        currentPlayer.GetComponent<PlayerController>().Init(levelsObjects[levelIndex], Audio, GunShot, EmptyGunShot);
                        swapState = SwapState.FADE_IN;
                    }
                    else
                    {
                        swapState = SwapState.DISPLAY_UPGRADES;
                    }
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
        activeLevel.Update();

        if (levelIndex < levelsObjects.Length - 1)
        {
            if (activeLevel.Ended())
            {
                ActivateSwap(false);
                slimeManager.Init();
            }
        }
        else
        {
            if (currentGameState == GameState.START)
            {
                endGameCanvas.gameObject.SetActive(true);
                currentGameState = GameState.END;

                OnEndGameEvent(true, elapsedTime, levelIndex + 1);
            }
        }
    }

    private void HandleKeys()
    {
        /* Debug
        if (Input.GetKeyDown(KeyCode.K) && levelIndex > 0)
        {
            ActivateSwap(true);
        }
        else if (Input.GetKeyDown(KeyCode.L) && levelIndex < levelsObjects.Length - 1)
        {
            ActivateSwap(false);
        }
        */
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
