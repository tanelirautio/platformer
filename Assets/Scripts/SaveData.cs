using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    [System.Serializable]
    public class SaveData
    {
        public int selectedCharacter;
        public int currentLevel;
        public int[] bestScores;
        public int[] scores;
        public int health;
        public bool[,] levelObjectivesCompleted;
        public float[] bestTimes;

        // Statistics
        //public bool[] levelsCompleted;
        //public bool[] levelsCompletedWithoutHits;
        
        public int collectedApples;
        public int collectedBananas;
        public int collectedStrawberries;
        public int collectedCherries;
        public int collectedKiwis;
        public int collectedMelons;
        public int collectedOranges;
        public int collectedPineapples;
        

        public SaveData()
        {
            selectedCharacter = PlayerStats.SelectedCharacter;
            currentLevel = PlayerStats.SceneIndex;
            health = PlayerStats.Health;

            scores = new int[Defs.LEVEL_AMOUNT];
            for (int i = 0; i < PlayerStats.Scores.Length; i++)
            {
                scores[i] = PlayerStats.Scores[i];
            }
            bestScores = new int[Defs.LEVEL_AMOUNT];
            for (int i = 0; i < PlayerStats.BestScores.Length; i++)
            {
                bestScores[i] = PlayerStats.BestScores[i];
            }

            /*
            levelObjectivesCompleted = new bool[Defs.LEVEL_AMOUNT, Defs.OBJECTIVES_PER_LEVEL];
            bestTimes = new float[Defs.LEVEL_AMOUNT];

            for (int i = 0; i < Defs.LEVEL_AMOUNT; i++)
            {
                LevelObjectives l = PlayerStats.CompletedObjectives[i];

                levelObjectivesCompleted[i, 0] = l.CompletedPoints;
                levelObjectivesCompleted[i, 1] = l.CompletedTime;
                levelObjectivesCompleted[i, 2] = l.CompletedNoHits;

                bestTimes[i] = l.BestTime;
            }
            */

            
            collectedApples = PlayerStats.CollectedApples;
            collectedBananas = PlayerStats.CollectedBananas;
            collectedCherries = PlayerStats.CollectedCherries;
            collectedKiwis = PlayerStats.CollectedKiwis;
            collectedMelons = PlayerStats.CollectedMelons;
            collectedOranges = PlayerStats.CollectedOranges;
            collectedPineapples = PlayerStats.CollectedPineapples;
            collectedStrawberries = PlayerStats.CollectedStrawberries;

            /*
            for(int i=0; i <PlayerStats.LevelsCompleted.Count; i++)
            {
                levelsCompleted[i] = PlayerStats.LevelsCompleted[i];
            }

            for(int i=0; i <PlayerStats.LevelsCompletedWithoutHits.Count; i++)
            {
                levelsCompletedWithoutHits[i] = PlayerStats.LevelsCompletedWithoutHits[i];
            }
            */


        }
    }
}
