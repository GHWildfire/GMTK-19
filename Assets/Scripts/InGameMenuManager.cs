using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    public delegate void RestartGameEvent();
    public static event RestartGameEvent OnRestartGameEvent;

    public delegate void ResumeGameEvent();
    public static event ResumeGameEvent OnResumeGameEvent;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Restart()
    {
        OnRestartGameEvent();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        OnResumeGameEvent();
    }
}
