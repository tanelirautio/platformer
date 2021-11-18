using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    const int MAX_SCORE_CHARACTER_COUNT = 8;
    const float UI_UPDATE_WAIT_TIME = 0.0001f;

    private int currentScore;
    private TextMeshProUGUI scoreText;
    private bool coroutineRunning = false;

    private void Awake()
    {
        GameObject scoreTextObj = GameObject.Find("UICanvas/Score/ScoreText");
        Assert.IsNotNull(scoreTextObj);
        scoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
    }

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
            yield return new WaitForSeconds(0.01f);
        }
        coroutineRunning = false;
    }
}
