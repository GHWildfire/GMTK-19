using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuManager : MonoBehaviour
{
    public delegate void RestartGameEvent();
    public static event RestartGameEvent OnRestartGameEvent;

    public delegate void ResumeGameEvent();
    public static event ResumeGameEvent OnResumeGameEvent;

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
