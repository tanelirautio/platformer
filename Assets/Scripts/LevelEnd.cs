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
        private TextMeshProUGUI levelCompleteText;
        private TextLocalizerUI descriptionLocalizer;
        private TextLocalizerUI parTimeLocalizer;
        private TextLocalizerUI ownTimeLocalizer;

        private PlayerScore score;

        private bool levelEndReady = false;

        private void Awake()
        {
            fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
            trophyBase = transform.Find("TrophyBase").gameObject;
            trophies[0] = transform.Find("TrophyBase/Trophy0").GetComponent<Image>();
            trophies[1] = transform.Find("TrophyBase/Trophy1").GetComponent<Image>();
            trophies[2] = transform.Find("TrophyBase/Trophy2").GetComponent<Image>();
            descriptionLocalizer = transform.Find("TrophyBase/DescriptionText").GetComponent<TextLocalizerUI>();
            parTimeLocalizer = transform.Find("TrophyBase/ParTimeText").GetComponent<TextLocalizerUI>();
            ownTimeLocalizer = transform.Find("TrophyBase/OwnTimeText").GetComponent<TextLocalizerUI>();
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
            print("reset fadeImage");
            /*Color c1 = fadeImage.color;
            c1.a = 0;
            fadeImage.color = c1;*/
            fadeImage.DOFade(0, 0);

            trophyBase.transform.localScale = Vector3.zero;
            trophyBase.SetActive(false);
            for (int i = 0; i < trophies.Length; i++)
            {
                trophies[i].color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            }

            descriptionLocalizer.key = "empty";
            descriptionLocalizer.Localize();

            parTimeLocalizer.key = "empty";
            parTimeLocalizer.Localize();

            ownTimeLocalizer.key = "empty";
            ownTimeLocalizer.Localize();

            levelEndReady = false;
        }

        void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.F))
            {
                FadeBackground();
                ShowTrophyBase();
                ShowLevelCompleteText();
                CheckTrophies(false, 0, 0);
            }
            if(Input.GetKeyDown(KeyCode.G))
            {
                Reset();
            } 
            */
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
            fadeImage.DOFade(1.0f, 1.0f); //.OnComplete(ShowTrophyBase);     
        }
        private void ShowTrophyBase()
        {
            trophyBase.SetActive(true);
            trophyBase.transform.DOScale(1.0f, 1.0f);
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
            int level = PlayerStats.Level;
            print("level is: " + level);
            float parTime = 0;
            float playerTime = time;
            if (level >= 0 && level < PlayerStats.CompletedObjectives.Count)
            {
                LevelObjectives o = PlayerStats.CompletedObjectives[level];
                parTime = o.GetRequiredTime();
                if (o.CompletedNoHits || !hit)
                {
                    showFirst = true;
                    if (!o.CompletedNoHits)
                    {
                        o.CompletedNoHits = true;
                    }
                }
                if (o.CompletedPoints || score >= o.GetRequiredScore())
                {
                    showSecond = true;
                    if (!o.CompletedPoints)
                    {
                        o.CompletedPoints = true;
                    }
                }
                if (o.CompletedTime || time <= parTime)
                {
                    showThird = true;
                    if (!o.CompletedTime)
                    {
                        o.CompletedTime = true;
                    }
                }
            }

            StartCoroutine(ShowTrophies(showFirst, showSecond, showThird, parTime, playerTime));
        }

        private IEnumerator ShowTrophies(bool first, bool second, bool third, float parTime, float playerTime)
        {
            yield return new WaitForSeconds(1.0f);

            if (first)
            {
                trophies[0].DOColor(Color.white, 1.0f);
                descriptionLocalizer.key = "no_hits";
                descriptionLocalizer.Localize();
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield return null;
            }

            if (second)
            {
                trophies[1].DOColor(Color.white, 1.0f);
                descriptionLocalizer.key = "got_required_score";
                descriptionLocalizer.Localize();
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield return null;
            }

            if (third)
            {
                trophies[2].DOColor(Color.white, 1.0f);
                descriptionLocalizer.key = "time_under_par";
                descriptionLocalizer.Localize();
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield return null;
            }

            // TODO: some nice animation for timer(?)
            TimeSpan ts = TimeSpan.FromMilliseconds(parTime);
            parTimeLocalizer.key = "par_time";
            parTimeLocalizer.Localize();
            string parTimeStr = parTimeLocalizer.GetText() + " " + string.Format("{0:D2}:{1:D2}:{2:D3}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            parTimeLocalizer.SetText(parTimeStr);

            ts = TimeSpan.FromMilliseconds(playerTime);
            ownTimeLocalizer.key = "your_time";
            ownTimeLocalizer.Localize();
            string ownTimeStr = ownTimeLocalizer.GetText() + " " + string.Format("{0:D2}:{1:D2}:{2:D3}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            ownTimeLocalizer.SetText(ownTimeStr);

            levelEndReady = true;
            yield return null;
        }
    }
}
