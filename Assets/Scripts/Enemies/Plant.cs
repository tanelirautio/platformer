using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{

    public class Plant : Enemy
    {
        private Vector2 raycastDir;
        public float raycastDistance = 5f;

        // Start is called before the first frame update
        void Start()
        {
            raycastDir = Vector2.left;
            if (facing == Facing.Right)
            {
                spriteRenderer.flipX = true;
                raycastDir = Vector2.right;
            }
        }

        // Update is called once per frame
        void Update()
        {
           

            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, raycastDir, raycastDistance, 1 << LayerMask.NameToLayer("Player"));
            if(hit)
            {
                Debug.Log("*** HIT *** ");
                Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.green);
            }
        }
    }

}
