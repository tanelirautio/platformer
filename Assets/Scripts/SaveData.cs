using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //TODO: read from definitions
    public const int LEVEL_AMOUNT = 2;
    public const int OBJECTIVES_PER_LEVEL = 3;

    public int selectedCharacter;
    public int currentLevel;
    public int[] scores;
    public int health;
    public bool[,] levelObjectivesCompleted;
    public float[] bestTimes;

    public SaveData()
    {
        selectedCharacter = PlayerStats.SelectedCharacter;
        currentLevel = PlayerStats.SceneIndex;
        health = PlayerStats.Health;

        scores = new int[LEVEL_AMOUNT];
        for(int i=0; i < PlayerStats.Scores.Count; i++)
        {
            scores[i] = PlayerStats.Scores[i];
        }

        levelObjectivesCompleted = new bool[LEVEL_AMOUNT, OBJECTIVES_PER_LEVEL];
        bestTimes = new float[LEVEL_AMOUNT];

        for(int i=0; i < PlayerStats.CompletedObjectives.Count; i++)
        {
            LevelObjectives l = PlayerStats.CompletedObjectives[i];

            levelObjectivesCompleted[i, 0] = l.CompletedPoints;
            levelObjectivesCompleted[i, 1] = l.CompletedTime;
            levelObjectivesCompleted[i, 2] = l.CompletedNoHits;

            bestTimes[i] = l.BestTime;
        }
    }


}
