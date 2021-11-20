using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectives
{
    private int level;
    private int points;
    private float time;

    public bool CompletedPoints { get; set; }
    public bool CompletedTime { get; set; }
    public bool CompletedNoDeaths { get; set; }
    public float BestTime { get; set; }

    public LevelObjectives(int level, int points, float time)
    {
        this.level = level;
        this.points = points;
        this.time = time;
    }
}
