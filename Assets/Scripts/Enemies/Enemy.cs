using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace pf
{
    public class Enemy : MonoBehaviour
    {
        public enum Type
        {
            Plant,
        }

        public Type type;

        public enum Facing
        {
            Left,
            Right,
        }

        public Facing facing;

        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }
}
