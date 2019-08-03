using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameObject levelsContainer;
    [SerializeField] private Sprite slimeSprite;

    private GameObject[] levelsObjects;
    private Level[] levels;
    private List<List<(float, Slime)>> mobs;
    private int levelIndex;
    private float initTime;
    
    private void Start()
    {
        levelIndex = 0;
        initTime = Time.time;

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
        HandleKeys();
        UpdateLevel();
    }

    private void UpdateLevel()
    {
        Level activeLevel = levels[levelIndex];
        activeLevel.Update(Time.time - initTime);
    }

    private void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            levelIndex = (levelIndex - 1 + levelsObjects.Length) % levelsObjects.Length;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            levelIndex = (levelIndex + 1) % levelsObjects.Length;
        }
        ActivateLevel();
    }
}
