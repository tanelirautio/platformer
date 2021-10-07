using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    const float PLAYER_VELOCITY_X_THRESHOLD = 5.0f;

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

    private SpriteRenderer spriteRenderer;

    private bool isTakingDamage = false;

    private void Awake()
    {
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
        
    }

    public void TakeDamage()
    {
        isTakingDamage = true;
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
