using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

namespace pf
{
    public class LevelSelectMenu : MonoBehaviour
    {
        public GameObject container;
        public GameObject scrollArea;
        public GameObject back;
        public GameObject levelPrefab;

        private ScrollRect scrollrect;
        private RectTransform viewport;

        public List<Transform> levelIconPositions = new List<Transform>();
        public List<Sprite> levelImages = new List<Sprite>();
        public List<TextMeshProUGUI> levelPackTitles = new List<TextMeshProUGUI>();

        private int lastCompletedLevel = -1;
        private List<Transform> levelSelectionObjects = new List<Transform>();

        private PlayerInputActions playerInputActions;
        private Selection selection = 0;
        private LevelLoader levelLoader;
        private Music music;

        private int index = 0;
        private bool isBottomRow = false; 

        public enum Selection
        {
            Scroll,
            Back
        }

        private List<string> levelPackNames = new List<string>()
        {
            "Emergency on Planet Earth",
            "The Return of the Space Cowboy",
            "Travelling Without Moving",
            "Synkronized"
        };

        private List<string> levelNames = new List<string>()
        {
            "When You Gonna Learn?",
            "Too Young To Die",
            "Hooked Up",
            "If I Like It, I Do It",
            "Music of the Mind",
            "Emergency on Planet Earth",
            "Whatever It Is, I Just Can't Stop",
            "Blow Your Mind",
            "Revolution 1993",
            "Didgin' Out",
            "Just Another Story",
            "Stillness In Time",
            "Half the Man",
            "Light Years",
            "Manifest Destiny",
            "The Kids",
            "Mr. Moon",
            "Scam",
            "Morning Glory",
            "Space Cowboy"
        };

        private enum ColorInfoKey
        {
            Open,
            Closed
        }

        private class ColorInfo
        {
            public ColorInfo(Color bgColor, Color trophyColor, Color textColor)
            {
                BgColor = bgColor;
                TrophyColor = trophyColor;
                TextColor = textColor;
            }
            public Color BgColor { get; private set; }
            public Color TrophyColor { get; private set; }
            public Color TextColor { get; private set; }
        }

        private Dictionary<ColorInfoKey, ColorInfo> colors = new Dictionary<ColorInfoKey, ColorInfo>() {
            { ColorInfoKey.Open, new ColorInfo(Color.white, new Color(1, 234f / 255f, 0), Color.white) },
            { ColorInfoKey.Closed, new ColorInfo(new Color(105f / 255f, 105f / 255f, 105f / 255f), new Color(48f / 255f, 48f / 255f, 48f / 255f), new Color(48f / 255f, 48f / 255f, 48f / 255f)) }
        };

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            playerInputActions = new PlayerInputActions();

            scrollrect = scrollArea.GetComponent<ScrollRect>();
            viewport = scrollArea.GetComponent<RectTransform>();
            music = GameObject.Find("AudioSystem").GetComponent<Music>();

#if UNITY_EDITOR
            // Try to load level objectives and achievements when played in Unity editor
            // This way independently played levels can still show them
            DataLoader.ParseData();
#endif
        }

