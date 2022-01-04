using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    [Serializable]
    public class LevelObjectives
    {
        public int level = 0;
        public int points = 0;
        public float time = 0;

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
}
