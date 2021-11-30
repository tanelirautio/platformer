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
            Fire,
            KillZone
        }

        public Type type;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(type == Type.Fire)
            {
                print("****************");
                print("trigger fire pit");
                print("****************");
            }
        }

    }
}
