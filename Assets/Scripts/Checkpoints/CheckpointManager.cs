using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class CheckpointManager : MonoBehaviour
    {
        private SpawnPoint spawnPoint;
        public List<Checkpoint> checkPoints;

        private void Start()
        {
            spawnPoint = GameObject.Find("SpawnPoint").GetComponent<SpawnPoint>();
        }

        public Transform GetLatest()
        {
            Checkpoint checkpoint = null;
            foreach(Checkpoint c in checkPoints)
            {
                if(c.IsTriggered())
                {
                    checkpoint = c;
                }
            }

            if(checkpoint != null)
            {
                return checkpoint.transform;
            }
            else
            {
                return spawnPoint.transform;
            }
            
        }
    }
}