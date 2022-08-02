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

        public bool respawn = false;
        private Facing originalFacing = Facing.Left;
        private Vector2 originalPosition;
        private static List<GameObject> respawnableEnemiesInScene = new List<GameObject>();

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
                spriteRenderer.flipX = true;
            }

            originalFacing = facing;
            originalPosition = transform.position;
            if (respawn)
            {
                respawnableEnemiesInScene.Add(this.gameObject);
            }
        }

        public static void Respawn()
        {
            //Debug.Log("Trying to respawn enemies in the scene...");
            foreach (GameObject obj in respawnableEnemiesInScene)
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy.respawn == true)
                {
                    enemy.transform.position = enemy.originalPosition;
                    enemy.facing = enemy.originalFacing;
                }
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

        public static void Uninit()
        {
            respawnableEnemiesInScene.Clear();
        }
    }
}
