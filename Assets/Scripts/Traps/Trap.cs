using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Trap : MonoBehaviour
    {
        private FirePit firePit = null;
        private FallingPlatform fallingPlatform = null;

        public enum Type
        {
            Unknown,
            Spike,
            Saw,
            Fire,
            KillZone,
            SpikeHead,
            FallingPlatform,
            RockHead
        }

        public Type type;

        private void Awake()
        {
            if (type == Type.Fire)
            {
                firePit = GetComponent<FirePit>();
            }
            else if(type == Type.FallingPlatform)
            {
                fallingPlatform = GetComponent<FallingPlatform>();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(type == Type.Fire && firePit != null)
            {
                firePit.Trigger();
            }
        }        
    }
}
