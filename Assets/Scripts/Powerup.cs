using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Powerup : MonoBehaviour
    {
        public enum Type
        {
            JumpPower
        }

        public Type type;
        private Transform obj;

        private Transform collected;
        private Animator collectedAnim = null;

        private Player player;

        private bool playerHit = false;

        // Start is called before the first frame update
        void Awake()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            
            obj = transform.Find("Object");

            collected = transform.Find("Collected");
            collectedAnim = collected.gameObject.GetComponent<Animator>();
            collected.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (playerHit)
                {
                    return;
                }
                playerHit = true;

                obj.gameObject.SetActive(false);
                collected.gameObject.SetActive(true);
                collectedAnim.Play("collected");

                player.CollectedPoweup(type);
            }
        }
    }
}
