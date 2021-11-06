using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace pf
{
    public class ContinueMenu : MonoBehaviour
    {
        private LevelLoader levelLoader;

        enum Selection
        {
            Yes,
            No
        }

        private Selection selection = Selection.Yes;
        private GameObject yesImage;
        private GameObject noImage;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        }

        private void Start()
        {
            yesImage = GameObject.Find("UICanvas/YesImage");
            noImage = GameObject.Find("UICanvas/NoImage");
            noImage.SetActive(false);
        }

        void Update()
        {
            //quick hack to get something working, replace with real input manager
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selection == Selection.Yes)
                {
                    selection = Selection.No;
                    noImage.SetActive(true);
                    yesImage.SetActive(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if(selection == Selection.No)
                {
                    selection = Selection.Yes;
                    noImage.SetActive(false);
                    yesImage.SetActive(true);

                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (selection == Selection.Yes)
                {
                    levelLoader.LoadScene(PlayerStats.CurrentSceneIndex);
                }
                else
                {
                    levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
                }
            }
        }

    }
}
