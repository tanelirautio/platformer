using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace pf {
    public class StatisticsMenu : MonoBehaviour
    {
        public GameObject stat;
        public GameObject container;
        public GameObject scrollArea;
        public GameObject back;

        private ScrollRect scrollrect;
        private RectTransform viewport;

        private List<Transform> stats = new List<Transform>();
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
            DataLoader.ParseData();

            float offset = 8f;
            int i = 0;

            foreach (Collectable.Type type in Enum.GetValues(typeof(Collectable.Type)))
            {
                if(type == Collectable.Type.Heart)
                {
                    //TODO: add hearts_collected to statistics
                    continue;
                }

                GameObject go = Instantiate(stat, new Vector3(0, 0, 0), Quaternion.identity);
                go.transform.SetParent(container.transform, false);
                go.name = stat.name + "_" + i;
                Vector3 pos = go.transform.position;
                pos.y = offset - i * 2f;
                go.transform.position = pos;

                TextLocalizerUI titleLocalizer = go.transform.Find("Title").GetComponent<TextLocalizerUI>();
                titleLocalizer.key = StatisticsManager.GetLocalizationKey(type);
                titleLocalizer.Localize();
      
                TextMeshProUGUI count = go.transform.Find("Count").GetComponent<TextMeshProUGUI>();
                count.text = StatisticsManager.GetCollectedFruits(type).ToString();

                stats.Add(go.transform);
                i++;
            }

            stats[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
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
                    stats[index].DOScale(1.0f, 1f);
                }
                else if (deltaY != 0)
                {
                    int prevIndex = index;

                    if (deltaY == 1)
                    {
                        if (index < stats.Count - 1)
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
                        stats[prevIndex].DOScale(1.0f, 1f);
                        stats[index].DOScale(1.1f, 1f);
                    }

                    if (!RendererExtensions.IsFullyVisibleFrom(stats[index].GetComponent<RectTransform>(), Camera.main))
                    {
                        Navigate(stats[index].GetComponent<RectTransform>());
                    }
                }
            }
            else if (selection == Selection.Back)
            {
                if (deltaX == -1)
                {
                    selection = Selection.Scroll;
                    back.GetComponent<SpriteRenderer>().color = Color.gray;
                    stats[index].DOScale(1.1f, 1f);
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