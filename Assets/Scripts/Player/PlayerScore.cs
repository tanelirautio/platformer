using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    const int MAX_SCORE_CHARACTER_COUNT = 8;
    const float UI_UPDATE_WAIT_TIME = 0.0001f;

    private int currentScore;
    public TextMeshProUGUI scoreText;
    private bool coroutineRunning = false;

    public void AddScore(int score)
    {
        currentScore += score;

        //debug
        print("Current score: " + currentScore);
        UpdateUiScore();
    }

    public int GetScore()
    {
        return currentScore;
    }

    private void UpdateUiScore()
    {
        if (!coroutineRunning)
        {
            StartCoroutine(UpdateAndWaitUiScore());
        }
    }

    private IEnumerator UpdateAndWaitUiScore()
    {
        coroutineRunning = true;
        int currentUiScore = int.Parse(scoreText.text);

        while (currentUiScore != currentScore)
        {
            if(currentUiScore < currentScore)
            {
                currentUiScore = currentUiScore + 5;
                if (currentUiScore > currentScore)
                {
                    currentUiScore = currentScore;
                }
            }
            else
            {
                currentUiScore = currentUiScore - 5;
                if (currentUiScore < currentScore)
                {
                    currentUiScore = currentScore;
                }
            }
            
            string str = currentUiScore.ToString();
            scoreText.text = str.PadLeft(MAX_SCORE_CHARACTER_COUNT, '0');
            yield return null;
        }
        coroutineRunning = false;
    }
}
