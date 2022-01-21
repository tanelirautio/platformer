using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;
using System;
using UnityEngine.EventSystems;

namespace pf
{
    public class SettingsMenu : MonoBehaviour
    {
        public GameObject container;
        public GameObject scrollArea;
        public GameObject back;

        public GameObject musicVolumeSliderPrefab;
        public GameObject soundVolumeSliderPrefab;
        public GameObject languageSelectPrefab;

        private AudioManager audioManager;
        private LanguageSelect language;

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

        private enum LockedSettings
        {
            NotLocked = -1,
            MusicVolume = 0,
            SoundVolume = 1,
            Language = 2
        }

        private PlayerInputActions playerInputActions;
        private Selection selection = 0;
        private LevelLoader levelLoader;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            playerInputActions = new PlayerInputActions();

            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();

            scrollrect = scrollArea.GetComponent<ScrollRect>();
            viewport = scrollArea.GetComponent<RectTransform>();
        }

        void Start()
        {
            float offset = 8f;
            int i = 0;

            GameObject musicVolumeSlider = Instantiate(musicVolumeSliderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            musicVolumeSlider.transform.SetParent(container.transform, false);
            Vector3 pos = musicVolumeSlider.transform.position;
            pos.y = offset;
            musicVolumeSlider.transform.position = pos;
            musicVolumeSlider.transform.Find("Slider").GetComponent<Slider>().value = PlayerStats.MusicVolume;
            settings.Add(musicVolumeSlider.transform);
            i++;

            GameObject soundVolumeSlider = Instantiate(soundVolumeSliderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            soundVolumeSlider.transform.SetParent(container.transform, false);
            pos = soundVolumeSlider.transform.position;
            pos.y = offset - 4f;
            soundVolumeSlider.transform.position = pos;
            soundVolumeSlider.transform.Find("Slider").GetComponent<Slider>().value = PlayerStats.SoundVolume;
            settings.Add(soundVolumeSlider.transform);
            i++;

            //GameObject soundVolumeSlider = Instantiate(soundVolumeSliderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject languageSelect = languageSelectPrefab;
            languageSelect.transform.SetParent(container.transform, false);
            pos = languageSelect.transform.position;
            pos.y = offset - 8.5f;
            languageSelect.transform.position = pos;
            settings.Add(languageSelect.transform);
            
            settings[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
            back.GetComponent<SpriteRenderer>().color = Color.gray;
            language = languageSelect.GetComponent<LanguageSelect>();
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
                if (deltaX == 1 && lockedIndex == (int)LockedSettings.NotLocked)
                {
                    selection = Selection.Back;
                    back.GetComponent<SpriteRenderer>().color = Color.white;
                    settings[index].DOScale(Defs.MENU_NORMAL_SCALE, 1f);
                }
                else if (deltaY != 0 && lockedIndex == (int)LockedSettings.NotLocked)
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
                else if(deltaX != 0 && lockedIndex != (int)LockedSettings.NotLocked)
                {
                    if(lockedIndex == (int)LockedSettings.MusicVolume || lockedIndex == (int)LockedSettings.SoundVolume)
                    {
                        HandleSliderMove(deltaX, lockedIndex);
                    }
                }
                else if(deltaY != 0 && lockedIndex != (int)LockedSettings.NotLocked)
                {
                    if(lockedIndex == (int)LockedSettings.Language)
                    {
                        print("Should change language selection here!");
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
            if(lockedIndex == (int)LockedSettings.MusicVolume || lockedIndex == (int)LockedSettings.SoundVolume)
            {
                Slider s = settings[lockedIndex].Find("Slider").GetComponent<Slider>();
                if(deltaX == 1)
                {
                    s.value -= 0.1f;
                    if(s.value < 0.001f)
                    {
                        s.value = 0.001f;
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

                if(lockedIndex == (int)LockedSettings.MusicVolume)
                {
                    audioManager.SetVolume(s.value, AudioManager.AudioChannel.Music);
                    PlayerStats.MusicVolume = s.value;
                }
                else
                {
                    audioManager.SetVolume(s.value, AudioManager.AudioChannel.fx);
                    PlayerStats.SoundVolume = s.value;
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
                //TODO: simplify this if-else mess... But it works for now at least :)
                if (index == 0)
                {
                    if (lockedIndex == (int)LockedSettings.NotLocked)
                    {
                        lockedIndex = (int)LockedSettings.MusicVolume;
                        settings[0].gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[0].transform.Find("Icons/SoundOn").GetComponent<Image>().color = new Color(0, 0, 0);
                        settings[0].transform.Find("Icons/SoundOff").GetComponent<Image>().color = new Color(0, 0, 0);
                        settings[0].transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0);
                    }
                    else
                    {
                        lockedIndex = (int)LockedSettings.NotLocked;
                        settings[0].gameObject.GetComponent<Image>().color = new Color(40f / 255f, 42f / 255f, 48f / 255f);
                        settings[0].transform.Find("Icons/SoundOn").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[0].transform.Find("Icons/SoundOff").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[0].transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1);
                        SaveSystem.Save();
                    }
                }
                else if (index == 1)
                {
                    if (lockedIndex == (int)LockedSettings.NotLocked)
                    {
                        lockedIndex = (int)LockedSettings.SoundVolume;
                        settings[1].gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[1].transform.Find("Icons/SoundOn").GetComponent<Image>().color = new Color(0, 0, 0);
                        settings[1].transform.Find("Icons/SoundOff").GetComponent<Image>().color = new Color(0, 0, 0);
                        settings[1].transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0);
                    }
                    else
                    {
                        lockedIndex = (int)LockedSettings.NotLocked;
                        settings[1].gameObject.GetComponent<Image>().color = new Color(40f / 255f, 42f / 255f, 48f / 255f);
                        settings[1].transform.Find("Icons/SoundOn").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[1].transform.Find("Icons/SoundOff").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[1].transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1);
                        SaveSystem.Save();
                    }
                }
                else if (index == 2)
                {
                    if (lockedIndex == (int)LockedSettings.NotLocked)
                    {
                        lockedIndex = (int)LockedSettings.Language;
                        language.SelectionMode(true);
                        settings[2].gameObject.GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        lockedIndex = (int)LockedSettings.NotLocked;
                        language.SelectionMode(false);
                        settings[2].gameObject.GetComponent<Image>().color = new Color(40f / 255f, 42f / 255f, 48f / 255f);
                    }
                }
            }
        }
    }
}