using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class FirePit : MonoBehaviour
    {
        private Animator anim;
        private GameObject flame;
        private bool firePitOn = false;

        //animation states
        const string FIRE_PIT_OFF = "fire_pit_off";
        const string FIRE_PIT_TRIGGER = "fire_pit_trigger";
        const string FIRE_PIT_ON = "fire_pit_on";

        private void Awake()
        {
            anim = GetComponent<Animator>();
            flame = transform.Find("Flame").gameObject;
        }

        private void Start()
        {
            anim.Play(FIRE_PIT_OFF);
            flame.SetActive(false);
        }

        public void Trigger()
        {
            if (!firePitOn)
            {
                anim.Play(FIRE_PIT_TRIGGER);
                Invoke("TurnOn", 1.0f);
                firePitOn = true;
            }
        }

        private void TurnOn()
        {
            anim.Play(FIRE_PIT_ON);
            flame.SetActive(true);
        }

    }
}
