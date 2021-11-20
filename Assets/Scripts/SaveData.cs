using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //TODO: read from definitions
    const int LEVEL_AMOUNT = 2;
    const int OBJECTIVES_PER_LEVEL = 3;

    public int selectedCharacter;
    public int currentLevel;
    public int score;
    public int health;
    public bool[,] levelObjectivesCompleted;
    public float[] bestTimes;

    public SaveData()
    {
        selectedCharacter = PlayerStats.SelectedCharacter;
        currentLevel = PlayerStats.SceneIndex;
        score = PlayerStats.Score;
        health = PlayerStats.Health;

        levelObjectivesCompleted = new bool[LEVEL_AMOUNT, OBJECTIVES_PER_LEVEL];
        bestTimes = new float[LEVEL_AMOUNT];

        for(int i=0; i < PlayerStats.completedObjectives.Count; i++)
        {
            LevelObjectives l = PlayerStats.completedObjectives[i];

            levelObjectivesCompleted[i, 0] = l.CompletedPoints;
            levelObjectivesCompleted[i, 1] = l.CompletedTime;
            levelObjectivesCompleted[i, 2] = l.CompletedNoDeaths;

            bestTimes[i] = l.BestTime;
        }
    }


}
