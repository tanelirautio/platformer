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
    /*
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    public float timeToWallUnstick;
    */

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;
    
    Vector2 directionalInput;

    private PlayerScore score;
    private PlayerHealth health;
    private PlayerAnimation anim;
    private PlayerWallSliding wallSliding;

    const float GRACE_PERIOD_LENGTH = 5.0f;
    private bool gracePeriod = false;

    private bool controllerDisabled = false;

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
        wallSliding = GetComponent<PlayerWallSliding>();
    }


    private void Update()
    {
        CalculateVelocity();

        if (wallslideEnabled)
        {
            wallSliding.HandleWallSliding(controller, directionalInput, velocity, velocityXSmoothing);
        }

        if (!controllerDisabled)
        {
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
            wallSliding.OnJumpInputDown(directionalInput, ref velocity);
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


    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accTimeGrounded : accTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void TakeDamage(Vector2 hitPosition)
    {
        Vector3 dir = transform.position - (Vector3)hitPosition;
        Vector3 movePos = transform.position + (dir * 1.1f);
        transform.DOMove(movePos, 0.3f); //TODO remove magic number

        //TODO: disable controller when tween is playing(?)
        //Check if it is needed
    }

    public void setGracePeriod()
    {
        //TODO: notify via anim that player is in grace period (flash to white etc.?)

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