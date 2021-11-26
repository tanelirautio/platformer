using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

namespace pf
{
    public class PlayerScore : MonoBehaviour
    {
        const int MAX_SCORE_CHARACTER_COUNT = 8;
        const float UI_UPDATE_WAIT_TIME = 0.0001f;

        private int totalScore = 0;
        private int levelScore = 0;
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
            levelScore = 0;

            if (levelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.StartLevel &&
                LevelLoader.GetPreviousSceneIndex() != -1)
            {
                print("get values from PlayerStats.Scores");

                for (int i = 0; i < PlayerStats.Level; i++)
                {
                    totalScore += PlayerStats.Scores[i];
                }

                string str = totalScore.ToString();
                scoreText.text = str.PadLeft(MAX_SCORE_CHARACTER_COUNT, '0');
            }
        }

        public void AddScore(int score)
        {
            levelScore += score;
            totalScore += score;

            //debug
            print("Current total score: " + totalScore);
            print("Current level score: " + levelScore);
            UpdateUiScore();
        }

        public int GetLevelScore()
        {
            return levelScore;
        }

        public int GetTotalScore()
        {
            return totalScore;
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

            while (currentUiScore != totalScore)
            {
                if (currentUiScore < totalScore)
                {
                    currentUiScore = currentUiScore + 5;
                    if (currentUiScore > totalScore)
                    {
                        currentUiScore = totalScore;
                    }
                }
                else
                {
                    currentUiScore = currentUiScore - 5;
                    if (currentUiScore < totalScore)
                    {
                        currentUiScore = totalScore;
                    }
                }

                string str = currentUiScore.ToString();
                scoreText.text = str.PadLeft(MAX_SCORE_CHARACTER_COUNT, '0');
                yield return new WaitForSeconds(0.01f);
            }
            coroutineRunning = false;
        }
    }
}
