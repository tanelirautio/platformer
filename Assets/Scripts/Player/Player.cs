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
    private GameObject spawnPoint;
    private UIController uiController;
    private CameraFollow cameraFollow;

    const float GRACE_PERIOD_LENGTH = 2.0f;
    private bool gracePeriod = false;

    const float FADE_SPEED = 1.0f;
    private bool isDead = false;

    private bool controllerDisabled = false;

    private void Awake()
    {
        spawnPoint = GameObject.Find("SpawnPoint");
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);

        score = GetComponent<PlayerScore>();
        health = GetComponent<PlayerHealth>();
        anim = GetComponent<PlayerAnimation>();
        wallSliding = GetComponent<PlayerWallSliding>();
        uiController = GameObject.Find("UICanvas").GetComponent<UIController>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    }

    private void Start()
    {
        uiController.FadeImmediately(true);
        Spawn();
    }

    private void Spawn()
    {
        controllerDisabled = false;
        isDead = false;
        anim.Reset();
        health.Reset();
        uiController.Fade(false, FADE_SPEED);
        if (spawnPoint)
        {
            transform.position = spawnPoint.transform.position;
            print("Spawning at: " + transform.position);
        }
    }

    private void Update()
    {
        CalculateVelocity();

        if (!controllerDisabled)
        {
            if (wallslideEnabled)
            {
                wallSliding.HandleWallSliding(controller, directionalInput, velocity, velocityXSmoothing);
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
        }
        else
        {
            controller.collisions.Reset();
        }

        anim.HandleAnimation(controller, velocity);

    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if(controllerDisabled)
        {
            return;
        }

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            print("hit player");
            if (!isGracePeriod() && !isDead)
            {
                int currentHealth = health.TakeDamage(Trap.Type.SPIKE); //TODO: Query the Trap type

                if (currentHealth > 0)
                {
                    anim.TakeDamage();
                    TakeDamage(collision.transform.position);
                    setGracePeriod();
                    print("player hurt!");
                }
                else
                {
                    controllerDisabled = true;
                    cameraFollow.StopFollowingPlayer();
                    anim.Die();
                    print("player dead!");

                    // TODO: tween movement should arc
                    transform.DOMove(transform.position + (-Vector3.up * 3), 1.0f);
                    //myTransform.DOMoveX(3, 2).SetEase(Ease.OutQuad);
                    //myTransform.DOMoveY(3, 2).SetEase(Ease.InQuad);

                    isDead = true;
                    uiController.Fade(true, FADE_SPEED);

                    //TODO: Ask do you want to continue, do not spawn directly!
                    Invoke("Spawn", FADE_SPEED * 2);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Finish")
        {
            // TODO: level finish - fade to black, load next level (or level menu)
            print("finish level");
        }
    }
}