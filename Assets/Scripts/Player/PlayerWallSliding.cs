using UnityEngine;

namespace pf
{
    public class PlayerWallSliding : MonoBehaviour
    {
        public Vector2 wallJumpClimb;
        public Vector2 wallJumpOff;
        public Vector2 wallLeap;
        public float wallSlideSpeedMax = 3;
        public float wallStickTime = 0.25f;
        public float timeToWallUnstick;

        private bool wallSliding;
        private int wallDirX;

        public void HandleWallSliding(Controller2D controller, Vector2 directionalInput, Vector2 velocity, float velocityXSmoothing)
        {
            wallDirX = (controller.collisions.left) ? -1 : 1;
            wallSliding = false;
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
            {
                wallSliding = true;

                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (directionalInput.x != wallDirX && directionalInput.x != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
        }

        public void OnJumpInputDown(Vector2 directionalInput, ref Vector3 velocity)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
    }
}
