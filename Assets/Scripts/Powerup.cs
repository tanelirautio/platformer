using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Powerup : MonoBehaviour
    {
        public enum Type
        {
            Jump,
            Speed
        }

        public Type type;
        public float activeTimeSeconds;
        public bool expire;
        public bool respawn;

        private Transform obj;

        private Transform collected;
        private Animator collectedAnim = null;

        private Player player;
        private AudioManager audioManager;

        private bool powerupCollected = false;

        private class ActivePowerup
        {
            public ActivePowerup(Type type, float time, bool willExpire)
            {
                Type = type;
                Time = time;
                WillExpire = willExpire;
            }

            public Type Type { get; set; }
            public float Time { get; set; }
            public bool WillExpire { get; set; }
        };
        private static List<ActivePowerup> activePowerups = new List<ActivePowerup>();
        private static List<GameObject> respawnablePowerupsInScene = new List<GameObject>();

        void Awake()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();
            
            obj = transform.Find("Object");

            collected = transform.Find("Collected");
            collectedAnim = collected.gameObject.GetComponent<Animator>();
            collected.gameObject.SetActive(false);

            if (respawn)
            {
                respawnablePowerupsInScene.Add(this.gameObject);
            }
        }

        private void Update()
        {
            if(activePowerups.Count > 0)
            {
                for(int i = activePowerups.Count -1; i >= 0; i--) 
                {
                    if(activePowerups[i].WillExpire && activePowerups[i].Time > 0)
                    {
                        activePowerups[i].Time -= Time.deltaTime;
                        //print(activePowerups[i].Time);
                        if(activePowerups[i].Time <= 0)
                        {
                            //print("Powerup: " + type.ToString() + " expired!");
                            player.PowerupExpired(type);
                            activePowerups.RemoveAt(i);
                        }
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (powerupCollected)
                {
                    return;
                }
                powerupCollected = true;
                audioManager.PlaySound2D("Powerup");

                obj.gameObject.SetActive(false);
                collected.gameObject.SetActive(true);
                collectedAnim.Play("collected");

                player.CollectedPowerup(type);

                activePowerups.Add(new ActivePowerup(type, activeTimeSeconds, expire));
            }
        }

        public static void Uninit()
        {
            activePowerups.Clear();
            respawnablePowerupsInScene.Clear();
        }

        public static void Respawn()
        {
            //Debug.Log("Trying to respawn powerups in the scene...");
            foreach(GameObject obj in respawnablePowerupsInScene)
            {
                if(obj.GetComponent<Powerup>().respawn == true)
                {
                    GameObject powerup = obj.transform.Find("Object").gameObject;
                    if (!powerup.activeSelf)
                    {
                        obj.transform.Find("Object").gameObject.SetActive(true);
                        obj.transform.Find("Collected").gameObject.SetActive(false);
                        obj.GetComponent<Powerup>().powerupCollected = false;
                    }
                }
            }
        }
    }
}
