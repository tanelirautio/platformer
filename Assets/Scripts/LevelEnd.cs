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
        //private TextMeshProUGUI levelCompleteText;
        private TextLocalizerUI titleLocalizer;
        private TextLocalizerUI descriptionLocalizer;
        private TextLocalizerUI helper1Localizer;
        private TextLocalizerUI helper2Localizer;

        private PlayerScore score;

        private bool levelEndReady = false;

        private Color SUCCESS_COLOR = Color.white;
        private Color FAILURE_COLOR = Color.red;

        private void Awake()
        {
            fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
            trophyBase = transform.Find("TrophyBase").gameObject;
            trophies[0] = transform.Find("TrophyBase/Trophy0").GetComponent<Image>();
            trophies[1] = transform.Find("TrophyBase/Trophy1").GetComponent<Image>();
            trophies[2] = transform.Find("TrophyBase/Trophy2").GetComponent<Image>();

            titleLocalizer = transform.Find("TrophyBase/TitleText").GetComponent<TextLocalizerUI>();
            descriptionLocalizer = transform.Find("TrophyBase/DescriptionText").GetComponent<TextLocalizerUI>();
            helper1Localizer = transform.Find("TrophyBase/HelperText1").GetComponent<TextLocalizerUI>();
            helper2Localizer = transform.Find("TrophyBase/HelperText2").GetComponent<TextLocalizerUI>();
        }

        void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
            print("Destroy all");
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

            descriptionLocalizer.key = "empty";
            descriptionLocalizer.Localize();

            helper1Localizer.key = "empty";
            helper1Localizer.Localize();

            helper2Localizer.key = "empty";
            helper2Localizer.Localize();

            levelEndReady = false;
        }

        public void ShowLevelEnd(bool hit, int score, float timer)
        {
            FadeBackground();
            ShowTrophyBase();
            CheckTrophies(hit, score, timer);
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

        private void CheckTrophies(bool hit, int score, float time)
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

            print("*** level is: " + level + " ***");
            float parTime = 0;
            float playerTime = time;

            LevelObjectives o = PlayerStats.LevelObjectives[level];
            if (level >= 0 && level < PlayerStats.CompletedObjectives.Length)
            {
                parTime = o.GetRequiredTime();
                PlayerStats.LevelsCompleted[level] = true;

                if (!hit)
                {
                    showFirst = true;
                }
                if (score >= o.GetRequiredScore())
                {
                    showSecond = true;
                }
                if (time <= parTime)
                {
                    showThird = true;     
                }
            }

            StartCoroutine(ShowTrophies(showFirst, showSecond, showThird, o.GetRequiredScore(), score, parTime, playerTime));
        }

        private IEnumerator ShowTrophies(bool first, bool second, bool third, int requiredScore, int score, float parTime, float playerTime)
        {
            yield return new WaitForSeconds(1.0f);
            LocalizeFirst(first, true);
            yield return new WaitForSeconds(2.0f);
            LocalizeSecond(second, true, requiredScore, score);
            yield return new WaitForSeconds(2.0f);
            LocalizeThird(third, true, parTime, playerTime);
            levelEndReady = true;
            yield return null;
        }

        void LocalizeFirst(bool show, bool firstRun)
        {
            if (show)
            {
                trophies[0].DOColor(SUCCESS_COLOR, 1.0f);
                descriptionLocalizer.key = "success";
                descriptionLocalizer.Color = SUCCESS_COLOR;
            }
            else
            {
                //trophies[0].DOColor(FAILURE_COLOR, 1.0f);
                descriptionLocalizer.key = "failed";
                descriptionLocalizer.Color = FAILURE_COLOR;
            }
            descriptionLocalizer.Localize();
            titleLocalizer.key = "no_hits";
            titleLocalizer.Localize();
        }

        void LocalizeSecond(bool show, bool firstRun, int requiredScore, int playerScore)
        {
            if (show)
            {
                trophies[1].DOColor(SUCCESS_COLOR, 1.0f);
                descriptionLocalizer.key = "success";
                descriptionLocalizer.Color = SUCCESS_COLOR;
            }
            else
            {
                //trophies[1].DOColor(FAILURE_COLOR, 1.0f);
                descriptionLocalizer.key = "failed";
                descriptionLocalizer.Color = FAILURE_COLOR;
            }
            descriptionLocalizer.Localize();
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
            if (show)
            {
                trophies[2].DOColor(SUCCESS_COLOR, 1.0f);
                descriptionLocalizer.key = "success";
                descriptionLocalizer.Color = SUCCESS_COLOR;
            }
            else
            {
                //trophies[2].DOColor(FAILURE_COLOR, 1.0f);
                descriptionLocalizer.key = "failed";
                descriptionLocalizer.Color = FAILURE_COLOR;
            }
            descriptionLocalizer.Localize();
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
