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
        public LoopType loopType = LoopType.Restart;

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