        void Start()
        {
            for(int i=0; i < levelPackTitles.Count; i++)
            {
                levelPackTitles[i].text = levelPackNames[i];
            }

            for (int i = 0; i < levelIconPositions.Count; i++)
            {
                GameObject go = Instantiate(levelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                go.transform.SetParent(container.transform, false);
                go.name = levelPrefab.name + "_" + i;
                go.transform.position = levelIconPositions[i].position;

                LevelUtils utils = go.GetComponent<LevelUtils>();
                if(i < 10)
                {
                    utils.levelPack = 0;
                }
                else if(i < 20)
                {
                    utils.levelPack = 1;
                }


                if(i < levelImages.Count )
                {
                    go.GetComponent<Image>().sprite = levelImages[i];
                }

                TextMeshProUGUI name = go.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                if (i < levelNames.Count)
                {
                    name.text = levelNames[i];
                }

                Image trophy0 = go.transform.Find("TrophyBg/Trophy0").GetComponent<Image>();
                Image trophy1 = go.transform.Find("TrophyBg/Trophy1").GetComponent<Image>();
                Image trophy2 = go.transform.Find("TrophyBg/Trophy2").GetComponent<Image>();
                if (i < Defs.LEVEL_AMOUNT)
                {
                    if (PlayerStats.LevelsCompleted[i])
                    {
                        SetLevelOpen(go, i, trophy0, trophy1, trophy2, name);
                        lastCompletedLevel = i;
                    }
                    else
                    {
                        SetLevelClosed(go, trophy0, trophy1, trophy2, name);
                    }
                }
                else
                {
                    //Note: Code shouldn't go here ever after all the levels are finalized
                    SetLevelClosed(go, trophy0, trophy1, trophy2, name);
                }

                levelSelectionObjects.Add(go.transform);
            }

            print("LevelSelectionObjects.Count: " + levelSelectionObjects.Count);

            // Open the first not completed level to be played
            if (lastCompletedLevel < Defs.LEVEL_AMOUNT)
            {
                int i = lastCompletedLevel + 1;
                index = i;
                print("Opening index: " + i);

                if (i < levelSelectionObjects.Count)
                {
                    GameObject go = levelSelectionObjects[i].gameObject;
                    Image trophy0 = go.transform.Find("TrophyBg/Trophy0").GetComponent<Image>();
                    Image trophy1 = go.transform.Find("TrophyBg/Trophy1").GetComponent<Image>();
                    Image trophy2 = go.transform.Find("TrophyBg/Trophy2").GetComponent<Image>();
                    TextMeshProUGUI name = go.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                    SetLevelOpen(go, i, trophy0, trophy1, trophy2, name);

                    levelSelectionObjects[i].DOScale(Defs.MENU_SELECTED_SCALE, 0f);
                    levelSelectionObjects[i].Find("Selection").gameObject.SetActive(true);
                }
            }
            else
            {
                print("ALL LEVELS OPEN - just select the last one");
                levelSelectionObjects[Defs.LEVEL_AMOUNT - 1].DOScale(Defs.MENU_SELECTED_SCALE, 0f);
                levelSelectionObjects[Defs.LEVEL_AMOUNT - 1].Find("Selection").gameObject.SetActive(true);
                index = Defs.LEVEL_AMOUNT - 1;
            }

            back.GetComponent<SpriteRenderer>().color = Color.gray;
        }

        private void SetLevelOpen(GameObject go, int i, Image trophy0, Image trophy1, Image trophy2, TextMeshProUGUI name)
        {
            go.GetComponent<LevelUtils>().open = true;
            go.GetComponent<Image>().color = colors[ColorInfoKey.Open].BgColor;
            name.color = colors[ColorInfoKey.Open].TextColor;

            if (PlayerStats.CompletedObjectives[i].CompletedNoHits)
            {
                trophy0.color = colors[ColorInfoKey.Open].TrophyColor;
            }
            else
            {
                trophy0.color = colors[ColorInfoKey.Closed].TrophyColor;
            }

            if (PlayerStats.CompletedObjectives[i].CompletedPoints)
            {
                trophy1.color = colors[ColorInfoKey.Open].TrophyColor;
            }
            else
            {
                trophy1.color = colors[ColorInfoKey.Closed].TrophyColor;
            }

            if (PlayerStats.CompletedObjectives[i].CompletedTime)
            {
                trophy2.color = colors[ColorInfoKey.Open].TrophyColor;
            }
            else
            {
                trophy2.color = colors[ColorInfoKey.Closed].TrophyColor;
            }
        }

        private void SetLevelClosed(GameObject go, Image trophy0, Image trophy1, Image trophy2, TextMeshProUGUI name)
        {
            go.transform.GetComponent<Image>().color = colors[ColorInfoKey.Closed].BgColor;
            trophy0.color = colors[ColorInfoKey.Closed].TrophyColor;
            trophy1.color = colors[ColorInfoKey.Closed].TrophyColor;
            trophy2.color = colors[ColorInfoKey.Closed].TrophyColor;
            name.color = colors[ColorInfoKey.Closed].TextColor;
        }

        
        private void Navigate(Transform item)
        {
            LevelUtils levelUtils = item.gameObject.GetComponent<LevelUtils>();
            Vector2 newNormalizedPosition = Vector2.zero;
            switch(levelUtils.levelPack)
            {
                case 0:
                {
                    newNormalizedPosition = new Vector2(0, Mathf.Infinity);
                    break;
                }
                case 1:
                {
                    newNormalizedPosition = new Vector2(1, Mathf.Infinity);
                    break;
                }
            }
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
                int prevIndex = index;

                if (deltaX != 0)
                {
                    if (deltaX == 1)
                    {
                        if (index == 0 || index == 5)
                        {
                            selection = Selection.Back;
                            back.GetComponent<SpriteRenderer>().color = Color.white;
                            levelSelectionObjects[index].DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_LEVELSELECT_SCALE_SPEED);
                            levelSelectionObjects[index].Find("Selection").gameObject.SetActive(false);
                        }
                        else
                        {
                            if (index == 10 || index == 15)
                            {
                                index = index - 6;
                            }
                            else
                            {
                                index = index - 1;
                            }
                        }
                    }
                    else
                    {
                        if (index == 4 || index == 9)
                        {
                            index = index + 6;
                        }
                        else if (index == 14) {
                            //do nothing...
                        }
                        else
                        {
                            index = index + 1;
                        }
                    }
                }
                else if (deltaY != 0)
                {
                    if (deltaY == 1)
                    {
                        if (!isBottomRow)
                        {
                            index = index + 5;
                            isBottomRow = true;
                        }
                    }
                    else
                    {
                        if (isBottomRow)
                        {
                            index = index - 5;
                            isBottomRow = false;
                        }
                        else
                        {
                            selection = Selection.Back;
                            back.GetComponent<SpriteRenderer>().color = Color.white;
                            levelSelectionObjects[index].DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_LEVELSELECT_SCALE_SPEED);
                            levelSelectionObjects[index].Find("Selection").gameObject.SetActive(false);
                        }
                    }
                }

