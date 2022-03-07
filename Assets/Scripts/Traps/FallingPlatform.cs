using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace pf
{
    public class FallingPlatform : RaycastController
    { 
        public LayerMask passengerMask;


        public Vector3[] localIdleWaypoints = new Vector3[2];
        private Vector3[] globalIdleWaypoints;

        public Vector3 localFallingWaypoint;
        private Vector3[] globalFallingWaypoints;

        Vector3 fallStartPosition;

        public float speed;
        public float fallingSpeed;

        public float waitTime;
        [Range(0, 2)]
        public float easeAmount;

        int fromWaypointIndex;
        float percentBetweenWaypoints;
        float nextMoveTime;



        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

        enum State
        {
            Idle,
            Falling,
            Bottom,
            GoingUp,
        }
        State state = State.Idle;

        public override void Start()
        {
            base.Start();

            // Waypoints that the platform goes when player not on top
            globalIdleWaypoints = new Vector3[2];
            globalIdleWaypoints[0] = localIdleWaypoints[0] + transform.position;
            globalIdleWaypoints[1] = localIdleWaypoints[1] + transform.position;

            // Waypoints that the platform goes when player on top (will continue stay in localFallingWaypoint)
            globalFallingWaypoints = new Vector3[2];
            globalFallingWaypoints[0] = localIdleWaypoints[0] + transform.position;
            globalFallingWaypoints[1] = localFallingWaypoint + transform.position;
        }

        void Update()
        {
            UpdateRaycastOrigins();
            Vector3 velocity = Vector3.zero;

            if (state != State.Falling && PlayerDetectedOnPlatform())
            {
                //TODO: start timer, fall the platform (change state) after 0.5 seconds or smth
                state = State.Falling;
            }     
            
            // Always fall to bottom when State.Falling is set
            if(state == State.Falling) {
                velocity = CalculatePlatformFallMovement(true);
            }
            else if(state == State.Idle)
            {
                velocity = CalculatePlatformIdleMovement();                 
            }

            //TODO: detect player leaving the platform and move up
            //only after moving back to globalIdleWaypoint[0] change state back to State.Idle
            
            CalculatePassengerMovement(velocity);
            MovePassengers(true);
            transform.Translate(velocity);
            MovePassengers(false);
        }

        private bool PlayerDetectedOnPlatform()
        {
            float rayLength = SKINWIDTH * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                Debug.DrawLine(rayOrigin, rayOrigin + Vector2.up * rayLength, Color.blue);

                if (hit && hit.distance != 0)
                {         
                    if(fallStartPosition == Vector3.zero)
                    {
                        fallStartPosition = transform.position;
                    }
                    return true;
                }
            }
            return false;
        }

        float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        Vector3 CalculatePlatformIdleMovement()
        {
            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }

            fromWaypointIndex %= globalIdleWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalIdleWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalIdleWaypoints[fromWaypointIndex], globalIdleWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

            Vector3 newPos = Vector3.Lerp(globalIdleWaypoints[fromWaypointIndex], globalIdleWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if (percentBetweenWaypoints >= 1)
            {
                percentBetweenWaypoints = 0;
                fromWaypointIndex++;
                nextMoveTime = Time.time + waitTime;
            }

            return newPos - transform.position;
        }



        Vector3 CalculatePlatformFallMovement(bool falling)
        {
            if (falling)
            {
                float distanceBetweenWaypoints = Vector3.Distance(fallStartPosition, globalFallingWaypoints[1]);
                percentBetweenWaypoints += Time.deltaTime * fallingSpeed / distanceBetweenWaypoints;
                percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
                float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

                Vector3 newPos = Vector3.Lerp(fallStartPosition, globalFallingWaypoints[1], easedPercentBetweenWaypoints);
                return newPos - transform.position;
            }

            /*
            else
            {
                fromWaypointIndex %= globalWaypoints.Length;
                float distanceBetweenWaypoints = Vector3.Distance(globalFallingWaypoint, globalWaypoints[fromWaypointIndex]);
                percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
                percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
                float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

                Vector3 newPos = Vector3.Lerp(globalFallingWaypoint, globalWaypoints[fromWaypointIndex], easedPercentBetweenWaypoints);

                if (percentBetweenWaypoints >= 1) 
                {
                    state = State.Idle;
                }
                return newPos - transform.position;
            }   
            */
            return Vector3.zero;
        }

        void MovePassengers(bool beforeMovePlatform)
        {
            foreach (PassengerMovement passenger in passengerMovement)
            {
                if (!passengerDictionary.ContainsKey(passenger.transform))
                {
                    passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
                }
                if (passenger.moveBeforePlatform == beforeMovePlatform)
                {
                    passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
                }
            }
        }

        void CalculatePassengerMovement(Vector3 velocity)
        {
            HashSet<Transform> movedPassengers = new HashSet<Transform>();
            passengerMovement = new List<PassengerMovement>();

            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            // Vertically moving platform
            /*
            if (velocity.y != 0)
            {
                float rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                    rayOrigin += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                    //Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.blue);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = (directionY == 1) ? velocity.x : 0;
                            float pushY = velocity.y - (hit.distance - SKINWIDTH) * directionY;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                        }
                    }
                }
            }
            */

            /*

            // Horizontally moving platform
            if (velocity.x != 0)
            {
                float rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

                for (int i = 0; i < horizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                    rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = velocity.x - (hit.distance - SKINWIDTH) * directionX;
                            float pushY = -SKINWIDTH;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                        }
                    }
                }

            }
            */

            // Passenger on top of a horizontally or downward moving platform
            //if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                float rayLength = SKINWIDTH * 2;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = raycastOrigins.topLeft += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = velocity.x;
                            float pushY = velocity.y;


                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                        }
                    }
                }
            }

            
        }

        struct PassengerMovement
        {
            public Transform transform;
            public Vector3 velocity;
            public bool standingOnPlatform;
            public bool moveBeforePlatform;

            public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
            {
                transform = _transform;
                velocity = _velocity;
                standingOnPlatform = _standingOnPlatform;
                moveBeforePlatform = _moveBeforePlatform;
            }
        }

        private void OnDrawGizmos()
        {
            if (localIdleWaypoints != null)
            {
                Gizmos.color = Color.red;
                float size = 0.3f;
                for (int i = 0; i < localIdleWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? globalIdleWaypoints[i] : localIdleWaypoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }

                Gizmos.color = Color.green;
                Vector3 globalFallingWaypointPos = (Application.isPlaying) ? globalFallingWaypoints[1] : localFallingWaypoint + transform.position;
                Gizmos.DrawLine(globalFallingWaypointPos - Vector3.up * size, globalFallingWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalFallingWaypointPos - Vector3.left * size, globalFallingWaypointPos + Vector3.left * size);
            }
        }
    }
}