using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace pf
{
    public class ContinueMenu : MonoBehaviour
    {
        private LevelLoader levelLoader;

        enum Selection
        {
            Yes,
            No
        }

        private Selection selection = Selection.Yes;
        private GameObject yesImage;
        private GameObject noImage;
        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            playerInputActions = new PlayerInputActions();
        }

        private void OnNavigate(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            var delta = (int)value.x;
            if (delta == 0) return;

            var left = (delta == -1);
            var right = (delta == 1);

            if (right && selection == Selection.Yes)
            {
                selection = Selection.No;
                noImage.SetActive(true);
                yesImage.SetActive(false);
            }
            else if (left && selection == Selection.No)
            {
                selection = Selection.Yes;
                noImage.SetActive(false);
                yesImage.SetActive(true);
            }
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (selection == Selection.Yes)
            {
                levelLoader.LoadScene(PlayerStats.SceneIndex);
            }
            else
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }
        }

        private void OnEnable()
        {
            playerInputActions.MenuControls.Submit.performed += OnSubmit;
            playerInputActions.MenuControls.Submit.Enable();

            playerInputActions.MenuControls.Navigate.performed += OnNavigate;
            playerInputActions.MenuControls.Navigate.Enable();
        }

        private void OnDisable()
        {
            playerInputActions.MenuControls.Submit.Disable();
            playerInputActions.MenuControls.Submit.performed -= OnSubmit;

            playerInputActions.MenuControls.Navigate.Disable();
            playerInputActions.MenuControls.Navigate.performed -= OnNavigate;
        }

        private void Start()
        {
            yesImage = GameObject.Find("UICanvas/YesImage");
            noImage = GameObject.Find("UICanvas/NoImage");
            noImage.SetActive(false);
        }

        void Update()
        {
        }

    }
}
