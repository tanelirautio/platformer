using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System;

namespace pf
{
    public class PlayerScore : MonoBehaviour
    {
        const int MAX_SCORE_CHARACTER_COUNT = 8;
        const float UI_UPDATE_WAIT_TIME = 0.0001f;

        private int score = 0;
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
            //print("Reset player score");
            score = 0;
            string str = score.ToString();
            scoreText.text = str.PadLeft(MAX_SCORE_CHARACTER_COUNT, '0');
        }

        public void AddScore(int score)
        {
            this.score += score;

            //debug
            //print("Current level score: " + this.score);
            UpdateUiScore();
        }

        public int GetScore()
        {
            return score;
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

            while (currentUiScore != score)
            {
                if (currentUiScore < score)
                {
                    currentUiScore = currentUiScore + 5;
                    if (currentUiScore > score)
                    {
                        currentUiScore = score;
                    }
                }
                else
                {
                    currentUiScore = currentUiScore - 5;
                    if (currentUiScore < score)
                    {
                        currentUiScore = score;
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
