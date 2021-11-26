using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Trap : MonoBehaviour
    {

        public enum Type
        {
            Spike,
            Saw,
            KillZone
        }

        public Type type;
    }
}
