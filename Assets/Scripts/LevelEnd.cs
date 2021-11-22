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

    private PlayerScore score;

    private void Awake()
    {
        fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
        trophyBase = transform.Find("TrophyBase").gameObject;
        trophies[0] = transform.Find("TrophyBase/Trophy0").GetComponent<Image>();
        trophies[1] = transform.Find("TrophyBase/Trophy1").GetComponent<Image>();
        trophies[2] = transform.Find("TrophyBase/Trophy2").GetComponent<Image>();
        levelCompleteText = transform.Find("LevelCompleteText").GetComponent<TextMeshProUGUI>();

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeBackground();
            ShowTrophyBase();
            ShowLevelCompleteText();
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

    public void ShowTrophies()
    {
        // TODO: check from save/playerdata if player has accomplished any trophies
        // these will be shown always
        bool showFirst = false;
        bool showSecond = false;
        bool showThird = false;

        // Then check if player has accomplished level objectives and
        // show the trophies that are granted to player 

        // Check from the saved data if trophies have been given in previous plays
        int level = PlayerStats.Level;
        if (level >= 0) {
            LevelObjectives o = PlayerStats.CompletedObjectives[level];
            if(o.CompletedNoDeaths) // TODO: track player deaths in level
            {
                showFirst = true;
                if(!o.CompletedNoDeaths)
                {
                    o.CompletedNoDeaths = true;
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

    }
}
