using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    [System.Serializable]
    public class SaveData
    {
        public int version;
        public int selectedCharacter;
        public float musicVolume;
        public float soundVolume;
        public int currentLevel;
        public int[] bestScores;
        //public int[] scores;
        public int health;
        public bool[,] levelObjectivesCompleted;
        public float[] bestTimes;

        // Statistics
        public bool[] levelsCompleted;
        public bool[] levelsCompletedWithoutHits;

        public int collectedHearts;
        public int collectedApples;
        public int collectedBananas;
        public int collectedStrawberries;
        public int collectedCherries;
        public int collectedKiwis;
        public int collectedMelons;
        public int collectedOranges;
        public int collectedPineapples;

        // Achievements
        public bool[] completedAchievements;
        
        public SaveData()
        {
            version = Defs.SAVEDATA_VERSION;
            selectedCharacter = PlayerStats.SelectedCharacter;
            currentLevel = PlayerStats.SceneIndex;
            health = PlayerStats.Health;

            musicVolume = PlayerStats.MusicVolume;
            soundVolume = PlayerStats.SoundVolume;

            bestScores = new int[Defs.LEVEL_AMOUNT];
            for (int i = 0; i < PlayerStats.BestScores.Length; i++)
            {
                bestScores[i] = PlayerStats.BestScores[i];
            }

            levelObjectivesCompleted = new bool[Defs.LEVEL_AMOUNT, Defs.OBJECTIVES_PER_LEVEL];
            bestTimes = new float[Defs.LEVEL_AMOUNT];

            for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
            {
                CompletedObjectives c = PlayerStats.CompletedObjectives[i];
                // keep the same order that is declared in CompletedObjectives class
                levelObjectivesCompleted[i, 0] = c.CompletedNoHits;
                levelObjectivesCompleted[i, 1] = c.CompletedPoints;
                levelObjectivesCompleted[i, 2] = c.CompletedTime;
                bestTimes[i] = c.BestTime;
            }

            collectedHearts = PlayerStats.CollectedHearts;
            collectedApples = PlayerStats.CollectedApples;
            collectedBananas = PlayerStats.CollectedBananas;
            collectedCherries = PlayerStats.CollectedCherries;
            collectedKiwis = PlayerStats.CollectedKiwis;
            collectedMelons = PlayerStats.CollectedMelons;
            collectedOranges = PlayerStats.CollectedOranges;
            collectedPineapples = PlayerStats.CollectedPineapples;
            collectedStrawberries = PlayerStats.CollectedStrawberries;

            levelsCompleted = new bool[Defs.LEVEL_AMOUNT];
            for (int i=0; i <PlayerStats.LevelsCompleted.Length; i++)
            {
                levelsCompleted[i] = PlayerStats.LevelsCompleted[i];
            }

            levelsCompletedWithoutHits = new bool[Defs.LEVEL_AMOUNT];
            for(int i=0; i <PlayerStats.LevelsCompletedWithoutHits.Length; i++)
            {
                levelsCompletedWithoutHits[i] = PlayerStats.LevelsCompletedWithoutHits[i];
            }

            completedAchievements = new bool[Defs.ACHIEVEMENT_COUNT];
            for(int i=0; i < Defs.ACHIEVEMENT_COUNT; i++)
            {
                completedAchievements[i] = PlayerStats.CompletedAchievements[i];
            }
        }
    }
}
