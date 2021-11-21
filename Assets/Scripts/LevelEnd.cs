using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelEnd : MonoBehaviour
{
    private Image fadeImage;
    private GameObject trophyBase;
    private Image[] trophies = new Image[3];

    private void Awake()
    {
        fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
        trophyBase = transform.Find("TrophyBase").gameObject;
        trophies[0] = transform.Find("TrophyBase/Trophy0").GetComponent<Image>();
        trophies[1] = transform.Find("TrophyBase/Trophy1").GetComponent<Image>();
        trophies[2] = transform.Find("TrophyBase/Trophy2").GetComponent<Image>();
    }

    public void Start()
    {
        Color c = fadeImage.color;
        c.a = 0;
        fadeImage.color = c;

        trophyBase.transform.localScale = Vector3.zero;

        for(int i=0; i<trophies.Length; i++)
        {
            trophies[i].color = new Color(0.1f,0.1f,0.1f,1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeBackground();
            ShowTrophyBase();
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            Start();
        } 
    }
    
    public void FadeBackground()
    {
        fadeImage.DOFade(0.8f, 1.0f);
    }

    public void ShowTrophyBase()
    {
        trophyBase.transform.DOScale(Vector3.one, 1.0f);
    }

    public void ShowTrophies()
    {
        // TODO: check from save if player has accomplished any trophies
        // these will be shown always

        // Then check if player has accomplished level objectives and
        // show the trophies that are granted to player 


    }
}
