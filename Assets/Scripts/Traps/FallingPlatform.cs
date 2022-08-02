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

        private AudioManager audioManager;

        Vector3 fallStartPosition;

        public float speed;
        public float fallingSpeed;
        public float risingSpeed;

        public float waitTime;
        [Range(0, 2)]
        public float easeAmount;

        int fromWaypointIndex;
        float percentBetweenWaypoints = 0;
        float nextMoveTime = 0;
        bool playerOnPlatform;

        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

        private PauseGame pauseGame;
        
        private float fTime = 0;
        private bool paused = false;

        enum State
        {
            Idle,
            Falling,
            Bottom,
            GoingUp,
        }
        State state = State.Idle;

        public override void Awake()
        {
            base.Awake();
            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();    
            pauseGame = GameObject.Find("UICanvas/PauseMenu").GetComponent<PauseGame>();
        }

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
            if (pauseGame.Paused)
            {
                return;
            }
            else if (pauseGame.ContinuedFromPause)
            {
                fTime = 0;
            }

            fTime += Time.deltaTime;

            UpdateRaycastOrigins();
            Vector3 velocity = Vector3.zero;

            playerOnPlatform = PlayerDetectedOnPlatform();

            if (state != State.Falling && state != State.Bottom && state != State.GoingUp && playerOnPlatform)
            {
                ChangeState(State.Falling);
                audioManager.PlaySound2D("FallingPlatform");
            }
            else if(state == State.Bottom && !playerOnPlatform)
            {
                ChangeState(State.GoingUp);
            }

            // Always fall to bottom when State.Falling is set
            if (state == State.Falling) {
                velocity = CalculatePlatformFallMovement(true);
            }
            else if(state == State.GoingUp)
            {
                velocity = CalculatePlatformFallMovement(false);
            }
            else if(state == State.Idle)
            {
                velocity = CalculatePlatformIdleMovement();                 
            }

            if (velocity.x == float.NaN || velocity.y == float.NaN || velocity.z == float.NaN)
            {
                return;
            }
            else
            {
                //print("Velocity: " + velocity);
            }
                   

            CalculatePassengerMovement(velocity);
            MovePassengers(true);
            transform.Translate(velocity);
            MovePassengers(false);
            
        }

        private void ChangeState(State newState)
        {
            //Debug.Log("Changing state to: " + newState);
            state = newState;
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
                    if(state == State.Idle)
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
            if (fTime < nextMoveTime)
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
                nextMoveTime = fTime + waitTime;
            }

            return newPos - transform.position;
        }

        Vector3 CalculatePlatformFallMovement(bool falling)
        {
            if (falling)
            {
                float distanceBetweenWaypoints = Vector3.Distance(fallStartPosition, globalFallingWaypoints[1]);
                float multiplier = distanceBetweenWaypoints / fallingSpeed;
                float f = multiplier * fallingSpeed;
                //percentBetweenWaypoints += Time.deltaTime * fallingSpeed / distanceBetweenWaypoints;
                percentBetweenWaypoints += Time.deltaTime * f / distanceBetweenWaypoints;
                percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
                //Debug.Log("FallStartPosition y: " + fallStartPosition.y + ", Distance between waypoints: " + distanceBetweenWaypoints + ", f: " + f + ", multiplier:" + multiplier + ", percent between waypoints: " + percentBetweenWaypoints);
                float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);
                Vector3 newPos = Vector3.Lerp(fallStartPosition, globalFallingWaypoints[1], easedPercentBetweenWaypoints);

                if (percentBetweenWaypoints >= 1)
                {
                    percentBetweenWaypoints = 0;
                    ChangeState(State.Bottom);
                }

                return newPos - transform.position;
            }
            else
            {
                float distanceBetweenWaypoints = Vector3.Distance(globalFallingWaypoints[1], globalFallingWaypoints[0]);
                percentBetweenWaypoints += Time.deltaTime * risingSpeed / distanceBetweenWaypoints;
                percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
                float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);
                Vector3 newPos = Vector3.Lerp(globalFallingWaypoints[1], globalFallingWaypoints[0], easedPercentBetweenWaypoints);

                if(playerOnPlatform)
                {
                    percentBetweenWaypoints = 0;
                    fallStartPosition = transform.position;
                    ChangeState(State.Falling);
                }
                else if (percentBetweenWaypoints >= 1)
                {
                    percentBetweenWaypoints = 0;
                    fromWaypointIndex = 0;
                    ChangeState(State.Idle);
                }

                return newPos - transform.position;
            }
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
            

            // Passenger on top of a horizontally or downward moving platform
            if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
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