                if (selection == Selection.Scroll)
                {
                    index = Mathf.Clamp(index, 0, levelSelectionObjects.Count - 1);
                    print("ALRIGHTY THEN: index is: " + index);

                    levelSelectionObjects[prevIndex].DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_LEVELSELECT_SCALE_SPEED);
                    levelSelectionObjects[prevIndex].Find("Selection").gameObject.SetActive(false);

                    levelSelectionObjects[index].DOScale(Defs.MENU_SELECTED_SCALE, Defs.MENU_LEVELSELECT_SCALE_SPEED);
                    levelSelectionObjects[index].Find("Selection").gameObject.SetActive(true);
                }

                if (!RendererExtensions.IsFullyVisibleFrom(levelSelectionObjects[index].GetComponent<RectTransform>(), Camera.main))
                {
                    Navigate(levelSelectionObjects[index]);
                }
                
            }
            else if (selection == Selection.Back)
            {
                if (deltaX == -1 || deltaY == 1)
                {
                    selection = Selection.Scroll;
                    back.GetComponent<SpriteRenderer>().color = Color.gray;
                    levelSelectionObjects[index].DOScale(Defs.MENU_SELECTED_SCALE, Defs.MENU_LEVELSELECT_SCALE_SPEED);
                    levelSelectionObjects[index].Find("Selection").gameObject.SetActive(true);
                }
            }
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (selection == Selection.Back)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.CharacterSelect);
            }
            else
            {
                if(levelSelectionObjects[index].GetComponent<LevelUtils>().open)
                {
                    levelLoader.LoadScene((int)LevelLoader.Scenes.StartLevel + index);
                    music.StopFade(1f);
                }
                else
                {
                    print("level is not open!");
                }
            }
        }
    }
}