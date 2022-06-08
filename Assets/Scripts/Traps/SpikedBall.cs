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

        public float speed = 100f;
        public int angleLimit = 0;
     
        void Start()
        {
            if (angleLimit != 0)
            {
                RotateRight();
            }
        }


        void RotateRight()
        {
            dir = Direction.Right;
            transform.DORotate(new Vector3(0, 0, angleLimit), speed / 60f).OnComplete(RotateStart).SetEase(Ease.Linear);
        }

        void RotateStart()
        {
            if (dir == Direction.Right)
            {
                transform.DORotate(new Vector3(0, 0, 0), speed / 60f).OnComplete(RotateLeft).SetEase(Ease.Linear);
            }
            else
            {
                transform.DORotate(new Vector3(0, 0, 0), speed / 60f).OnComplete(RotateRight).SetEase(Ease.Linear);
            }

        }

        void RotateLeft()
        {
            dir = Direction.Left;
            transform.DORotate(new Vector3(0,0,-angleLimit), speed/60f).OnComplete(RotateStart).SetEase(Ease.Linear);
        }

        public void Update()
        {
            if(angleLimit == 0)
            {
                transform.Rotate(new Vector3(0, 0, Time.deltaTime * speed));
            }
        }
        private void OnDestroy()
        {
            DOTween.KillAll();
        }
    }
}
