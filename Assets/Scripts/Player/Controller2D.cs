using UnityEngine;
using System.Collections;
using System;

namespace pf
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Controller2D : RaycastController
    {
        public float maxSlopeAngle = 80;

        public CollisionInfo collisions;
        [HideInInspector]
        public Vector2 playerInput;

        public override void Start()
        {
            base.Start();
            collisions.faceDir = 1;
        }

        public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
        {
            Move(moveAmount, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();
            collisions.Reset();
            collisions.moveAmountOld = moveAmount;
            playerInput = input;

            if (moveAmount.x != 0)
            {
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HorizontalCollisions(ref moveAmount);

            if (moveAmount.y != 0)
            {
                VerticalCollisions(ref moveAmount);
            }

            transform.Translate(moveAmount);

            if (standingOnPlatform)
            {
                collisions.below = true;
            }
        }

        void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + SKINWIDTH;

            if (Mathf.Abs(moveAmount.x) < SKINWIDTH)
            {
                rayLength = 2 * SKINWIDTH;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.yellow);

                if (hit)
                {
                    if (hit.distance == 0)
                    {
                        continue;
                    }

                    moveAmount.x = (hit.distance - SKINWIDTH) * directionX;
                    rayLength = hit.distance;
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + SKINWIDTH;

            // Check if we are still intersecting with the collider we
            // chose to fallthrough.
            if (collisions.fallingThroughCollider != null)
            {
                if (!m_collider.bounds.Intersects(collisions.fallingThroughCollider.bounds))
                {
                    collisions.fallingThroughCollider = null;
                }
            }

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hit)
                {
                    if (hit.collider.tag == "Through")
                    {
                        var fallingThrough = (collisions.fallingThroughCollider != null);

                        if (fallingThrough || directionY == 1 || hit.distance == 0)
                        {
                            continue;
                        }

                        if (playerInput.y == -1)
                        {
                            collisions.fallingThroughCollider = hit.collider;
                            continue;
                        }
                    }

                    moveAmount.y = (hit.distance - SKINWIDTH) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }
        }

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public Vector2 moveAmountOld;
            public int faceDir;

            /// Non-null if currently falling through something.
            /// @TODO: This probably should be a list of colliders.
            public Collider2D fallingThroughCollider;

            public void Reset()
            {
                above = below = false;
                left = right = false;
                fallingThroughCollider = null;
            }
        }
    }
}