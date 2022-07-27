using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace pf
{
    public class SpikedBall : MonoBehaviour
    {
        public enum Direction
        {
            Right,
            Left,
        }

        private Direction dir = Direction.Right;

        public float duration = 1.5f;
        public int angleLimit = 0;
        public bool isStatic = false;
     
        void Start()
        {
            if (!isStatic && angleLimit != 0)
            {
                RotateRight();
            }
        }


        void RotateRight()
        {
            dir = Direction.Right;
            transform.DORotate(new Vector3(0, 0, angleLimit), duration).OnComplete(RotateStart).SetEase(Ease.Linear);
        }

        void RotateStart()
        {
            if (dir == Direction.Right)
            {
                transform.DORotate(new Vector3(0, 0, 0), duration).OnComplete(RotateLeft).SetEase(Ease.Linear);
            }
            else
            {
                transform.DORotate(new Vector3(0, 0, 0), duration).OnComplete(RotateRight).SetEase(Ease.Linear);
            }

        }

        void RotateLeft()
        {
            dir = Direction.Left;
            transform.DORotate(new Vector3(0,0,-angleLimit), duration).OnComplete(RotateStart).SetEase(Ease.Linear);
        }

        public void Update()
        {
            if(!isStatic && angleLimit == 0)
            {
                transform.Rotate(new Vector3(0, 0, Time.deltaTime * duration*60f));
            }
        }
        private void OnDestroy()
        {
            DOTween.KillAll();
        }
    }
}
