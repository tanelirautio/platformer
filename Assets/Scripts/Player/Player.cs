using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    [Header("Jump Settings")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;

    [Header("Move Settings")]
    [SerializeField] private float accTimeAirborne = 0.2f;
    [SerializeField] private float accTimeGrounded = 0.1f;
    [SerializeField] private float moveSpeed = 6;

    [Header("Wall Jump Settings")]
    public bool wallslideEnabled = false;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    public float timeToWallUnstick;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;
    
    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    private PlayerScore score;
    private PlayerHealth health;
    private PlayerAnimation anim;

    const float GRACE_PERIOD_LENGTH = 0.5f;
    private bool gracePeriod = false;

    private void Awake()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);

        score = GetComponent<PlayerScore>();
        health = GetComponent<PlayerHealth>();
        anim = GetComponent<PlayerAnimation>();
    }


    private void Update()
    {
        CalculateVelocity();

        if (wallslideEnabled)
        {
            HandleWallSliding();
        }

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        anim.HandleAnimation(controller, velocity);

    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;

            }
        }
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if(directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                {
                    //not jumping agains max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accTimeGrounded : accTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public GameObject test;

    public void TakeDamage(Vector2 hitPosition)
    {
        Vector3 dir = transform.position - (Vector3)hitPosition;

        print("Hit position: " + hitPosition);
        print("Player position: " + transform.position);
        print("Dir: " + dir);

        Vector3 movePos = transform.position + (dir * 1.1f);
        print("Move player to position: " + movePos);
        GameObject.Instantiate(test, movePos, Quaternion.identity);
        transform.DOMove(movePos, 0.3f); //.SetEase(Ease.OutElastic);

        //TODO: disable controller for tween time 
    }

    public void setGracePeriod()
    {
        gracePeriod = true;
        Invoke("EndGracePeriod", GRACE_PERIOD_LENGTH);
    }

    public bool isGracePeriod()
    {
        return gracePeriod;
    }

    private void EndGracePeriod()
    {
        gracePeriod = false;
    }
}