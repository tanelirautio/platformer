using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

namespace pf
{
    public class AchievementMenu : MonoBehaviour
    {

        public GameObject achievement;
        public GameObject container;
        public GameObject scrollArea;
        public GameObject back;

        private ScrollRect scrollrect;
        private RectTransform viewport;

        private List<Transform> achievements = new List<Transform>();
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

            float x = achievement.transform.position.x;
            float y = achievement.transform.position.y;
            float offset = 8f;

            for (int i = 0; i < PlayerStats.Achievements.Count; i++)
            {
                GameObject go = Instantiate(achievement, new Vector3(0, 0, 0), Quaternion.identity);
                go.transform.SetParent(container.transform, false);
                go.name = achievement.name + "_" + i;
                Vector3 pos = go.transform.position;
                pos.y = offset - i * 3f;
                go.transform.position = pos;

                TextMeshProUGUI title = go.transform.Find("AchieveTitle").GetComponent<TextMeshProUGUI>();
                title.text = PlayerStats.Achievements[i].title;

                TextMeshProUGUI desc = go.transform.Find("AchieveDesc").GetComponent<TextMeshProUGUI>();
                desc.text = PlayerStats.Achievements[i].desc;

                print("Setting position " + i + ": " + go.transform.position);

                //TODO grey out the achievement if not completed
                
                if(!PlayerStats.CompletedAchievements[i])
                {
                    go.GetComponent<Image>().color = new Color(100f/255f, 100f/255f, 100f/255f);
                    go.transform.Find("AchieveImgBg/AchieveImg").GetComponent<RawImage>().color = new Color(100f / 255f, 100f / 255f, 100f / 255f);

                    var titleText = go.transform.Find("AchieveTitle").GetComponent<TextMeshProUGUI>();
                    titleText.DOColor(new Color(150f / 255f, 150f / 255f, 150f / 255f), 0f);  
 
                    var descText = go.transform.Find("AchieveDesc").GetComponent<TextMeshProUGUI>();
                    descText.DOColor(new Color(150f / 255f, 150f / 255f, 150f / 255f), 0f);
                }
                

                achievements.Add(go.transform);
            }

            achievements[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
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

            if(selection == Selection.Scroll)
            {
                if (deltaX == 1)
                {
                    selection = Selection.Back;
                    back.GetComponent<SpriteRenderer>().color = Color.white;
                    achievements[index].DOScale(1.0f, 1f);
                }
                else if(deltaY != 0)
                {
                    int prevIndex = index;

                    if (deltaY == 1) {
                        if (index < achievements.Count-1)
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

                    if(prevIndex != index)
                    {
                        achievements[prevIndex].DOScale(1.0f, 1f);
                        achievements[index].DOScale(1.1f, 1f);
                    }

                    if(!RendererExtensions.IsFullyVisibleFrom(achievements[index].GetComponent<RectTransform>(), Camera.main))
                    {
                        Navigate(achievements[index].GetComponent<RectTransform>());                        
                    }
                }
            }
            else if(selection == Selection.Back)
            {
                if(deltaX == -1)
                {
                    selection = Selection.Scroll;
                    back.GetComponent<SpriteRenderer>().color = Color.gray;
                    achievements[index].DOScale(1.1f, 1f);
                }
            }
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if(selection == Selection.Back)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }
        }        
    }
}