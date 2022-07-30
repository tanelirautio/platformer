using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace pf
{
    public class Enemy : MonoBehaviour
    {
        private string currentAnimState;

        public enum Type
        {
            Plant,
            Chicken,
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
        protected AudioManager audioManager;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            if (GameObject.Find("AudioSystem"))
            {
                audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();
            }

            if (facing == Facing.Right)
            {
                print("flip spriterenderer x");
                spriteRenderer.flipX = true;
            }
        }

        protected void ChangeAnimState(string newAnimState)
        {
            if (currentAnimState == newAnimState)
            {
                return;
            }

            animator.Play(newAnimState);
            currentAnimState = newAnimState;
        }

        public string GetCurrentAnimState()
        {
            return currentAnimState;
        }

        /*
        void Start()
        {

        }

        void Update()
        {

        }
        */
    }
}
