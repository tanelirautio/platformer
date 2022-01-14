using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using DG.Tweening;

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

        private Selection selection = 0;

        private DataLoader dataLoader;
        private Music music;
        private bool firstRun = true;

        private PlayerInputActions playerInputActions;

        private void OnNavigate(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            var delta = -(int)value.y; // -1 or 1, invert...
            if (delta == 0) return;

            selection += delta;
            if (selection == Selection.Load && !dataLoader.ShowLoadOption)
            {
                selection += delta;
            }

            // handle wrap
            if (selection < Selection.New)
            {
                selection = Selection.Quit;
            }
            else if (selection > Selection.Quit)
            {
                selection = Selection.New;
            }

            CheckSelection();
        }

        private void OnSubmit(InputAction.CallbackContext context)
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
                    levelLoader.LoadScene((int)LevelLoader.Scenes.Settings);
                    break;
                }
                case Selection.Achievements:
                {
                    print("Achievements!");
                    levelLoader.LoadScene((int)LevelLoader.Scenes.Achievements);
                    break;
                }
                case Selection.Statistics:
                {
                    print("Statistics!");
                    levelLoader.LoadScene((int)LevelLoader.Scenes.Statistics);
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

        private void Awake()
        {
            DOTween.Init();

            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            dataLoader = GetComponent<DataLoader>();
            music = GameObject.Find("AudioSystem").GetComponent<Music>();
            playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            playerInputActions.MenuControls.Navigate.performed += OnNavigate;
            playerInputActions.MenuControls.Navigate.Enable();

            playerInputActions.MenuControls.Submit.performed += OnSubmit;
            playerInputActions.MenuControls.Submit.Enable();
        }

        private void OnDisable()
        {
            playerInputActions.MenuControls.Navigate.Disable();
            playerInputActions.MenuControls.Navigate.performed -= OnNavigate;

            playerInputActions.MenuControls.Submit.Disable();
            playerInputActions.MenuControls.Submit.performed -= OnSubmit;
        }

        void Start()
        {
            titleText.outlineColor = Color.black;
            titleText.outlineWidth = 0.2f;

            if (!music.IsPlaying)
            {
                music.Play("Intro");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(firstRun)
            {
                CheckSelection();
                firstRun = false;
            }
        }

        private void CheckSelection()
        {

            for (int i = 0; i < (int)Selection.Count; i++)
            {
                if (i == (int)selection)
                {
                    menu[i].transform.DOScale(Defs.MENU_SELECTED_SCALE, 1f);
                    menu[i].color = Color.white;
                }
                else
                {
                    menu[i].transform.DOScale(Defs.MENU_NORMAL_SCALE, 1f);
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
