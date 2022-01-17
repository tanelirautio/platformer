using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;


/*using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class ExampleClass : MonoBehaviour, IPointerDownHandler// required interface when using the OnPointerDown method.
{
    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown (PointerEventData eventData) 
    {
        Debug.Log (this.gameObject.name + " Was Clicked.");
    }
}*/

namespace pf
{
    public class SettingsMenu : MonoBehaviour //, IPointerDownHandler, IMoveHandler, IEndDragHandler
    {
        public GameObject container;
        public GameObject scrollArea;
        public GameObject back;

        public GameObject musicVolumeSlider;
        public GameObject soundVolumeSlider;

        private ScrollRect scrollrect;
        private RectTransform viewport;

        private List<Transform> settings = new List<Transform>();
        private int index = 0;
        private int lockedIndex = -1;

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
            float offset = 8f;
            int i = 0;

            musicVolumeSlider.transform.SetParent(container.transform, false);
            Vector3 pos = musicVolumeSlider.transform.position;
            pos.y = offset;
            musicVolumeSlider.transform.position = pos;
            musicVolumeSlider.transform.Find("Slider").GetComponent<Slider>().value = PlayerStats.MusicVolume;
            settings.Add(musicVolumeSlider.transform);
            i++;

            soundVolumeSlider.transform.SetParent(container.transform, false);
            pos = soundVolumeSlider.transform.position;
            pos.y = offset - i * 4f;
            soundVolumeSlider.transform.position = pos;
            musicVolumeSlider.transform.Find("Slider").GetComponent<Slider>().value = PlayerStats.SoundVolume;
            settings.Add(soundVolumeSlider.transform);
            i++;


            settings[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
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
                if (deltaX == 1 && lockedIndex == -1)
                {
                    selection = Selection.Back;
                    back.GetComponent<SpriteRenderer>().color = Color.white;
                    settings[index].DOScale(Defs.MENU_NORMAL_SCALE, 1f);
                }
                else if (deltaY != 0 && lockedIndex == -1)
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
                else if(deltaX != 0 && lockedIndex != -1)
                {
                    if(lockedIndex == 0 || lockedIndex == 1)
                    {
                        HandleSliderMove(deltaX, lockedIndex);
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

        private void HandleSliderMove(int deltaX, int lockedIndex)
        {
            if(lockedIndex == 0 || lockedIndex == 1)
            {
                Slider s = settings[lockedIndex].Find("Slider").GetComponent<Slider>();
                if(deltaX == 1)
                {
                    s.value -= 0.1f;
                    if(s.value < 0)
                    {
                        s.value = 0;
                    }
                }
                else
                {
                    s.value += 0.1f;
                    if(s.value > 1)
                    {
                        s.value = 1;
                    }
                }
            }
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (selection == Selection.Back)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }
            else
            {
                if(index == 0)
                {
                    if(lockedIndex == -1)
                    {
                        lockedIndex = 0;
                        settings[0].gameObject.GetComponent<Image>().color = new Color(1,0,0); //material.SetColor("_Color", Color.cyan);
                    }
                    else
                    {
                        lockedIndex = -1;
                        settings[0].gameObject.GetComponent<Image>().color = new Color(40f/255f, 42f/255f, 48f/255f); 
                    }
                }
                else if(index == 1)
                {
                    if (lockedIndex == -1)
                    {
                        lockedIndex = 1;
                        settings[1].gameObject.GetComponent<Image>().color = new Color(1, 0, 0); //material.SetColor("_Color", Color.cyan);
                    }
                    else
                    {
                        lockedIndex = -1;
                        settings[1].gameObject.GetComponent<Image>().color = new Color(40f / 255f, 42f / 255f, 48f / 255f);
                    }
                }
            }
        }
    }
}