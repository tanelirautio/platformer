using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace pf
{
    public class TweenObject : MonoBehaviour
    {
        private Vector3 start;
        [SerializeField] private Vector3 end;
        public DG.Tweening.Ease ease;
        public float time;

        // Start is called before the first frame update
        void Start()
        {
            transform.DOMove(transform.position + end, time).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        }

        private void OnDrawGizmos()
         {
                Gizmos.color = Color.blue;
                float size = 0.3f;
                Vector3 endPointPos = end + transform.position;
                Gizmos.DrawLine(endPointPos - Vector3.up * size, endPointPos + Vector3.up * size);
                Gizmos.DrawLine(endPointPos - Vector3.left * size, endPointPos + Vector3.left * size);
         }
    }
}
