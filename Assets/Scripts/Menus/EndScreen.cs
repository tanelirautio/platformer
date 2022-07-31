using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

namespace pf
{

    public class EndScreen : MonoBehaviour
    {
        private LevelLoader levelLoader;
        private Music music;
        private float timer = 0;

        public GameObject pressAnyKeyText;
        private TextMeshProUGUI anyKeyTMPro;

        private PlayerInputActions playerInputActions;

        bool startFadeSequence = false;
        Sequence fadeSequence = null;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            music = GameObject.Find("AudioSystem").GetComponent<Music>();
            playerInputActions = new PlayerInputActions();
        }

        void Start()
        {
            if (music != null)
            {
                music.Play(LevelLoader.GetCurrentSceneName());
            }

            anyKeyTMPro = pressAnyKeyText.GetComponent<TextMeshProUGUI>();
            anyKeyTMPro.DOFade(0, 0);
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer > Defs.END_SCREEN_WAIT_TIME)
            {
                if (!startFadeSequence)
                {
                    fadeSequence = DOTween.Sequence().Append(anyKeyTMPro.DOFade(1, 1)).SetLoops(-1, LoopType.Yoyo);
                    startFadeSequence = true;
                }

                if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    LoadNextScene();
                }

                if(Gamepad.current != null && (Gamepad.current.aButton.wasPressedThisFrame || Gamepad.current.bButton.wasPressedThisFrame)) {
                    LoadNextScene();
                }

                if(Joystick.current != null && Joystick.current.trigger.wasPressedThisFrame)
                {
                    LoadNextScene();
                }
            }
        }

        private void LoadNextScene()
        {
            music.Stop();
            levelLoader.LoadScene((int)LevelLoader.Scenes.Credits);
        }

        private void OnEnable()
        {
            playerInputActions.MenuControls.Submit.performed += OnSubmit;
            playerInputActions.MenuControls.Submit.Enable();
        }

        private void OnDisable()
        {
            playerInputActions.MenuControls.Submit.Disable();
            playerInputActions.MenuControls.Submit.performed -= OnSubmit;
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (timer > Defs.END_SCREEN_WAIT_TIME)
            {
                LoadNextScene();
            }
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }
    }
}
