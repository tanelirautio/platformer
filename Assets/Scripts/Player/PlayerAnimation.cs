using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class PlayerAnimation : MonoBehaviour
    {
        const float PLAYER_VELOCITY_X_THRESHOLD = 0.5f;
        const float PLAYER_GRACE_PERIOD_FLASH_TIME = 0.25f;

        public Material baseMaterial;
        public Material hitMaterial;
        private Material currentMaterial;

        //animation states
        const string PLAYER_IDLE = "idle";
        const string PLAYER_JUMP = "jump";
        const string PLAYER_RUN = "run";
        const string PLAYER_HIT = "hit";
        const string PLAYER_FALL = "fall";
        const string PLAYER_DEAD = "double_jump";

        private Animator animator;
        private string currentAnimState;
        public RuntimeAnimatorController[] animators = new RuntimeAnimatorController[4];
        public Sprite[] characters = new Sprite[4];

        private Player player;
        private SpriteRenderer spriteRenderer;

        private bool isTakingDamage = false;
        private bool isDead = false;

        private float time = 0;
        private bool changeMaterial = false;

        private Color powerupColor;

        private void Awake()
        {
            player = GetComponent<Player>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            float intensity = 10f;
            float factor = Mathf.Pow(2, intensity);
            powerupColor = new Color(1.0f * factor, 0 * factor, 0.5f * factor);


        }

        void Start()
        {
            print("Selected character: " + PlayerStats.SelectedCharacter);
            animator.runtimeAnimatorController = animators[PlayerStats.SelectedCharacter];
            spriteRenderer.sprite = characters[PlayerStats.SelectedCharacter];

            Reset();
        }

        public void Reset()
        {
            ResetBaseMaterial();
            isTakingDamage = false;
            isDead = false;
        }

        private void ResetBaseMaterial()
        {
            baseMaterial.color = Color.black;
        }

        void Update()
        {
            if (player.isGracePeriod())
            {
                if (changeMaterial)
                {
                    if (currentMaterial != hitMaterial)
                    {
                        spriteRenderer.material = hitMaterial;
                        currentMaterial = hitMaterial;
                    }
                    else
                    {
                        spriteRenderer.material = baseMaterial;
                        currentMaterial = baseMaterial;
                    }
                    changeMaterial = false;
                }

                time += Time.deltaTime;
                if (time > PLAYER_GRACE_PERIOD_FLASH_TIME)
                {
                    changeMaterial = true;
                    time = 0;
                }

            }
            else
            {
                if (currentMaterial != baseMaterial)
                {
                    spriteRenderer.material = baseMaterial;
                    currentMaterial = baseMaterial;
                    time = 0;
                }
            }
        }

        public void Die()
        {
            isDead = true;
        }

        public void TakeDamage()
        {
            isTakingDamage = true;
            changeMaterial = true;
            Invoke("DamageComplete", 0.5f); //cheesy way
        }

        private void DamageComplete()
        {
            isTakingDamage = false;
        }

        public void CollectPowerup(Powerup.Type type)
        {
            switch(type)
            {
                case Powerup.Type.Jump:
                {
                    baseMaterial.color = powerupColor;
                    break;
                }
            }
        } 

        public void PowerupExpired(Powerup.Type type)
        {
            ResetBaseMaterial();
        }

        public void HandleAnimation(Controller2D controller, Vector2 velocity)
        {
            if (isTakingDamage)
            {
                ChangeAnimationState(PLAYER_HIT);
                return;
            }
            if (isDead)
            {
                ChangeAnimationState(PLAYER_DEAD);
                return;
            }

            spriteRenderer.flipX = controller.collisions.faceDir == -1 ? true : false;

            if (controller.collisions.below)
            {
                if (Mathf.Abs(velocity.x) > PLAYER_VELOCITY_X_THRESHOLD)
                {
                    ChangeAnimationState(PLAYER_RUN);
                }
                else
                {
                    ChangeAnimationState(PLAYER_IDLE);
                }
            }
            else
            {
                if (velocity.y > 0)
                {
                    ChangeAnimationState(PLAYER_JUMP);
                }
                else
                {
                    ChangeAnimationState(PLAYER_FALL);
                }
            }
        }

        private void ChangeAnimationState(string newAnimState)
        {
            if (currentAnimState == newAnimState)
            {
                return;
            }

            animator.Play(newAnimState);
            currentAnimState = newAnimState;

        }
    }
}
