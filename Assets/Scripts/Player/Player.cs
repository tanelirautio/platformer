using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float timeToJumpApex = 0.4f;

    [Header("Move Settings")]
    [SerializeField] private float accTimeAirborne = 0.2f;
    [SerializeField] private float accTimeGrounded = 0.1f;
    [SerializeField] private float moveSpeed = 6;

    [SerializeField] private float stopFollowingPlayerY = -2f;
    [SerializeField] private float killZoneY = -10f;

    private Controller2D controller;
    private Movement movement;
    private Vector2 input;

    private PlayerScore score;
    private PlayerHealth health;
    private PlayerAnimation anim;
    private PlayerWallSliding wallSliding;
    private GameObject spawnPoint;
    private CameraFollow cameraFollow;
    private LevelLoader levelLoader;
    private GameObject uiCanvas;

    const float GRACE_PERIOD_LENGTH = 2.0f;
    private bool gracePeriod = false;
    private bool killZoneDamageTaken = false;

    const float FADE_SPEED = 1.0f;
    private bool isDead = false;

    private bool controllerDisabled = false;

    private void Awake()
    {
        spawnPoint = GameObject.Find("SpawnPoint");
        controller = GetComponent<Controller2D>();

        movement = new Movement(
            moveSpeed,
            maxJumpHeight,
            timeToJumpApex
        );

        score = GetComponent<PlayerScore>();
        health = GetComponent<PlayerHealth>();
        anim = GetComponent<PlayerAnimation>();
        wallSliding = GetComponent<PlayerWallSliding>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();

        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        uiCanvas = GameObject.Find("UICanvas");

        PlayerStats.CurrentSceneIndex = levelLoader.GetCurrentSceneIndex();
    }

    private void Start()
    {
        Spawn();
    }

    private void Spawn(bool resetHealth = true)
    {

        controllerDisabled = false;
        isDead = false;
        cameraFollow.Reset();
        anim.Reset();
        if (resetHealth)
        {
            health.Reset();
        }
        uiCanvas.SetActive(true);
        if (spawnPoint)
        {
            transform.position = spawnPoint.transform.position;
            print("Spawning at: " + transform.position);
        }
        else
        {
            print("Spawn point not found!");
        }
    }

    private void Update()
    {
        if (!controllerDisabled)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            input.x = 0;
            input.y = 0;
        }

        movement.CalculateVelocityX(input.x, (controller.collisions.below) ? accTimeGrounded : accTimeAirborne);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (controller.collisions.below)
            {
                movement.Jump(transform.position.y);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            movement.DoubleGravity();
        }
 
        controller.Move(movement.CalculateVelocity(Time.deltaTime, transform.position.y));

        // Removes the accumulation of gravity
        if (controller.collisions.above || controller.collisions.below)
        {
            movement.ZeroVelocityY();
        }

        anim.HandleAnimation(controller, movement.Velocity);

        // TODO: kill player if falling away from platform
        if(transform.position.y < stopFollowingPlayerY)
        {
            cameraFollow.StopFollowingPlayer();
        }
        if (transform.position.y < killZoneY)
        {
            if (!killZoneDamageTaken)
            {
                TakeKillZoneDamage();
            }
        }
    }

    public void DamageMove(Vector2 hitPosition)
    {
        Vector3 dir = transform.position - (Vector3)hitPosition;
        Vector3 movePos = transform.position + (dir * 1.1f);
        transform.DOMove(movePos, 0.3f); //TODO remove magic number

        //TODO: disable controller when tween is playing(?)
        //Check if it is needed
    }

    public void DeathMove()
    {
        // TODO: tween movement should arc
        transform.DOMove(transform.position + (-Vector3.up * 3), 1.0f);
        //myTransform.DOMoveX(3, 2).SetEase(Ease.OutQuad);
        //myTransform.DOMoveY(3, 2).SetEase(Ease.InQuad);
    }

    public void TakeKillZoneDamage()
    {
        int currentHealth = health.TakeDamage(Trap.Type.KillZone);
        if (currentHealth > 0)
        {
            FadeToBlack();
            Invoke("SpawnAfterKillZoneDamage", 1.0f);
            Invoke("FadeFromBlack", 1.0f);
        }
        else
        {
            HandleDamage(0);
        }
        killZoneDamageTaken = true;
    }

    private void SpawnAfterKillZoneDamage()
    {
        Spawn(false);
        killZoneDamageTaken = false;
    }

    private void FadeToBlack()
    {
        levelLoader.TriggerTransitionOnly(true);
    }

    private void FadeFromBlack()
    {
        levelLoader.TriggerTransitionOnly(false);
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

    private void HandleDamage(int currentHealth)
    {
        if (currentHealth > 0)
        {
            anim.TakeDamage();
            setGracePeriod();
            print("player hurt!");
        }
        else
        {
            controllerDisabled = true;
            cameraFollow.StopFollowingPlayer();
            anim.Die();
            print("player dead!");

            isDead = true;
            levelLoader.LoadScene((int)LevelLoader.Scenes.Continue);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            Trap.Type type = collision.gameObject.GetComponent<Trap>().type;

            if (!isGracePeriod() && !isDead)
            {
                int currentHealth = health.TakeDamage(type);
                HandleDamage(currentHealth);
                if(currentHealth > 0)
                {
                    DamageMove(collision.transform.position);
                }
                else
                {
                    DeathMove();
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
            controllerDisabled = true;
            uiCanvas.SetActive(false);

            // TODO: debug, if we reach the last scene, just go to main menu...
            if (levelLoader.GetCurrentSceneIndex() == 4)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }
            else
            {
                levelLoader.LoadNextScene();
            }
        }
    }
}