using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

namespace pf
{

    public class Credits : MonoBehaviour
    {
        public float yOffsetStart = -13.0f;
        public float yOffsetEnd = 13.0f;

        public TextMeshPro creditsText;

        private Bounds bounds;
        private TextAsset credits;
        private LevelLoader levelLoader;
        private Music music;

        private PlayerInputActions playerInputActions;
        private InputAction submitAction;
        private InputAction cancelAction;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            music = GameObject.Find("AudioSystem").GetComponent<Music>();
            playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            submitAction = playerInputActions.MenuControls.Submit;
            submitAction.Enable();            
            
            cancelAction = playerInputActions.MenuControls.Cancel;
            cancelAction.Enable();
        }

        private void OnDisable()
        {
            submitAction.Disable();
            cancelAction.Disable();
        }

        void Start()
        {
            music.Play(LevelLoader.GetCurrentSceneName());

            string filename;
            switch (PlayerStats.Language)
            {

                case (int)LocalizationManager.Language.Finnish:
                {
                    filename = "credits_fin";
                    break;
                }
                case (int)LocalizationManager.Language.English:
                default:
                {
                    filename = "credits_eng";
                    break;
                }


            }
            credits = Resources.Load(filename) as TextAsset;
            print(credits);

            creditsText.text = credits.text;

            creditsText.ForceMeshUpdate();
            bounds = creditsText.textBounds;
            print(bounds.size);

            creditsText.transform.position = new Vector3(0, yOffsetStart - bounds.size.y/2, 0);

            creditsText.transform.DOMoveY(yOffsetEnd + bounds.size.y / 2, Defs.CREDITS_TIME).SetEase(Ease.Linear).OnComplete(CreditsDone);

        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }

        void CreditsDone()
        {
            LoadNextScene();
        }

        void Update()
        {
            if (submitAction.WasPerformedThisFrame() || cancelAction.WasPerformedThisFrame())
            {
                LoadNextScene();
            }
        }

        private void LoadNextScene()
        {
            music.Stop();
            levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
        }
    }
}
