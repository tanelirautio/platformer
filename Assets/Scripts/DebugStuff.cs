using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace pf
{
    public class DebugStuff : MonoBehaviour
    {
        private LevelLoader levelLoader;

        void Start()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        }

        void Update()
        {
            // TODO: this is for debugging
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                print("should exit!");
                if(levelLoader == null)
                {
                    print("levelloader is null, fetch it from scene...");
                    levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
                }
                if (levelLoader == null)
                {
                    print("levelloader is still null!");
                }
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                SaveSystem.Save();
                print("Saved!");
            }

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                SaveData s = SaveSystem.Load();
                print("*****************************");
                print("saved character is: " + s.selectedCharacter);
                print("saved level is: " + s.currentLevel);
                print("saved health is: " + s.health);

                for (int i = 0; i < s.bestScores.Length; i++)
                {
                    print("best score " + i + ": " + s.bestScores[i]);
                }

                for (int i = 0; i < s.bestTimes.Length; i++)
                {
                    print("best time " + i + ": " + s.bestTimes[i]);
                }

                for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
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

                for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
                {
                    print("level " + i + " completed: " + s.levelsCompleted[i] + " | completed without hits: " + s.levelsCompletedWithoutHits[i]);
                }
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                SaveSystem.Reset();
                print("Save file reset!");

            }
        }

    }
}
