using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace pf
{
    public class MainMenu : MonoBehaviour
    {
        public enum Selection
        {
            New,
            Load,
            Options,
            Achievements,
            Statistics,
            Credits,
            Quit,
            Count
        };

        public TextMeshPro titleText;
        public TextMeshPro[] menu = new TextMeshPro[7];

        private LevelLoader levelLoader;

        private bool selectionChanged = false;
        private Selection selection = 0;

        private DataLoader dataLoader;
        private MenuMusic menuMusic;
        private bool firstRun = true;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            dataLoader = GetComponent<DataLoader>();
            menuMusic = GameObject.Find("MenuAudio").GetComponent<MenuMusic>();

            // We don't need GameAudio object in the menus
            GameObject gameAudio = GameObject.Find("GameAudio");
            if (gameAudio)
            {
                Destroy(gameAudio);
            }
        }

        void Start()
        {
            titleText.outlineColor = Color.black;
            titleText.outlineWidth = 0.2f;
            menuMusic.Play("Intro");
        }

        // Update is called once per frame
        void Update()
        {
            if(firstRun)
            {
                CheckSelection();
                firstRun = false;
            }

            //quick hack to get something working, replace with real input manager
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selection++;
                if (!dataLoader.ShowLoadOption && selection == Selection.Load)
                {
                    selection++;
                }
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selection--;
                if (!dataLoader.ShowLoadOption && selection == Selection.Load)
                {
                    selection--;
                }
                selectionChanged = true;
            }

            if (selection < Selection.New)
            {
                selection = Selection.Quit;
            }
            else if (selection > Selection.Quit)
            {
                selection = Selection.New;
            }

            if (selectionChanged)
            {
                CheckSelection();
                selectionChanged = false;
            }

            // TODO:
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                switch (selection)
                {
                    case Selection.New:
                        {
                            levelLoader.LoadScene((int)LevelLoader.Scenes.CharacterSelect);
                            break;
                        }
                    case Selection.Load:
                        {
                            print("Load game!");
                            if (dataLoader.GetSaveData() != null)
                            {
                                PlayerStats.SelectedCharacter = dataLoader.GetSaveData().selectedCharacter;
                                PlayerStats.SceneIndex = dataLoader.GetSaveData().currentLevel;
                                PlayerStats.Health = dataLoader.GetSaveData().health;
                                levelLoader.LoadScene(PlayerStats.SceneIndex);
                            }
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
                    case Selection.Credits:
                        {
                            levelLoader.LoadScene((int)LevelLoader.Scenes.Credits);
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

            for (int i = 0; i < (int)Selection.Count; i++)
            {
                if (i == (int)selection)
                {
                    menu[i].color = Color.white;
                }
                else
                {
                    //print("Setting index: " + i + " gray");
                    menu[i].color = Color.gray;
                }

                if (!dataLoader.ShowLoadOption)
                {
                    menu[(int)Selection.Load].color = new Color(0.2f, 0.2f, 0.2f, 1);
                }
            }
        }
    }
}
