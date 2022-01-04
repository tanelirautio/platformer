using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace pf
{
    public class DataLoader : MonoBehaviour
    {

        private SaveData saveData = null; 
        private static bool initialized = false;

        public bool ShowLoadOption { get; set; }

        void Start()
        {
            print("Mainmenu start");
            ParseData();

            saveData = SaveSystem.Load();
            print("Savedata loaded!");
            if (saveData != null)
            {
                ShowLoadOption = true;

                // Level objectives
                for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
                {
                    PlayerStats.CompletedObjectives[i].CompletedNoHits = saveData.levelObjectivesCompleted[i, 0];
                    PlayerStats.CompletedObjectives[i].CompletedPoints = saveData.levelObjectivesCompleted[i, 1];
                    PlayerStats.CompletedObjectives[i].CompletedTime = saveData.levelObjectivesCompleted[i, 2];      
                }
                // Best scores
                for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
                {
                    PlayerStats.BestScores[i] = saveData.bestScores[i];
                }

                // Statistics
                /*
                PlayerStats.CollectedApples = saveData.collectedApples;
                PlayerStats.CollectedBananas = saveData.collectedBananas;
                PlayerStats.CollectedCherries = saveData.collectedCherries;
                PlayerStats.CollectedKiwis = saveData.collectedKiwis;
                PlayerStats.CollectedMelons = saveData.collectedMelons;
                PlayerStats.CollectedOranges = saveData.collectedOranges;
                PlayerStats.CollectedPineapples = saveData.collectedPineapples;
                PlayerStats.CollectedStrawberries = saveData.collectedStrawberries;
                */

                /*
                for(int i=0; i<saveData.levelsCompleted.Length; i++)
                {
                    if(i < PlayerStats.LevelsCompleted.Count)
                    {
                        PlayerStats.LevelsCompleted[i] = saveData.levelsCompleted[i];
                    }
                }                
                
                for(int i=0; i<saveData.levelsCompletedWithoutHits.Length; i++)
                {
                    if(i < PlayerStats.LevelsCompletedWithoutHits.Count)
                    {
                        PlayerStats.LevelsCompletedWithoutHits[i] = saveData.levelsCompletedWithoutHits[i];
                    }
                }
                */
            }
            else
            {
                ShowLoadOption = false;
            }
        }

        public SaveData GetSaveData()
        {
            return saveData;
        }

        public static void ParseData()
        {
            if (!initialized)
            {
                TextAsset levelObjectivesText = Resources.Load<TextAsset>("levelObjectives");
                if (levelObjectivesText != null)
                {
                    LevelObjectives[] obj = JsonHelper.FromJson<LevelObjectives>(levelObjectivesText.text);
                    if (obj != null)
                    {
                        Assert.AreEqual(obj.Length, Defs.LEVEL_AMOUNT);
                        for (int i = 0; i < obj.Length; i++)
                        {
                            PlayerStats.LevelObjectives.Add(obj[i]);
                        }
                    }
                }

                TextAsset achievementText = Resources.Load<TextAsset>("achievements");
                if(achievementText != null)
                {
                    Achievements[] ach = JsonHelper.FromJson<Achievements>(achievementText.text);
                    if(ach != null)
                    {
                        for(int i=0; i<ach.Length; i++)
                        {
                            PlayerStats.Achievements.Add(ach[i]);
                        }
                    }
                }

                initialized = true;
            }
        }
    }
}
