using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace pf
{
    public class CharacterMenu : MonoBehaviour
    {
        private int selectedCharacter = 0;

        public TextMeshPro titleText;
        public TextMeshPro nameText;
        public TextMeshPro descriptionText;
        public GameObject[] characters = new GameObject[4];
        public GameObject back;

        private Dictionary<int, Dictionary<string, Transform>> charTransforms = new Dictionary<int, Dictionary<string, Transform>>();

        private string[] names = new string[4] { "Leo", "Viivi", "Venla", "Milja" };
        private string[] descriptions = new string[4]
        {
            "leo_desc",
            "viivi_desc",
            "venla_desc",
            "milja_desc"
        };

        private LevelLoader levelLoader;
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

            selectedCharacter += delta;
            if (selectedCharacter > 3) {
                selectedCharacter = -1;
            } else if (selectedCharacter < -1) {
                selectedCharacter = 3;
            }

            CheckSelectedCharacter();
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (selectedCharacter == -1)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }
            else
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.LevelSelect);
            }
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

        private void CheckBackButton()
        {
            if(selectedCharacter == -1)
            {
                back.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                back.GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }

        private void CheckSelectedCharacter()
        {
            CheckBackButton();
            for (int i = 0; i < 4; i++)
            {
                var charData = charTransforms[i];
                if (i == selectedCharacter)
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

                    string localizedDesc = LocalizationManager.GetLocalizedValue(descriptions[i]);
                    descriptionText.text = localizedDesc;

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
}
