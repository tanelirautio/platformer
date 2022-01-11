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
        public GameObject achievementContainer;
        public GameObject back;
        public GameObject scrollbar;
        public ScrollRect scrollrect;

        private List<Transform> achievements = new List<Transform>();
        private int index = 0;
        private bool lerping = false;

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
                go.transform.SetParent(achievementContainer.transform, false);
                go.name = achievement.name + "_" + i;
                Vector3 pos = go.transform.position;
                pos.y = offset - i * 3f;
                go.transform.position = pos;

                TextMeshProUGUI title = go.transform.Find("AchieveTitle").GetComponent<TextMeshProUGUI>();
                title.text = PlayerStats.Achievements[i].title;

                TextMeshProUGUI desc = go.transform.Find("AchieveDesc").GetComponent<TextMeshProUGUI>();
                desc.text = PlayerStats.Achievements[i].desc;

                //print(title.text + ": " + desc.text);
                achievements.Add(go.transform);
            }

            achievements[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
            back.GetComponent<SpriteRenderer>().color = Color.gray;
        }
 
        public void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();

            achievementContainer.GetComponent<RectTransform>().anchoredPosition =
                (Vector2)scrollrect.transform.InverseTransformPoint(achievementContainer.GetComponent<RectTransform>().position)
                - (Vector2)scrollrect.transform.InverseTransformPoint(target.position);
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
                }
                else if(deltaY != 0)
                {
                    //TODO actual scrolling, add some tweening?
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
                        SnapTo(achievements[index].GetComponent<RectTransform>());
                        //scrollrect.content.localPosition = scrollrect.GetSnapToPositionToBringChildIntoView(achievements[index].GetComponent<RectTransform>());
                    }
                }
            }
            else if(selection == Selection.Back)
            {
                if(deltaX == -1)
                {
                    selection = Selection.Scroll;
                    back.GetComponent<SpriteRenderer>().color = Color.gray;
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