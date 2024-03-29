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

        private Sequence selectionSequence = null; 

        public enum Selection
        {
            Scroll,
            Back
        }


        private string levelPackNameBase = "level_pack_title_";
        private string levelNameBase = "level_title_";


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

            selectionSequence = DOTween.Sequence();
        }

        void Start()
        {

            for(int i=0; i < Defs.LEVEL_PACKS; i++)
            {
                TextLocalizerUI loc = levelPackTitles[i].GetComponent<TextLocalizerUI>();
                loc.key = levelPackNameBase + i.ToString();
                loc.Localize();
                levelPackTitles[i].text = loc.GetText();
            }

            for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
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
                TextLocalizerUI loc = go.transform.Find("Name").GetComponent<TextLocalizerUI>();
                if (i < Defs.LEVEL_AMOUNT)
                {
                    loc.key = levelNameBase + i.ToString();
                    loc.Localize();
                    name.text = loc.GetText();
                }

                Image trophy0 = go.transform.Find("TrophyBg/Trophy0").GetComponent<Image>();
                Image trophy1 = go.transform.Find("TrophyBg/Trophy1").GetComponent<Image>();
                Image trophy2 = go.transform.Find("TrophyBg/Trophy2").GetComponent<Image>();
                if (i < Defs.LEVEL_AMOUNT)
                {
                    if(i == 0)
                    {
                        //First level is always open, no matter what
                        SetLevelOpen(go, i, trophy0, trophy1, trophy2, name);
                    }
                    else if (PlayerStats.LevelsCompleted[i])
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


            // Open the first not completed level to be played
            if (lastCompletedLevel < Defs.LEVEL_AMOUNT-1)
            {
                int i = lastCompletedLevel + 1;
                index = i;

                if (i < levelSelectionObjects.Count)
                {
                    GameObject go = levelSelectionObjects[i].gameObject;
                    Image trophy0 = go.transform.Find("TrophyBg/Trophy0").GetComponent<Image>();
                    Image trophy1 = go.transform.Find("TrophyBg/Trophy1").GetComponent<Image>();
                    Image trophy2 = go.transform.Find("TrophyBg/Trophy2").GetComponent<Image>();
                    TextMeshProUGUI name = go.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                    SetLevelOpen(go, i, trophy0, trophy1, trophy2, name);
                    SetSelectionActive(levelSelectionObjects[i]);
                    if(i >= 5)
                    {
                        isBottomRow = true;
                    }
                }
            }
            else
            {
                //print("ALL LEVELS OPEN - just select the last one");
                SetSelectionActive(levelSelectionObjects[Defs.LEVEL_AMOUNT-1]);
                index = Defs.LEVEL_AMOUNT - 1;
                isBottomRow = true;
            }

            back.GetComponent<SpriteRenderer>().color = Color.gray;

            if (!music.IsPlaying)
            {
                music.Play(LevelLoader.GetCurrentSceneName());
            }
        }

        private void SetLevelOpen(GameObject go, int i, Image trophy0, Image trophy1, Image trophy2, TextMeshProUGUI name)
        {
            go.GetComponent<LevelUtils>().open = true;
            go.GetComponent<Image>().color = colors[ColorInfoKey.Open].BgColor;
            name.color = colors[ColorInfoKey.Open].TextColor;

            if (PlayerStats.CompletedObjectives[i].CompletedNoHits == true)
            {
                trophy0.color = colors[ColorInfoKey.Open].TrophyColor;
            }
            else
            {
                trophy0.color = colors[ColorInfoKey.Closed].TrophyColor;
            }

            if (PlayerStats.CompletedObjectives[i].CompletedPoints == true)
            {
                trophy1.color = colors[ColorInfoKey.Open].TrophyColor;
            }
            else
            {
                trophy1.color = colors[ColorInfoKey.Closed].TrophyColor;
            }

            if (PlayerStats.CompletedObjectives[i].CompletedTime == true)
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

            // TODO: Hack, this is made for 10 levels only. Tweak the values and if-elses when new level packs are added
            if (selection == Selection.Scroll)
            {
                int prevIndex = index;

                if (deltaX != 0)
                {
                    if (deltaX == 1)
                    {
                        if (index == 0 /*|| index == 5*/)
                        {
                            selection = Selection.Back;
                            back.GetComponent<SpriteRenderer>().color = Color.white;
                            SetSelectionInactive(levelSelectionObjects[index]);
                        }
                        else
                        {
                            /*
                            if (Defs.LEVEL_AMOUNT > 10 && (index == 10 || index == 15))
                            {
                                index = index - 6;
                            }
                            else
                            */
                            {
                                index = index - 1;
                                if(index < 0)
                                {
                                    index = 0;
                                }
                                if(index <= 4)
                                {
                                    isBottomRow = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        /*
                        if (Defs.LEVEL_AMOUNT > 10 && (index == 4 || index == 9))
                        {
                            index = index + 6;
                        }
                        else if (index == 14) {
                            //do nothing...
                        }
                        else
                        */
                        {
                            index = index + 1;
                            if(index > 9)
                            {
                                index = 9;
                            }
                            if(index >= 5)
                            {
                                isBottomRow = true;
                            }
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
                            if(index > 9)
                            {
                                index = 9;
                            }
                            isBottomRow = true;
                        }
                    }
                    else
                    {
                        if (isBottomRow)
                        {
                            index = index - 5;
                            if(index < 0)
                            {
                                index = 0;
                            }
                            isBottomRow = false;
                        }
                        else
                        {
                            selection = Selection.Back;
                            back.GetComponent<SpriteRenderer>().color = Color.white;
                            SetSelectionInactive(levelSelectionObjects[index]);
                        }
                    }
                }

                if (selection == Selection.Scroll)
                {
                    index = Mathf.Clamp(index, 0, levelSelectionObjects.Count - 1);
                    //print("ALRIGHTY THEN: index is: " + index);
                    SetSelectionInactive(levelSelectionObjects[prevIndex]);
                    SetSelectionActive(levelSelectionObjects[index]);
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
                    SetSelectionActive(levelSelectionObjects[index]);
                }
            }

            //print("NAVIGATE: Current index is: " + index);
        }

        private void SetSelectionActive(Transform t)
        {
            if (selectionSequence != null)
            {
                selectionSequence.Kill();
                selectionSequence = null;
            }
            selectionSequence = DOTween.Sequence()
                .Append(t.DOScale(Defs.MENU_SELECTED_SCALE, Defs.MENU_LEVELSELECT_SCALE_SPEED))
                //.SetDelay(0.3f))
                //.Append(t.DOScale(Vector2.one, 0.3f))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void SetSelectionInactive(Transform t)
        {
            if (selectionSequence != null)
            {
                selectionSequence.Kill();
                selectionSequence = null;
            }

            t.DOScale(Defs.MENU_NORMAL_SCALE, Defs.MENU_LEVELSELECT_SCALE_SPEED);
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

        private void OnCancel(InputAction.CallbackContext context)
        {
            levelLoader.LoadScene((int)LevelLoader.Scenes.CharacterSelect);
        }
    }
}