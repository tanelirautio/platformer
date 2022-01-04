using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class GameManager : MonoBehaviour
    {
        private LevelLoader levelLoader;

        void Start()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        }

        void Update()
        {
            // TODO: this is for debugging
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                /*
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                */

                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }

            if(Input.GetKeyDown(KeyCode.S))
            {
                SaveSystem.Save();
                print("Saved!");
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                SaveData s = SaveSystem.Load();
                print("*****************************");
                print("saved character is: " + s.selectedCharacter);
                print("saved level is: " + s.currentLevel);
                print("saved health is: " + s.health);

                for (int i = 0; i < s.scores.Length; i++)
                {
                    print("current score " + i + ": " + s.scores[i]);
                }

                for (int i=0; i < s.bestScores.Length; i++)
                {
                    print("best score " + i + ": " + s.bestScores[i]);
                }

                for (int i = 0; i < s.bestTimes.Length; i++)
                {
                    print("best time " + i + ": " + s.bestTimes[i]);
                }

                for(int i=0; i < Defs.LEVEL_AMOUNT; i++)
                {
                    print("objectives per level: " + s.levelObjectivesCompleted[i, 0] + ", " + s.levelObjectivesCompleted[i, 1] + ", " + s.levelObjectivesCompleted[i, 2]);
                }

                print("Apples: " + s.collectedApples);
                print("Bananas: " + s.collectedBananas);
                print("Cherries: " + s.collectedCherries);
                print("Kiwis: " + s.collectedKiwis);
                print("Melons: " + s.collectedMelons);
                print("Oranges: " + s.collectedOranges);
                print("Pineapple: " + s.collectedPineapples);
                print("Strawberries: " + s.collectedStrawberries);

                print("****************************");
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SaveSystem.Reset();
                print("Save file reset!");

            }
        }
    }
}
