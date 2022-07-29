using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace pf
{
    public class TweenObjectSequence : MonoBehaviour
    {
        public DG.Tweening.Ease ease = Ease.Linear;
        public float speed = 1f;

        public enum Type
        {
            Restart,
            Yoyo
        }

        public Type type = Type.Restart;

        //[SerializeField] private float waitTime;
        public Vector3[] localWaypoints;
        
        private Vector3[] globalWaypoints;

        Sequence sequence;

        void Start()
        {
            CreateSequence();
        }

        private void CreateSequence() 
        {

            List<Vector3> waypoints = new List<Vector3>(localWaypoints);

            if (type == Type.Yoyo)
            {
           
                print("waypoints length: " + waypoints.Count);

                List<Vector3> waypointsReversed = new List<Vector3>(localWaypoints);
                waypointsReversed.RemoveAt(0); 
                waypointsReversed.RemoveAt(waypointsReversed.Count - 1);
                waypointsReversed.Reverse();

                print("waypointsReversed length: " + waypointsReversed.Count);

                waypoints.AddRange(waypointsReversed);

                print("waypoints length after add: " + waypoints.Count);
            }

            globalWaypoints = new Vector3[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (i != globalWaypoints.Length - 1)
                {
                    globalWaypoints[i] = waypoints[i + 1] + transform.position;
                }
                else
                {
                    globalWaypoints[i] = waypoints[0] + transform.position;
                }
            }

            /*
            globalWaypoints = new Vector3[localWaypoints.Length];
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                if (i != globalWaypoints.Length - 1)
                {
                    globalWaypoints[i] = localWaypoints[i + 1] + transform.position;
                }
                else
                {
                    globalWaypoints[i] = localWaypoints[0] + transform.position;
                }
            }
            */

            float[] time = new float[globalWaypoints.Length];

            float dist = Vector3.Distance(globalWaypoints[globalWaypoints.Length - 1], globalWaypoints[0]);
            time[0] = dist / speed;

            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                if (i < globalWaypoints.Length - 1)
                {
                    float d = Vector3.Distance(globalWaypoints[i], globalWaypoints[i + 1]);
                    time[i+1] = d / speed;
                }
            }

            /*
            for(int i = 0; i<time.Length; i++)
            {
                print("Time " + i + ": " + time[i]);
            }
            */

            sequence = DOTween.Sequence();
            for (int i = 0; i < globalWaypoints.Length; i++)
            {
               sequence.Append(transform.DOMove(globalWaypoints[i], time[i])).SetEase(ease);
            }

            sequence.AppendCallback(CreateSequence);
            StartSequence();
        }

        void StartSequence()
        {
            if(sequence != null)
            {
                sequence.Play();
            }
        }

        private void OnDrawGizmos()
        {
            if (localWaypoints != null)
            {
                Gizmos.color = Color.red;
                float size = 0.3f;
                for (int i = 0; i < localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }
            }
        }
    }
}
