using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace pf
{
    public class FirePit : MonoBehaviour
    {
        private Animator anim;
        private GameObject flame;
        private bool firePitOn = false;

        private Light2D fireLight;
        private float lightBaseIntensity;

        //animation states
        const string FIRE_PIT_OFF = "fire_pit_off";
        const string FIRE_PIT_TRIGGER = "fire_pit_trigger";
        const string FIRE_PIT_ON = "fire_pit_on";

        private void Awake()
        {
            anim = GetComponent<Animator>();
            flame = transform.Find("Flame").gameObject;

            fireLight = GetComponent<Light2D>();
            lightBaseIntensity = fireLight.intensity;
        }

        private void Start()
        {
            anim.Play(FIRE_PIT_OFF);
            flame.SetActive(false);
        }

        private void Update()
        {
            if (fireLight.enabled)
            {
                fireLight.intensity = lightBaseIntensity + 0.4f * Mathf.PerlinNoise(10.0f * Time.realtimeSinceStartup, 0.0f);
            }
        }

        public void Trigger()
        {
            if (!firePitOn)
            {
                anim.Play(FIRE_PIT_TRIGGER);
                Invoke("TurnOn", Defs.TRAP_FIRE_PIT_TURNON_TIME);
                firePitOn = true;
            }
        }

        private void TurnOn()
        {
            anim.Play(FIRE_PIT_ON);
            flame.SetActive(true);
            fireLight.enabled = true;
        }

    }
}
