using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectives : MonoBehaviour
{
    public enum ObjectiveType
    {
        Points,
        Time,
        Kills,
        Died,
        CollectItem
    }

    private class Objective
    {
        private ObjectiveType type;
        private int amount;

        Objective(ObjectiveType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }

    // TODO: create 3 objectives for each level
    // At level end, check if objectives met (or if objectives have been met before)
}
