using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    private int selectedCharacter = 0;
    private bool selectionChanged = false;
    private bool changingScene = false;

    public TextMeshPro titleText;
    public TextMeshPro nameText;
    public TextMeshPro descriptionText;
    public GameObject[] characters = new GameObject[4];

    private Dictionary<int, Dictionary<string, Transform>> charTransforms = new Dictionary<int, Dictionary<string, Transform>>();

    private string[] names = new string[4] { "Leo", "Viivi", "Venla", "Milja" };
    private string[] descriptions = new string[4]
    {
        "Ninja frog extraordinaire",
        "Pink & wild ",
        "Michiefs in small package",
        "Small and agile"
    };

    private UIController uiController;
    const float FADE_SPEED = 1.0f;

    private void Awake()
    {
        uiController = GameObject.Find("UICanvas").GetComponent<UIController>();
    }

    void Start()
    {
        changingScene = false;

        titleText.outlineColor = Color.black;
        titleText.outlineWidth = 0.2f;

        for (int i = 0; i < 4; i++)
        {
            Dictionary<string, Transform> transforms = new Dictionary<string, Transform>();
            GameObject c = characters[i];
            transforms.Add("active", c.transform.Find("selection_active"));
            transforms.Add("inactive", c.transform.Find("selection_inactive"));
            transforms.Add("character", c.transform.Find("character"));
            charTransforms.Add(i, transforms);
        }

        CheckSelectedCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if(changingScene)
        {
            return;
        }

        //quick hack to get something working, replace with real input manager
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedCharacter++;
            if(selectedCharacter > 3)
            {
                selectedCharacter = 0;
            }
            selectionChanged = true;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedCharacter--;
            if (selectedCharacter < 0)
            {
                selectedCharacter = 3;
            }
            selectionChanged = true;
        }

        if (selectionChanged)
        {
            CheckSelectedCharacter();
            selectionChanged = false;
        }

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            uiController.Fade(true, FADE_SPEED);
            Invoke("ChangeScene", FADE_SPEED * 2);
            changingScene = true;
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    private void CheckSelectedCharacter()
    {
        for(int i = 0; i < 4; i++)
        {
            var charData = charTransforms[i];
            if(i == selectedCharacter)
            {
                charData["active"].gameObject.SetActive(true);
                charData["inactive"].gameObject.SetActive(false);
                charData["character"].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                Animator animator = charData["character"].gameObject.GetComponent<Animator>();
                if (animator)
                {
                    animator.enabled = true;
                    animator.SetBool("grounded", true);
                }
                nameText.text = names[i];
                descriptionText.text = descriptions[i];

                PlayerStats.SelectedCharacter = selectedCharacter;
            }
            else
            {
                charData["active"].gameObject.SetActive(false);
                charData["inactive"].gameObject.SetActive(true);
                charData["character"].gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                Animator animator = charData["character"].gameObject.GetComponent<Animator>();
                if (animator)
                {
                    animator.enabled = false;
                }
            }
        }

        
    }
}
