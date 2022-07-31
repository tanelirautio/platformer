using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{

    public class Plant : Enemy
    {
        const string IDLE = "idle";
        const string ATTACK = "attack";

        private Vector2 raycastDir;
        public float raycastDistance = 10f;

        public GameObject bulletPrefab;

        private bool shouldRaycast = true;
        private float timer = 0f;

        private Player player;

        void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();

            ChangeAnimState(IDLE);

            raycastDir = Vector2.left;
            if (facing == Facing.Right)
            {
                raycastDir = Vector2.right;
            }
        }

        void Update()
        {
            if(!shouldRaycast)
            {
                timer += Time.deltaTime;
                if(timer >= 0.667f)
                {
                    if(audioManager != null)
                    {
                        audioManager.PlaySound2D("Peashooter");
                    }
                    print("SHOOT!");
                    GameObject bullet = Instantiate(bulletPrefab, transform.position + (Vector3)raycastDir*0.8f + Vector3.up * 0.2f, Quaternion.identity);
                    bullet.GetComponent<Bullet>().SetDirection(raycastDir);
                    timer = 0f;
                    shouldRaycast = true;
                }
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, raycastDir, raycastDistance, 1 << LayerMask.NameToLayer("Player"));
            if(hit)
            {
                if (!player.IsDead)
                {
                    ChangeAnimState(ATTACK);
                    shouldRaycast = false;
                    Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.red);
                }
            }
            else
            {
                shouldRaycast = true;
                timer = 0f;
                ChangeAnimState(IDLE);
                Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.green);
            }
        }

       
    }

}
