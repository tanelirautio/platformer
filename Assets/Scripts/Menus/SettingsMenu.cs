using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace pf
{
    public class SettingsMenu : MonoBehaviour
    {
        public GameObject container;
        public GameObject scrollArea;
        public GameObject back;

        private ScrollRect scrollrect;
        private RectTransform viewport;

        private List<Transform> settings = new List<Transform>();
        private int index = 0;

        public enum Selection
        {
            Scroll,
            Back
        }

        private PlayerInputActions playerInputActions;
        private Selection selection = 0;
        private LevelLoader levelLoader;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            playerInputActions = new PlayerInputActions();

            scrollrect = scrollArea.GetComponent<ScrollRect>();
            viewport = scrollArea.GetComponent<RectTransform>();
        }

        void Start()
        {
            //settings[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
            back.GetComponent<SpriteRenderer>().color = Color.gray;
        }

        private void Navigate(RectTransform item)
        {
            Vector3 itemCurrentLocalPostion = scrollrect.GetComponent<RectTransform>().InverseTransformVector(ConvertLocalPosToWorldPos(item));
            Vector3 itemTargetLocalPos = scrollrect.GetComponent<RectTransform>().InverseTransformVector(ConvertLocalPosToWorldPos(viewport));

            Vector3 diff = itemTargetLocalPos - itemCurrentLocalPostion;
            diff.z = 0.0f;

            var newNormalizedPosition = new Vector2(
                diff.x / (container.GetComponent<RectTransform>().rect.width - viewport.rect.width),
                diff.y / (container.GetComponent<RectTransform>().rect.height - viewport.rect.height)
                );

            newNormalizedPosition = scrollrect.GetComponent<ScrollRect>().normalizedPosition - newNormalizedPosition;

            newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
            newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);

            DOTween.To(() => scrollrect.GetComponent<ScrollRect>().normalizedPosition, x => scrollrect.GetComponent<ScrollRect>().normalizedPosition = x, newNormalizedPosition, 0.8f);
        }

        private Vector3 ConvertLocalPosToWorldPos(RectTransform target)
        {
            var pivotOffset = new Vector3(
                (0.5f - target.pivot.x) * target.rect.size.x,
                (0.5f - target.pivot.y) * target.rect.size.y,
                0f);

            var localPosition = target.localPosition + pivotOffset;

            return target.parent.TransformPoint(localPosition);
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
            var deltaX = -(int)value.x;
            var deltaY = -(int)value.y; // -1 or 1, invert...
            if (deltaX == 0 && deltaY == 0) return;

            if (selection == Selection.Scroll)
            {
                if (deltaX == 1)
                {
                    selection = Selection.Back;
                    back.GetComponent<SpriteRenderer>().color = Color.white;
                    settings[index].DOScale(Defs.MENU_NORMAL_SCALE, 1f);
                }
                else if (deltaY != 0)
                {
                    int prevIndex = index;

                    if (deltaY == 1)
                    {
                        if (index < settings.Count - 1)
                        {
                            index++;
                        }
                    }
                    else
                    {
                        if (index > 0)
                        {
                            index--;
                        }
                    }

                    if (prevIndex != index)
                    {
                        settings[prevIndex].DOScale(Defs.MENU_NORMAL_SCALE, 1f);
                        settings[index].DOScale(Defs.MENU_SELECTED_SCALE, 1f);
                    }

                    if (!RendererExtensions.IsFullyVisibleFrom(settings[index].GetComponent<RectTransform>(), Camera.main))
                    {
                        Navigate(settings[index].GetComponent<RectTransform>());
                    }
                }
            }
            else if (selection == Selection.Back)
            {
                if (deltaX == -1)
                {
                    selection = Selection.Scroll;
                    back.GetComponent<SpriteRenderer>().color = Color.gray;
                    settings[index].DOScale(Defs.MENU_SELECTED_SCALE, 1f);
                }
            }
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (selection == Selection.Back)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }
        }
    }
}