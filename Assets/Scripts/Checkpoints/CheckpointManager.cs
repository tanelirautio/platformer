using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class CheckpointManager : MonoBehaviour
    {
        public SpawnPoint spawnPoint;
        public List<Checkpoint> checkPoints;

        public Checkpoint GetLatest()
        {
            Checkpoint checkpoint = null;
            foreach(Checkpoint c in checkPoints)
            {
                if(c.IsTriggered())
                {
                    checkpoint = c;
                }
            }
            return checkpoint;
        }

        public SpawnPoint GetSpawnPoint()
        {
            return spawnPoint;
        }
    }
}