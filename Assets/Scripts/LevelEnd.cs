using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

namespace pf
{
    public class LevelEnd : MonoBehaviour
    {
        private Image fadeImage;
        private GameObject trophyBase;
        private Image[] trophies = new Image[3];
        private TextLocalizerUI titleLocalizer;
        private GameObject successText;
        private GameObject failureText;
        private TextLocalizerUI helper1Localizer;
        private TextLocalizerUI helper2Localizer;
        private TextLocalizerUI helper3Localizer;

        private PlayerScore score;

        private bool levelEndReady = false;
        private bool advanceTrophies = false;

        enum State
        {
            Init,
            Start,
            Trophy1,
            Trophy2,
            Trophy3
        };

        private State state = State.Init;
        private bool runStateUpdate = false;

        private Color SUCCESS_COLOR = Color.white;
        private Color FAILURE_COLOR = new Color(0.3f, 0.3f, 0.3f, 1);

        private class LevelEndVariables
        {
            public bool[] success;
            public int hits;
            public int requiredScore;
            public int score;
            public int hearts;
            public float parTime;
            public float playerTime;
            
            public LevelEndVariables(bool _first, bool _second, bool _third, int _hits, int _requiredScore, int _score, int _hearts, float _parTime, float _playerTime)
            {
                success = new bool[3];
                success[0] = _first;
                success[1] = _second;
                success[2] = _third;
                hits = _hits;
                requiredScore = _requiredScore;
                score = _score;
                hearts = _hearts;
                parTime = _parTime;
                playerTime = _playerTime;
            }
        };

        LevelEndVariables lv;

        private void Awake()
        {
            fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
            trophyBase = transform.Find("TrophyBase").gameObject;
            trophies[0] = transform.Find("TrophyBase/Trophy0").GetComponent<Image>();
            trophies[1] = transform.Find("TrophyBase/Trophy1").GetComponent<Image>();
            trophies[2] = transform.Find("TrophyBase/Trophy2").GetComponent<Image>();

            titleLocalizer = transform.Find("TrophyBase/TitleText").GetComponent<TextLocalizerUI>();
            successText = transform.Find("TrophyBase/SuccessText").gameObject;
            failureText = transform.Find("TrophyBase/FailureText").gameObject;
            helper1Localizer = transform.Find("TrophyBase/HelperText1").GetComponent<TextLocalizerUI>();
            helper2Localizer = transform.Find("TrophyBase/HelperText2").GetComponent<TextLocalizerUI>();
            helper3Localizer = transform.Find("TrophyBase/HelperText3").GetComponent<TextLocalizerUI>();
        }

        void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
            //print("Destroy all");
            DOTween.KillAll();
        }

        public void Reset()
        {
            fadeImage.DOFade(0, 0);

            trophyBase.transform.localScale = Vector3.zero;
            trophyBase.SetActive(false);
            for (int i = 0; i < trophies.Length; i++)
            {
                trophies[i].color = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            }

            successText.GetComponent<TextLocalizerUI>().Localize();
            failureText.GetComponent<TextLocalizerUI>().Localize();
            successText.SetActive(false);
            failureText.SetActive(false);

            helper1Localizer.key = "empty";
            helper1Localizer.Localize();

            helper2Localizer.key = "empty";
            helper2Localizer.Localize();            
            
            helper3Localizer.key = "empty";
            helper3Localizer.Localize();

            levelEndReady = false;
            state = State.Init;
        }

        public void ShowLevelEnd(int hits, int score, float timer, int hearts)
        {
            //Debug.Log("Show Level End");
            FadeBackground();
            ShowTrophyBase();
            CheckTrophies(hits, score, timer, hearts);
        }

        public bool LevelEndReady()
        {
            return levelEndReady;
        }

        private void FadeBackground()
        {
            fadeImage.DOFade(0.8f, 1.0f); //.OnComplete(ShowTrophyBase);
        }
        private void ShowTrophyBase()
        {
            trophyBase.SetActive(true);
            trophyBase.transform.DOScale(Defs.MENU_NORMAL_SCALE, 1.0f);
        }

