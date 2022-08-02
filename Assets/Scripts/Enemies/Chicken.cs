using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Chicken : Enemy
    {
        const string IDLE = "idle";
        const string RUN = "run";

        private Vector2 raycastDir;
        public float raycastDistance = 10f;

        public float speed = 5f;
        private bool shouldRaycast = true;
        private Vector2 hitpoint;

        private bool soundTimerOn = false;
        private float soundTimer = 0f;

        // Start is called before the first frame update
        void Start()
        {
            Spawn();
        }

        private void Spawn()
        {
            ChangeAnimState(IDLE);

            raycastDir = Vector2.left;
            if (facing == Facing.Right)
            {
                raycastDir = Vector2.right;
            }
        }

        void Update()
        {
            if (!shouldRaycast)
            {
                if (audioManager != null && !soundTimerOn)
                {
                    audioManager.PlaySound2D("Chicken");
                    soundTimerOn = true;
                }

                //print("RUN!");
                transform.Translate(raycastDir * speed * Time.deltaTime);
                float distance = Vector2.Distance((Vector2)transform.position, hitpoint);
                if(distance < 0.2f)
                {
                    ChangeAnimState(IDLE);
                    shouldRaycast = true;
                }
                return;
            }

            if (soundTimerOn)
            {
                soundTimer += Time.deltaTime;
                if (soundTimer >= 3f)
                {
                    soundTimer = 0f;
                    soundTimerOn = false;
                }
            }

            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, raycastDir, raycastDistance, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Ground"));
            if (hit)
            {
                bool skipPlayerHit = false;
                if (hit.collider.gameObject.tag == "Ground")
                {
                    float distance = Vector2.Distance((Vector2)transform.position, hit.point);
                    if (distance <= 2f)
                    {
                        if (raycastDir == Vector2.left)
                        {
                            raycastDir = Vector2.right;
                            spriteRenderer.flipX = true;
                        }
                        else
                        {
                            raycastDir = Vector2.left;
                            spriteRenderer.flipX = false;
                        }
                        skipPlayerHit = true;
                    }

                }
                
                if (!skipPlayerHit && hit.collider.gameObject.tag == "Player")
                {
                    ChangeAnimState(RUN);
                    hitpoint = hit.point;
                    shouldRaycast = false;
                }
               

                Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.red);
            }
            else
            {
                shouldRaycast = true;
                ChangeAnimState(IDLE);
                Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.green);

            }
        }
    }
}

