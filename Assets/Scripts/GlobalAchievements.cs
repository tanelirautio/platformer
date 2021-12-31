using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalAchievements : MonoBehaviour
{
    // General variables
    public GameObject achNote;
    public AudioSource achSound;
    public bool achActive = false;
    public GameObject achTitle;
    public GameObject achDesc;

    // Achievent 01 Specific
    public GameObject ach01Image;
    public static int ach01Count;
    public int ach01Trigger = 5;
    public int ach01Code;

    void Update()
    {
        ach01Code = PlayerPrefs.GetInt("Ach01");
        if(ach01Count == ach01Trigger && ach01Code != 12345)
        {
            StartCoroutine(Trigger01Ach());
        }
    }

    IEnumerator Trigger01Ach()
    {
        achActive = true;
        ach01Code = 12345;
        PlayerPrefs.SetInt("Ach01", ach01Code);
        achSound.Play();
        ach01Image.SetActive(true);
        achTitle.GetComponent<TextMeshPro>().text = "FOOBAR!";
        achDesc.GetComponent<TextMeshPro>().text = "Created a collection based achievement";
        achNote.SetActive(true);
        yield return new WaitForSeconds(7);

        // Resetting UI
        achNote.SetActive(false);
        ach01Image.SetActive(false);
        achTitle.GetComponent<TextMeshPro>().text = "";
        achDesc.GetComponent<TextMeshPro>().text = "";
        achActive = false;
    }
}
