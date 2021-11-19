using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectives : MonoBehaviour
{
    private int level;
    private int points;
    private float time;

    public LevelObjectives(int level, int points, float time)
    {
        this.level = level;
        this.points = points;
        this.time = time;
    }



    /*
    public Dictionary<int, List<LevelObjectives>> objectives = new Dictionary<int, List<LevelObjectives>>()
    {
        { 0, new List<LevelObjectives> { new LevelObjectives(ObjectiveType.Points, 300),
                                         new LevelObjectives(ObjectiveType.Died, 1),
                                         new LevelObjectives(ObjectiveType.CollectItem, 0) }
        },
        { 1, new List<LevelObjectives> { new LevelObjectives(ObjectiveType.Points, 300),
                                         new LevelObjectives(ObjectiveType.Died, 1),
                                         new LevelObjectives(ObjectiveType.CollectItem, 0) }
        }
    }
    */

    // TODO: create 3 objectives for each level
    // At level end, check if objectives met (or if objectives have been met before)

}
