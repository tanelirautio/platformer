using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    const int MAX_SCORE_CHARACTER_COUNT = 8;
    const float UI_UPDATE_WAIT_TIME = 0.0001f;

    private int currentScore = 0;
    private TextMeshProUGUI scoreText;
    private bool coroutineRunning = false;

    private LevelLoader levelLoader;

    private void Awake()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

        GameObject scoreTextObj = GameObject.Find("UICanvas/Score/ScoreText");
        Assert.IsNotNull(scoreTextObj);
        scoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
    }

    public void Reset()
    {
        print("Reset player score");

        if (levelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.StartLevel &&
            LevelLoader.GetPreviousSceneIndex() != -1)
        {
            print("get value from PlayerStats.Score");
            currentScore = PlayerStats.Score;

            string str = currentScore.ToString();
            scoreText.text = str.PadLeft(MAX_SCORE_CHARACTER_COUNT, '0');
        }
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
