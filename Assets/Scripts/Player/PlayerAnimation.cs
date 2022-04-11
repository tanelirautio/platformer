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

        private bool isAppearing = false;
        private bool isDisappearing = false;
        private bool isInvisible = false;
        private bool isTakingDamage = false;
        private bool isDead = false;

        private int previousFaceDir = 1;

        private float flashTimer = 0;
        private float[] flashTimes = new float[Defs.PLAYER_GRACE_PERIOD_FLASH_AMOUNT];

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

            float flashTimeOffset = (Defs.PLAYER_GRACE_PERIOD_LENGTH - Defs.PLAYER_GRACE_PERIOD_OFFSET) / (float)Defs.PLAYER_GRACE_PERIOD_FLASH_AMOUNT;
            for(int i=0; i < Defs.PLAYER_GRACE_PERIOD_FLASH_AMOUNT; i++)
            {
                flashTimes[i] = Defs.PLAYER_GRACE_PERIOD_OFFSET + flashTimeOffset * i;
                Debug.Log(flashTimes[i]);
            }
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
            baseMaterial.color = defaultColor;
        }

        void Update()
        {
            if(player.isGracePeriod())
            {
                flashTimer += Time.deltaTime;

                int f = 0;
                for(int i=0; i < flashTimes.Length-1; i++)
                {
                    if(flashTimer < flashTimes[0] || flashTimer > flashTimes[i] && flashTimer < flashTimes[i+1])
                    {
                        break;
                    }
                    f++;
                }

                if(f % 2 == 0)
                {
                    if (!spriteRenderer.enabled)
                    {
                        //Debug.Log("BlinkTimer: " + flashTimer);
                        //Debug.Log("f: " + f + " - show player");
                        spriteRenderer.enabled = true;
                    }
                }
                else
                {
                    if (spriteRenderer.enabled)
                    {
                        //Debug.Log("BlinkTimer: " + flashTimer);
                        //Debug.Log("f: " + f + " - hide player");
                        spriteRenderer.enabled = false;
                    }
                }
            }
            else
            {
                //Debug.Log("*** Grace period over ***");
                flashTimer = 0f;
                spriteRenderer.enabled = true;
            }
        }

        public void Die()
        {
            isDead = true;
        }

        public void TakeDamage()
        {
            isTakingDamage = true;
            //changeMaterial = true;
            Invoke("DamageComplete", 0.5f); //cheesy way
        }

        public void Disappear()
        {
            isDisappearing = true;
        }

        public void Appear()
        {
            isAppearing = true;
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
            if(isDisappearing)
            {
                if (currentAnimState == PLAYER_DISAPPEAR)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {
                        isDisappearing = false;
                        ChangeAnimationState(PLAYER_INVISIBLE);
                        isInvisible = true;
                    }
                }
                else
                {
                    ChangeAnimationState(PLAYER_DISAPPEAR);
                }
                return;
            }

            if(isAppearing)
            {
                if (currentAnimState == PLAYER_APPEAR)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {
                        Debug.Log("Should call player.TeleportAnimationDone()");
                        isAppearing = false;
                        player.TeleportAnimationDone();
                    }
                }
                else
                {
                    ChangeAnimationState(PLAYER_APPEAR);
                    isInvisible = false;
                }
                return;
            }

            if(isInvisible)
            {
                //ChangeAnimationState(PLAYER_INVISIBLE);
                return;
            }

            if (isDead)
            {
                ChangeAnimationState(PLAYER_DEAD);
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
    }
}
