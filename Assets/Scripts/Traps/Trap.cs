using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Trap : MonoBehaviour
    {
        private FirePit firePit = null;

        public enum Type
        {
            Unknown,
            Spike,
            Saw,
            Fire,
            KillZone,
            SpikeHead
        }

        public Type type;

        private void Awake()
        {
            firePit = GetComponent<FirePit>();
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
