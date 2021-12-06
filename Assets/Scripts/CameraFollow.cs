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
        float lookAheadDirX;
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

            Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4.0f;
                    }
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
            Gizmos.DrawCube(focusArea.centre, focusAreaSize);
        }

        struct FocusArea
        {
            public Vector2 centre;
            public Vector2 velocity;
            float left, right;
            float top, bottom;

            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                left = targetBounds.center.x - size.x / 2;
                right = targetBounds.center.x + size.x / 2;
                bottom = targetBounds.min.y;
                top = targetBounds.min.y + size.y;

                velocity = Vector2.zero;
                centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            }

            public void Update(Bounds targetBounds)
            {
                float shiftX = 0;
                if (targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if (targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }
                left += shiftX;
                right += shiftX;

                float shiftY = 0;
                if (targetBounds.min.y < bottom)
                {
                    shiftY = targetBounds.min.y - bottom;
                }
                else if (targetBounds.max.y > top)
                {
                    shiftY = targetBounds.max.y - top;
                }
                top += shiftY;
                bottom += shiftY;

                centre = new Vector2((left + right) / 2, (top + bottom) / 2);
                velocity = new Vector2(shiftX, shiftY);
            }
        }
    }
}
