using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class LevelEnd : MonoBehaviour
{
    private Image fadeImage;
    private GameObject trophyBase;
    private Image[] trophies = new Image[3];
    private TextMeshProUGUI levelCompleteText;
    private TextLocalizerUI descriptionLocalizer;

    private PlayerScore score;

    private void Awake()
    {
        fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
        levelCompleteText = transform.Find("LevelCompleteText").GetComponent<TextMeshProUGUI>();
        trophyBase = transform.Find("TrophyBase").gameObject;
        trophies[0] = transform.Find("TrophyBase/Trophy0").GetComponent<Image>();
        trophies[1] = transform.Find("TrophyBase/Trophy1").GetComponent<Image>();
        trophies[2] = transform.Find("TrophyBase/Trophy2").GetComponent<Image>();
        descriptionLocalizer = transform.Find("TrophyBase/Description").GetComponent<TextLocalizerUI>();

        score = GameObject.Find("Player").GetComponent<PlayerScore>();
    }

    public void Start()
    {
        fadeImage.DOFade(0, 0);

        trophyBase.transform.localScale = Vector3.zero;

        for(int i=0; i<trophies.Length; i++)
        {
            trophies[i].color = new Color(0.2f,0.2f,0.2f,1.0f);
        }

        levelCompleteText.transform.localScale = Vector3.zero;
        levelCompleteText.DOFade(0, 0);

        descriptionLocalizer.key = "empty";
        descriptionLocalizer.Localize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeBackground();
            ShowTrophyBase();
            ShowLevelCompleteText();
            CheckTrophies();
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            Start();
        } 
    }

    private void ShowLevelCompleteText()
    {
        levelCompleteText.transform.DOScale(Vector3.one, 1.0f);
        levelCompleteText.DOFade(1, 1.0f);
    }

    public void FadeBackground()
    {
        fadeImage.DOFade(1.0f, 1.0f);
    }

    public void ShowTrophyBase()
    {
        trophyBase.transform.DOScale(Vector3.one, 1.0f);
    }

    public void CheckTrophies()
    {
        // If scene is played directly in the editor, data might not have been parsed
        DataLoader.ParseData();

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
        if (level >= 0) {
            LevelObjectives o = PlayerStats.CompletedObjectives[level];
            if(o.CompletedNoHits) // TODO: track player deaths in level
            {
                showFirst = true;
                if(!o.CompletedNoHits)
                {
                    o.CompletedNoHits = true;
                }
            }
            if(o.CompletedPoints || score.GetScore() >= o.GetRequiredScore())
            {
                showSecond = true;
                if (!o.CompletedPoints)
                {
                    o.CompletedPoints = true;
                }
            }
            if(o.CompletedTime) //TODO: implement timer
            {
                showThird = true;
                if(!o.CompletedTime)
                {
                    o.CompletedTime = true;
                }
            }
        }

        showFirst = true;
        showSecond = true;
        showThird = true;
        StartCoroutine(ShowTrophies(showFirst, showSecond, showThird));

    }

    IEnumerator ShowTrophies(bool first, bool second, bool third)
    {
        yield return new WaitForSeconds(1.0f);

        if(first)
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

        if(second)
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

        //TODO: show par time and player time
        yield return null;
    }
}
