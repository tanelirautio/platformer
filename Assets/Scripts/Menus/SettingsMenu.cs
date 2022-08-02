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

        private Sequence selectionSequence = null;

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

            selectionSequence = DOTween.Sequence();
        }

        void Start()
        {
           GameObject musicVolumeSlider = musicVolumeSliderPrefab;
            musicVolumeSlider.transform.Find("Slider").GetComponent<Slider>().value = PlayerStats.MusicVolume;
            settings.Add(musicVolumeSlider.transform);

            GameObject soundVolumeSlider = soundVolumeSliderPrefab;
            soundVolumeSlider.transform.Find("Slider").GetComponent<Slider>().value = PlayerStats.SoundVolume;
            settings.Add(soundVolumeSlider.transform);

            GameObject languageSelect = languageSelectPrefab;
            settings.Add(languageSelect.transform);

            SetSelectionActive(settings[0]);
            SetSelectionInactive(settings[1]);
            SetSelectionInactive(settings[2]);

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
                    SetSelectionInactive(settings[index]);
                    //settings[index].DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_SCALE_SPEED);
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
                        //settings[prevIndex].DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_SCALE_SPEED);
                        //settings[index].DOScale(Defs.MENU_SELECTED_SCALE, Defs.MENU_SCALE_SPEED);
                        SetSelectionInactive(settings[prevIndex]);
                        SetSelectionActive(settings[index]);
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
                        //print("Should change language selection here!");
                        ChangeLanguageSelection(deltaY, lockedIndex);
                    }
                }
            }
            else if (selection == Selection.Back)
            {
                if (deltaX == -1)
                {
                    selection = Selection.Scroll;
                    back.GetComponent<SpriteRenderer>().color = Color.gray;
                    //settings[index].DOScale(Defs.MENU_SELECTED_SCALE, Defs.MENU_SCALE_SPEED);
                    SetSelectionActive(settings[index]);
                }
            }
        }

        private void ChangeLanguageSelection(int deltaY, int lockedIndex)
        {
            if(lockedIndex == (int)LockedSettings.Language)
            {
                int selectedLanguage = language.GetSelectedLanguage();
                if (deltaY == 1)
                {
                    //print("down");
                    language.ChangeLanguage(selectedLanguage + 1);
                }
                else
                {
                    //print("up");
                    language.ChangeLanguage(selectedLanguage - 1);
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
                        SetSelectionSelected(settings[0]);
                    }
                    else
                    {
                        lockedIndex = (int)LockedSettings.NotLocked;
                        settings[0].gameObject.GetComponent<Image>().color = new Color(40f / 255f, 42f / 255f, 48f / 255f);
                        settings[0].transform.Find("Icons/SoundOn").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[0].transform.Find("Icons/SoundOff").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[0].transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1);
                        SaveSystem.Save();
                        settings[0].transform.DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_SCALE_SPEED).OnComplete(()=>SetSelectionActive(settings[0]));
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
                        SetSelectionSelected(settings[1]);
                    }
                    else
                    {
                        lockedIndex = (int)LockedSettings.NotLocked;
                        settings[1].gameObject.GetComponent<Image>().color = new Color(40f / 255f, 42f / 255f, 48f / 255f);
                        settings[1].transform.Find("Icons/SoundOn").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[1].transform.Find("Icons/SoundOff").GetComponent<Image>().color = new Color(1, 1, 1);
                        settings[1].transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1);
                        SaveSystem.Save();
                        settings[1].transform.DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_SCALE_SPEED).OnComplete(() => SetSelectionActive(settings[1]));
                    }
                }
                else if (index == 2)
                {
                    if (lockedIndex == (int)LockedSettings.NotLocked)
                    {
                        lockedIndex = (int)LockedSettings.Language;
                        language.SelectionMode(true);
                        settings[2].gameObject.GetComponent<Image>().color = Color.white;
                        SetSelectionSelected(settings[2]);
                    }
                    else
                    {
                        lockedIndex = (int)LockedSettings.NotLocked;
                        PlayerStats.Language = language.GetSelectedLanguage();
                        language.SelectionMode(false);
                        settings[2].gameObject.GetComponent<Image>().color = new Color(40f / 255f, 42f / 255f, 48f / 255f);
                        SaveSystem.Save();

                        var texts = FindObjectsOfType<TextLocalizerUI>();
                        foreach(var text in texts)
                        {
                            text.Localize();
                        }
                        settings[2].transform.DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_SCALE_SPEED).OnComplete(() => SetSelectionActive(settings[2]));
                    }
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

        private void SetSelectionSelected(Transform t)
        {
            if (selectionSequence != null)
            {
                selectionSequence.Kill();
                selectionSequence = null;
            }
            t.DOScale(Defs.MENU_SELECTED_SCALE, Defs.MENU_SCALE_SPEED);
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
        }
    }
}