using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class SpawnPoint : MonoBehaviour
    {
        private Animator animator;
        private bool isPlaying = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float radius = 0.5f;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void Reset()
        {
            animator.Play("idle");
            isPlaying = false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                if (!isPlaying)
                {
                    animator.Play("start");
                    isPlaying = true;
                }
            }
        }
    }
}
