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
            //print("Mainmenu start");
            ParseData();

            saveData = SaveSystem.Load();
            //print("Savedata loaded!");
            if (saveData != null)
            {
                //print("Save version: " + saveData.version);

                ShowLoadOption = true;

                PlayerStats.MusicVolume = saveData.musicVolume;
                PlayerStats.SoundVolume = saveData.soundVolume;

                // Level objectives
                for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
                {
                    PlayerStats.CompletedObjectives[i].CompletedNoHits = saveData.levelObjectivesCompleted[i, 0];
                    PlayerStats.CompletedObjectives[i].CompletedPoints = saveData.levelObjectivesCompleted[i, 1];
                    PlayerStats.CompletedObjectives[i].CompletedTime = saveData.levelObjectivesCompleted[i, 2];
                    PlayerStats.CompletedObjectives[i].BestTime = saveData.bestTimes[i];
                }
                // Best scores
                for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
                {
                    PlayerStats.BestScores[i] = saveData.bestScores[i];
                }

                // Statistics
                PlayerStats.CollectedHearts = saveData.collectedHearts;
                PlayerStats.CollectedApples = saveData.collectedApples;
                PlayerStats.CollectedBananas = saveData.collectedBananas;
                PlayerStats.CollectedCherries = saveData.collectedCherries;
                PlayerStats.CollectedKiwis = saveData.collectedKiwis;
                PlayerStats.CollectedMelons = saveData.collectedMelons;
                PlayerStats.CollectedOranges = saveData.collectedOranges;
                PlayerStats.CollectedPineapples = saveData.collectedPineapples;
                PlayerStats.CollectedStrawberries = saveData.collectedStrawberries;

                
                for(int i=0; i < saveData.levelsCompleted.Length; i++)
                {
                    if(i < PlayerStats.LevelsCompleted.Length)
                    {
                        PlayerStats.LevelsCompleted[i] = saveData.levelsCompleted[i];
                    }
                }                
                
                for(int i=0; i < saveData.levelsCompletedWithoutHits.Length; i++)
                {
                    if(i < PlayerStats.LevelsCompletedWithoutHits.Length)
                    {
                        PlayerStats.LevelsCompletedWithoutHits[i] = saveData.levelsCompletedWithoutHits[i];
                    }
                }

                for(int i=0; i < saveData.completedAchievements.Length; i++)
                {
                    if(i < PlayerStats.CompletedAchievements.Length)
                    {
                        PlayerStats.CompletedAchievements[i] = saveData.completedAchievements[i];
                    }
                }
               
            }
            else
            {
                ShowLoadOption = false;
                PlayerStats.MusicVolume = 1;
                PlayerStats.SoundVolume = 1;
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
