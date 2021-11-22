using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelObjectives
{
    public int level;
    public int points;
    public float time;

    public bool CompletedNoDeaths { get; set; } // 1st
    public bool CompletedPoints { get; set; } //2nd
    public bool CompletedTime { get; set; } //3rd
    public float BestTime { get; set; }

    public LevelObjectives(int level, int points, float time)
    {
        this.level = level;
        this.points = points;
        this.time = time;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetRequiredScore()
    {
        return points;
    }

    public float GetRequiredTime()
    {
        return time;
    }
}
