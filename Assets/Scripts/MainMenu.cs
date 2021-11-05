using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{
    public enum Selection
    {
        New,
        Load,
        Options,
        Achievements,
        Statistics,
        Quit,
        Count
    };

    public TextMeshPro titleText;
    public TextMeshPro[] menu = new TextMeshPro[6];

    private LevelLoader levelLoader;

    private bool selectionChanged = false;
    private Selection selection = 0;

    private void Awake()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    void Start()
    {
        titleText.outlineColor = Color.black;
        titleText.outlineWidth = 0.2f;
        CheckSelection();
    }

    // Update is called once per frame
    void Update()
    {
        //quick hack to get something working, replace with real input manager
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selection++;
            selectionChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selection--;
            selectionChanged = true;
        }

        if(selection < Selection.New)
        {
            selection = Selection.Quit;
        }
        else if(selection > Selection.Quit)
        {
            selection = Selection.New;
        }

        if(selectionChanged)
        {
            CheckSelection();
            selectionChanged = false;
        }

        // TODO:
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            switch(selection)
            {
                case Selection.New:
                {
                    levelLoader.LoadScene((int)LevelLoader.Scenes.CharacterSelect);
                    break;
                }                
                case Selection.Load:
                {
                    print("Load game!");
                    break;
                }
                case Selection.Options:
                {
                    print("Options!");
                    break;
                }
                case Selection.Achievements:
                {
                    print("Achievements!");
                    break;
                }
                case Selection.Statistics:
                {
                    print("Statistics!");
                    break;
                }
                case Selection.Quit:
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                        break;
                }

            }

        }
    }

    private void CheckSelection()
    {

        for(int i = 0; i < (int)Selection.Count; i++)
        {
            if(i == (int)selection)
            {
                menu[i].color = Color.white;
            }
            else
            {
                menu[i].color = Color.gray;
            }
        }
    }
}
