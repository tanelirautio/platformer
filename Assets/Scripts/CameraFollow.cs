using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class CameraFollow : MonoBehaviour
    {
        private Controller2D target;
        public Vector2 focusAreaSize;
        public float lookAheadDstX;
        public float lookSmoothTimeX;
        public float verticalSmoothTime;
        public float verticalOffset;

        FocusArea focusArea;

        float currentLookAheadX;
        float targetLookAheadX;
        float smoothLookVelocityX;
        float smoothVelocityY;

        bool lookAheadStopped;
        [SerializeField] private bool isFollowingPlayer = true;

        public void Awake()
        {
            target = GameObject.Find("Player").GetComponent<Controller2D>();
        }

        private void Start()
        {
            focusArea = new FocusArea(target.m_collider.bounds, focusAreaSize);
        }

        public void Reset()
        {
            isFollowingPlayer = true;
        }

        public void StopFollowingPlayer()
        {
            print("stop following player!");
            isFollowingPlayer = false;
        }

        private void LateUpdate()
        {
            if (!isFollowingPlayer)
            {
                return;
            }

            focusArea.Update(target.m_collider.bounds);

            Vector2 focusPosition = focusArea.bounds.center + verticalOffset * Vector2.up;

            if (focusArea.velocity.x != 0)
            {
                float lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4.0f;
                }
            }

            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
            focusPosition += Vector2.right * currentLookAheadX;
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(focusArea.bounds.center, focusArea.bounds.size);
        }

        struct FocusArea
        {
            public Rect bounds;
            public Vector2 velocity;

            public FocusArea(Bounds target, Vector2 size)
            {
                var position = new Vector2(
                    target.center.x - size.x / 2,
                    target.min.x
                );

                bounds = new Rect(position, size);
                velocity = Vector2.zero;
            }

            public void Update(Bounds target)
            {
                float dx = 0;
                if (target.min.x < bounds.xMin)
                {
                    dx = target.min.x - bounds.xMin;
                }
                else if (target.max.x > bounds.xMax)
                {
                    dx = target.max.x - bounds.xMax;
                }

                float dy = 0;
                if (target.min.y < bounds.yMin)
                {
                    dy = target.min.y - bounds.yMin;
                }
                else if (target.max.y > bounds.yMax)
                {
                    dy = target.max.y - bounds.yMax;
                }

                velocity = new Vector2(dx, dy);
                bounds.center += new Vector2(dx, dy);
            }
        }
    }
}
