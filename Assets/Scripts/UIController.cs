using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject blackOutSquare;

    private Image img;

    private void Awake()
    {
        blackOutSquare.SetActive(true);

        img = blackOutSquare.GetComponent<Image>();
        Color c = img.color;
        c = new Color(c.r, c.g, c.b, 0);
        blackOutSquare.GetComponent<Image>().color = c;
    }

    void Update()
    {
        // TODO Just for testing, remove eventually
        if(Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(FadeBlackOutSquare());
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(FadeBlackOutSquare(false));
        }
    }

    public void Fade(bool fadeToBlack = true, float fadeSpeed = 1.0f)
    {
        StartCoroutine(FadeBlackOutSquare(fadeToBlack, fadeSpeed));
    }


    private IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, float fadeSpeed = 1.0f)
    {
        Color objectColor = img.color;
        float fadeAmount;
        int step = 0;

        if (fadeToBlack)
        {
            while (img.color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                
                if(step >= 60)
                {
                    step = 0;
                }
                if (step == 0)
                {
                    blackOutSquare.GetComponent<Image>().color = objectColor;
                }
                step++;

                yield return null;
            }
        }
        else
        {
            while (img.color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);

                if (step >= 60)
                {
                    step = 0;
                }
                if (step == 0)
                {
                    blackOutSquare.GetComponent<Image>().color = objectColor;
                }
                step++;
                yield return null;
            }
        }
    }
}