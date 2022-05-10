using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;
using System;

namespace pf
{
    public class PauseGame : MonoBehaviour
    {
        public enum Selection
        {
            Continue,
            Exit,
            Count
        }

        private Image fadeImage;
        private GameObject pauseBase;

        private TextMeshProUGUI cont;
        private TextMeshProUGUI exit;

        private PlayerInputActions playerInputActions;
        private Selection selection;

        private Sequence selectionSequence = null;
        private LevelLoader levelLoader;

        public bool Paused { get; private set; }
        public bool ContinuedFromPause { get; set; }

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            playerInputActions = new PlayerInputActions();

            fadeImage = transform.Find("Fade").GetComponentInChildren<Image>();
            pauseBase = transform.Find("PauseBase").gameObject;

            cont = pauseBase.transform.Find("Continue").GetComponent<TextMeshProUGUI>();
            exit = pauseBase.transform.Find("Exit").GetComponent<TextMeshProUGUI>();

            selectionSequence = DOTween.Sequence();
        }

        void Start()
        {
            Reset();
            CheckSelection();
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

        private void OnNavigate(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            var delta = -(int)value.y; // -1 or 1, invert...
            if (delta == 0) return;

            selection += delta;

            // handle wrap
            if (selection < Selection.Continue)
            {
                selection = Selection.Exit;
            }
            else if (selection > Selection.Exit)
            {
                selection = Selection.Continue;
            }

            CheckSelection();
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            switch (selection)
            {
                case Selection.Continue:
                    Time.timeScale = 1;
                    ContinuedFromPause = true;
                    HidePause();
                    break;
                case Selection.Exit:
                    Time.timeScale = 1;
                    levelLoader.LoadScene((int)LevelLoader.Scenes.LevelSelect);
                    break;
            }
        }

        private void CheckSelection()
        {
            if(selection == Selection.Continue)
            {
                SetSelectionActive(cont.transform);
                SetSelectionInactive(exit.transform);
                cont.color = Color.white;
                exit.color = Color.gray;
            }
            else
            {
                SetSelectionActive(exit.transform);
                SetSelectionInactive(cont.transform);
                cont.color = Color.gray;
                exit.color = Color.white;
            }
        }

        private void OnDestroy()
        {
            print("Destroy all");
            DOTween.KillAll();
        }

        private void Reset()
        {
            fadeImage.DOFade(0, 0);
            pauseBase.transform.localScale = Vector3.zero;
            pauseBase.SetActive(false);
        }

        public void ShowPause()
        {
            Paused = true;
            FadeBackground();
            ShowPauseBase();
        }

        private void FadeBackground()
        {
            fadeImage.DOFade(0.8f, 1.0f).SetUpdate(true);
        }

        private void ShowPauseBase()
        {
            pauseBase.SetActive(true);
            pauseBase.transform.DOScale(Defs.MENU_NORMAL_SCALE, 1.0f).SetUpdate(true);
        }

        private void HidePause()
        {
            Reset();
            Paused = false;
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
                .SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }

        private void SetSelectionInactive(Transform t)
        {
            t.DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_SCALE_SPEED).SetUpdate(true);
        }
    }
}