        private void CheckTrophies(int hits, int score, float time, int hearts)
        {

#if UNITY_EDITOR
            // If scene is played directly in the editor, data might not have been parsed
            DataLoader.ParseData();
#endif

            // TODO: check from save/playerdata if player has accomplished any trophies
            // these will be shown always
            bool showFirst = false;
            bool showSecond = false;
            bool showThird = false;

            // Then check if player has accomplished level objectives and
            // show the trophies that are granted to player

            // Check from the saved data if trophies have been given in previous plays
            int level = 0; 
            // Debug hack to get the test level to show the tropies
            if(LevelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.TestLevel)
            {
                level = PlayerStats.GetCurrentLevel();
            }

            //print("*** level is: " + level + " ***");
            float parTime = 0;
            float playerTime = time;

            LevelObjectives o = PlayerStats.LevelObjectives[level];
            if (level >= 0 && level < PlayerStats.CompletedObjectives.Length)
            {
                parTime = o.GetRequiredTime();
                PlayerStats.LevelsCompleted[level] = true;

                //TODO: heart multiplier for score
                int fullScore = score;  //+ (hearts * Defs.HEALTH_BONUS_MULTIPLIER);

                if (hits == 0)
                {
                    showFirst = true;
                    PlayerStats.CompletedObjectives[level].CompletedNoHits = true;
                }
                if (fullScore >= o.GetRequiredScore())
                {
                    showSecond = true;
                    PlayerStats.CompletedObjectives[level].CompletedPoints = true;
                }
                if (time <= parTime)
                {
                    showThird = true;
                    PlayerStats.CompletedObjectives[level].CompletedTime = true;
                }
            }

            lv = new LevelEndVariables(showFirst, showSecond, showThird, hits, o.GetRequiredScore(), score, hearts, parTime, playerTime);

            //Debug.Log("Change from State.Init to State.Start");
            ChangeState(State.Start);

        }

        private void ChangeState(State s)
        {
            //Debug.Log("Changing state to: " + s);
            state = s;
            runStateUpdate = true;
        }

        private void Update()
        {
            if (!runStateUpdate)
            {
                return;
            }

            switch (state)
            {
                case State.Start:
                {
                    //Debug.Log("State.Start");
                    StartCoroutine(WaitBeforeStateChange(1.0f));           
                    break;
                }
                case State.Trophy1:
                {
                    //Debug.Log("State.T1");
                    LocalizeFirst(lv.success[0], true, lv.hits);
                    StartCoroutine(WaitBeforeStateChange(2.0f));
                    break;
                }
                case State.Trophy2:
                {
                    //Debug.Log("State.T2");
                    LocalizeSecond(lv.success[1], true, lv.requiredScore, lv.score, lv.hearts);
                    StartCoroutine(WaitBeforeStateChange(2.0f));
                    break;
                }
                case State.Trophy3:
                {
                    //Debug.Log("State.T3");
                    LocalizeThird(lv.success[2], true, lv.parTime, lv.playerTime);
                    StartCoroutine(WaitBeforeStateChange(1.0f));
                    break;
                }
            }

            runStateUpdate = false;
        }

        private IEnumerator WaitBeforeStateChange(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            switch(state)
            {
                case State.Start:
                {
                    ChangeState(State.Trophy1);
                    break;
                }
                case State.Trophy1:
                {
                    ChangeState(State.Trophy2);
                    break;
                }
                case State.Trophy2:
                {
                    ChangeState(State.Trophy3);
                    break;
                }
                case State.Trophy3:
                {
                    if(!levelEndReady)
                    {
                        levelEndReady = true;
                    }
                    ChangeState(State.Start);
                    break;
                }
            }
        }

        /*
        private IEnumerator ShowTrophies(LevelEndVariables l)
        {
            //TODO:
            // -- create update states, check the state and render stuff in update, not in coroutine!
            // 1) Advance to next trophy if player presses jump button
            // 2) Animate values 
            // 3) Finally if not firstRun (check!), animate trophies/values if saved progress better than latest run

            yield return new WaitForSeconds(1.0f);
            LocalizeFirst(l.success[0], true, l.hits);
            yield return new WaitForSeconds(3.0f);
            LocalizeSecond(l.success[1], true, l.requiredScore, l.score, l.hearts);
            yield return new WaitForSeconds(3.0f);
            LocalizeThird(l.success[2], true, l.parTime, l.playerTime);
            levelEndReady = true;
            yield return new WaitForSeconds(2.0f);
            RunLevelEnd(l, false);
        }
        */

