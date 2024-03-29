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
            Play,
            Options,
            Achievements,
            Statistics,
            Credits,
            Quit,
            Count
        };

        public TextMeshPro titleText;
        public TextMeshPro[] menu = new TextMeshPro[6];
        
        private AudioManager audioManager;
        private LevelLoader levelLoader;

        private Selection selection = 0;

        private DataLoader dataLoader;
        private Music music;
        private bool firstRun = true;

        private PlayerInputActions playerInputActions;

        private Sequence selectionSequence = null;

        private void OnNavigate(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            var delta = -(int)value.y; // -1 or 1, invert...
            if (delta == 0) return;

            selection += delta;

            // handle wrap
            if (selection < Selection.Play)
            {
                selection = Selection.Quit;
            }
            else if (selection > Selection.Quit)
            {
                selection = Selection.Play;
            }

            CheckSelection();
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            switch (selection)
            {
                case Selection.Play:
                {
                    levelLoader.LoadScene((int)LevelLoader.Scenes.CharacterSelect);
                    break;
                }
                /*
                case Selection.Load:
                {
                    //TODO: Taneli 25.1.2022:
                    // If there's a level selection screen which allows playing completed levels (and defaults to next not completed level),
                    // there's no need for explicit "load game" selection in main menu at all. 
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
                */
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
            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();
            selectionSequence = DOTween.Sequence();

#if UNITY_EDITOR
            Cursor.visible = true;
#else
            Cursor.visible = false;
#endif
        }

        private void OnEnable()
        {
            playerInputActions.MenuControls.Navigate.performed += OnNavigate;
            playerInputActions.MenuControls.Navigate.Enable();

            playerInputActions.MenuControls.Submit.performed += OnSubmit;
            playerInputActions.MenuControls.Submit.Enable();

            playerInputActions.MenuControls.Cancel.performed += OnCancel;
            playerInputActions.MenuControls.Cancel.Enable();
        }

        private void OnDisable()
        {
            playerInputActions.MenuControls.Navigate.Disable();
            playerInputActions.MenuControls.Navigate.performed -= OnNavigate;

            playerInputActions.MenuControls.Submit.Disable();
            playerInputActions.MenuControls.Submit.performed -= OnSubmit;

            playerInputActions.MenuControls.Cancel.Disable();
            playerInputActions.MenuControls.Cancel.performed -= OnCancel;
        }

        void Start()
        {
            //titleText.outlineColor = Color.black;
            //titleText.outlineWidth = 0.2f;

            print("PlayerStats.MusicVolume: " + PlayerStats.MusicVolume);
            print("PlayerStats.SoundVolume: " + PlayerStats.SoundVolume);

            audioManager.SetVolume(PlayerStats.MusicVolume, AudioManager.AudioChannel.Music);
            audioManager.SetVolume(PlayerStats.SoundVolume, AudioManager.AudioChannel.fx);
            if (!music.IsPlaying)
            {
                music.Play(LevelLoader.GetCurrentSceneName());
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
                    SetSelectionActive(menu[i].transform);
                    menu[i].color = Color.white;
                }
                else
                {
                    SetSelectionInactive(menu[i].transform);
                    menu[i].color = Color.black;
                }
            }
        }

        private void SetSelectionActive(Transform t)
        {
            if (selectionSequence != null)
            {
                selectionSequence.Kill();
                selectionSequence = null;
            }
            selectionSequence = DOTween.Sequence()
                .Append(t.DOScale(Defs.MENU_SELECTED_SCALE, Defs.MENU_SCALE_SPEED))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void SetSelectionInactive(Transform t)
        {
            t.DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_SCALE_SPEED);
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }
    }
}
