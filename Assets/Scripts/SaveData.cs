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

        public SaveData()
        {
            selectedCharacter = PlayerStats.SelectedCharacter;
            currentLevel = PlayerStats.SceneIndex;
            health = PlayerStats.Health;

            scores = new int[Defs.LEVEL_AMOUNT];
            for (int i = 0; i < PlayerStats.Scores.Count; i++)
            {
                scores[i] = PlayerStats.Scores[i];
            }
            bestScores = new int[Defs.LEVEL_AMOUNT];
            for (int i = 0; i < PlayerStats.BestScores.Count; i++)
            {
                bestScores[i] = PlayerStats.BestScores[i];
            }

            levelObjectivesCompleted = new bool[Defs.LEVEL_AMOUNT, Defs.OBJECTIVES_PER_LEVEL];
            bestTimes = new float[Defs.LEVEL_AMOUNT];

            for (int i = 0; i < PlayerStats.CompletedObjectives.Count; i++)
            {
                LevelObjectives l = PlayerStats.CompletedObjectives[i];

                levelObjectivesCompleted[i, 0] = l.CompletedPoints;
                levelObjectivesCompleted[i, 1] = l.CompletedTime;
                levelObjectivesCompleted[i, 2] = l.CompletedNoHits;

                bestTimes[i] = l.BestTime;
            }
        }


    }
}
