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

    private GameObject selection;

    public TextMeshPro titleText;
    public GameObject[] characters = new GameObject[4];

    private Dictionary<int, Dictionary<string, Transform>> charTransforms = new Dictionary<int, Dictionary<string, Transform>>();

    private void Awake()
    {
        selection = GameObject.Find("CharacterSelection");
    }

    void Start()
    {
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

        checkSelectedCharacter();
    }

    // Update is called once per frame
    void Update()
    {
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
            checkSelectedCharacter();
            selectionChanged = false;
        }

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }

    private void checkSelectedCharacter()
    {
        for(int i=0; i<4; i++)
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

                selection.GetComponent<CharacterData>().SelectedCharacter = selectedCharacter;
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
