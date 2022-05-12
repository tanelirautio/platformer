using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class PlayerAnimation : MonoBehaviour
    {
        public ParticleSystem dust;

        //const float PLAYER_VELOCITY_X_THRESHOLD = 0.5f;
        //const float PLAYER_GRACE_PERIOD_FLASH_TIME = 0.25f;

        public Material baseMaterial;
        //public Material hitMaterial;
        private Material currentMaterial;

        //animation states
        const string PLAYER_IDLE = "idle";
        const string PLAYER_JUMP = "jump";
        const string PLAYER_RUN = "run";
        //const string PLAYER_HIT = "hit";
        const string PLAYER_FALL = "fall";
        const string PLAYER_DEAD = "double_jump";
        const string PLAYER_APPEAR = "appear";
        const string PLAYER_DISAPPEAR = "disappear";
        const string PLAYER_INVISIBLE = "invisible";

        private Animator animator;
        private string currentAnimState;
        public RuntimeAnimatorController[] animators = new RuntimeAnimatorController[4];
        public Sprite[] characters = new Sprite[4];

        private Player player;
        private SpriteRenderer spriteRenderer;

        public enum State
        {
            Visible,
            Appearing,
            Disappearing,
            Invisible,
            Dead
        }
        private State currentState = State.Visible;
        private State oldState = State.Visible;

        private int previousFaceDir = 1;

        private Color defaultColor = Color.white;
        [ColorUsage(true, true)] public Color jumpColor;
        [ColorUsage(true, true)] public Color speedColor;

        private Dictionary<Powerup.Type, Color> powerupColors;
        private void Awake()
        {
            player = GetComponent<Player>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            powerupColors = new Dictionary<Powerup.Type, Color>()
            {
                { Powerup.Type.Jump, jumpColor },
                { Powerup.Type.Speed, speedColor }
            };
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
            currentState = State.Visible;
        }

        private void ResetBaseMaterial()
        {
            baseMaterial.color = defaultColor;
        }

        void Update()
        {
        }

        public void Die()
        {
            print("******** PlayerAnimation: DIE ***********");
            currentState = State.Dead;
            ChangeAnimationState(PLAYER_DEAD);
        }

        public void Disappear()
        {
            currentState = State.Disappearing;
        }

        public void Appear()
        {
            currentState = State.Appearing;
        }

        public void CollectPowerup(Powerup.Type type)
        {
            switch(type)
            {
                case Powerup.Type.Jump:
                {
                    baseMaterial.color = powerupColors[Powerup.Type.Jump];
                    break;
                }
                case Powerup.Type.Speed:
                {
                    baseMaterial.color = powerupColors[Powerup.Type.Speed];
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
            if(currentState != oldState)
            {
                Debug.Log("Current state: " + currentState);
                oldState = currentState;
            }

            if(currentState == State.Disappearing)
            {
                if (currentAnimState == PLAYER_DISAPPEAR)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {
                        Debug.Log("### Player is INVISIBLE ###");
                        Debug.Log("### DISABLE SPRITERENDERER ###");
                        //spriteRenderer.enabled = false;
                        currentState = State.Invisible;
                    }
                }
                else
                {
                    ChangeAnimationState(PLAYER_DISAPPEAR);
                }
                return;
            }

            if(currentState == State.Appearing)
            {             
                if (currentAnimState == PLAYER_APPEAR)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {             
                        Debug.Log("### Player is VISIBLE ###");
                        player.TeleportAnimationDone();
                        currentState = State.Visible;
                    }
                }
                else
                {
                    ChangeAnimationState(PLAYER_APPEAR);
                    Debug.Log("### ENABLE SPRITERENDERER ###");
                    Debug.Log("### Player starts to APPEAR ###");
                    //spriteRenderer.enabled = true;   
                }
                return;
            }

            if(currentState == State.Invisible)
            {
                ChangeAnimationState(PLAYER_INVISIBLE);
                return;
            }

            spriteRenderer.flipX = controller.collisions.faceDir == -1 ? true : false;
            if(previousFaceDir != controller.collisions.faceDir)
            {
                CreateDust();
                previousFaceDir = controller.collisions.faceDir;
            }

            if (controller.collisions.below)
            {
                if (Mathf.Abs(velocity.x) > Defs.PLAYER_VELOCITY_X_THRESHOLD)
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

            print("Changing animation state to: " + newAnimState);

            if(newAnimState == PLAYER_RUN || newAnimState == PLAYER_JUMP)
            {
                CreateDust();
            }

            animator.Play(newAnimState);
            currentAnimState = newAnimState;

        }

        private void CreateDust()
        {
            dust.Play();
        }

        public State GetCurrentState()
        {
            return currentState;
        }
    }
}
