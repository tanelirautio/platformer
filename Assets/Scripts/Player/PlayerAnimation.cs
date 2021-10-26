using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    const float PLAYER_VELOCITY_X_THRESHOLD = 5.0f;
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

    private Animator animator;
    private string currentAnimState;
    public RuntimeAnimatorController[] animators = new RuntimeAnimatorController[4];
    public Sprite[] characters = new Sprite[4];

    private Player player;
    private SpriteRenderer spriteRenderer;

    private bool isTakingDamage = false;

    private float time = 0;
    private bool changeMaterial = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        print("Selected character: " + PlayerStats.SelectedCharacter);
        animator.runtimeAnimatorController = animators[PlayerStats.SelectedCharacter];
        spriteRenderer.sprite = characters[PlayerStats.SelectedCharacter];
    }

    void Update()
    {
        if(player.isGracePeriod())
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
            if(time > PLAYER_GRACE_PERIOD_FLASH_TIME)
            {
                changeMaterial = true;
                time = 0;
            }

        }
        else
        {
            if(currentMaterial != baseMaterial)
            {
                spriteRenderer.material = baseMaterial;
                currentMaterial = baseMaterial;
                time = 0;
            }
        }
    }

    public void Die()
    {

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

    public void HandleAnimation(Controller2D controller, Vector2 velocity)
    {
        if(isTakingDamage)
        {
            ChangeAnimationState(PLAYER_HIT);
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
