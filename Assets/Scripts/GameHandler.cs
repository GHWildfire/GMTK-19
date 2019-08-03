using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameObject levelsContainer;

    private GameObject[] levels;
    private int levelIndex;
    
    private void Start()
    {
        levelIndex = 0;

        FillLevels();
        ActivateLevel();
    }

    private void FillLevels()
    {
        levels = new GameObject[levelsContainer.transform.childCount];
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = levelsContainer.transform.GetChild(i).gameObject;
        }
    }

    private void ActivateLevel()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == levelIndex);
        }
    }

    private void Update()
    {
        HandleKeys();
    }

    private void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            levelIndex = (levelIndex - 1 + levels.Length) % levels.Length;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            levelIndex = (levelIndex + 1) % levels.Length;
        }
        ActivateLevel();
    }
}
