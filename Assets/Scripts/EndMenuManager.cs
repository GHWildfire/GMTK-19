using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text titleWon;
    [SerializeField] private TMP_Text titleLost;
    [SerializeField] private TMP_Text elapsedTime;
    [SerializeField] private TMP_Text reachedLevel;
    [SerializeField] private GameObject notRageQuitElement;

    private void OnEnable()
    {
        GameHandler.OnEndGameEvent += EndGame;
    }

    private void OnDisable()
    {
        GameHandler.OnEndGameEvent -= EndGame;
    }

    private void EndGame(bool isWinner, float elapsedTimeValue, int reachedLevelValue)
    {
        titleWon.gameObject.SetActive(isWinner);
        titleLost.gameObject.SetActive(!isWinner);
        notRageQuitElement.SetActive(isWinner);

        elapsedTime.text = ConvertFloatToDisplayableMinutesSeconds(elapsedTimeValue);

        string reachedLevelText = reachedLevelValue.ToString();
        if (isWinner)
        {
            reachedLevelText += " (MAX)";
        }
        reachedLevel.text = reachedLevelText;
    }

    /// <summary>
    /// Convert a given float to a displayable format (minutes, seconds)
    /// </summary>
    /// <param name="initSeconds"></param>
    /// <returns></returns>
    private string ConvertFloatToDisplayableMinutesSeconds(float initSeconds)
    {
        int minutes = (int)(initSeconds / 60);
        int seconds = (int)(initSeconds - minutes * 60);
        int millis = (int)((initSeconds - (seconds + minutes * 60)) * 1000);
        return minutes.ToString("00") + "min " + seconds.ToString("00") + "s";
    }
}