        /*
        private void RunLevelEnd(LevelEndVariables levelEndVariables, bool isFirstRun)
        {
            StartCoroutine(ShowTrophies(levelEndVariables));
        }
        */

        void AnimateTrophy(int t)
        {
            switch(t)
            {
                case 0:
                {
                    trophies[0].transform.DOScale(0.6f, 0.1f);
                    trophies[1].transform.DOScale(0.5f, 0.1f);
                    trophies[2].transform.DOScale(0.5f, 0.1f);
                    break;
                }
                case 1:
                {
                    trophies[0].transform.DOScale(0.5f, 0.1f);
                    trophies[1].transform.DOScale(0.6f, 0.1f);
                    trophies[2].transform.DOScale(0.5f, 0.1f);
                    break;
                }
                case 2:
                {
                    trophies[0].transform.DOScale(0.5f, 0.1f);
                    trophies[1].transform.DOScale(0.5f, 0.1f);
                    trophies[2].transform.DOScale(0.6f, 0.1f);
                    break;
                }
                default:
                    break;
            }

        }

        void LocalizeFirst(bool show, bool firstRun, int hits)
        {
            AnimateTrophy(0);
            if (show)
            {
                trophies[0].DOColor(SUCCESS_COLOR, 1.0f);
                successText.SetActive(true);
                failureText.SetActive(false);

                helper1Localizer.key = "flawless_run";
                helper1Localizer.Localize();
            }
            else
            {
                trophies[0].DOColor(FAILURE_COLOR, 1.0f);
                successText.SetActive(false);
                failureText.SetActive(true);

                if (hits == 1)
                {
                    helper1Localizer.key = "you_died_1_time";
                    helper1Localizer.Localize();
                }
                else
                {
                    helper1Localizer.key = "you_died_x_times";
                    helper1Localizer.Localize();
                    string t = helper1Localizer.GetText();
                    t = t.Replace("<x>", hits.ToString());
                    helper1Localizer.SetText(t);
                }
            }
            titleLocalizer.key = "no_hits";
            titleLocalizer.Localize();

            helper2Localizer.key = "empty";
            helper2Localizer.Localize();

            helper3Localizer.key = "empty";
            helper3Localizer.Localize();
        }

        void LocalizeSecond(bool show, bool firstRun, int requiredScore, int playerScore, int hearts)
        {
            AnimateTrophy(1);
            if (show)
            {
                trophies[1].DOColor(SUCCESS_COLOR, 1.0f);
                successText.SetActive(true);
                failureText.SetActive(false);
            }
            else
            {
                trophies[1].DOColor(FAILURE_COLOR, 1.0f);
                successText.SetActive(false);
                failureText.SetActive(true);
            }
            titleLocalizer.key = "got_required_score";
            titleLocalizer.Localize();

            helper1Localizer.key = "required_score";
            helper1Localizer.Localize();
            string reqScoreStr = helper1Localizer.GetText() + " " + requiredScore;
            helper1Localizer.SetText(reqScoreStr);

            helper2Localizer.key = "your_score";
            helper2Localizer.Localize();
            string yourScoreStr = helper2Localizer.GetText() + " " + playerScore;
            helper2Localizer.SetText(yourScoreStr);

            
        }

        void LocalizeThird(bool show, bool firstRun, float parTime, float playerTime)
        {
            AnimateTrophy(2);
            if (show)
            {
                trophies[2].DOColor(SUCCESS_COLOR, 1.0f);
                successText.SetActive(true);
                failureText.SetActive(false);
            }
            else
            {
                trophies[2].DOColor(FAILURE_COLOR, 1.0f);
                successText.SetActive(false);
                failureText.SetActive(true);
            }
            titleLocalizer.key = "time_under_par";
            titleLocalizer.Localize();

            TimeSpan ts = TimeSpan.FromMilliseconds(parTime);
            helper1Localizer.key = "par_time";
            helper1Localizer.Localize();
            string parTimeStr = helper1Localizer.GetText() + " " + string.Format("{0:D2}:{1:D2}:{2:D3}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            helper1Localizer.SetText(parTimeStr);

            ts = TimeSpan.FromMilliseconds(playerTime);
            helper2Localizer.key = "your_time";
            helper2Localizer.Localize();
            string ownTimeStr = helper2Localizer.GetText() + " " + string.Format("{0:D2}:{1:D2}:{2:D3}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            helper2Localizer.SetText(ownTimeStr);
        }
    }
}
