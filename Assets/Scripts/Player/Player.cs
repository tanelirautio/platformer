﻿using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

namespace pf
{
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

        private PlayerInputActions playerInputActions;
        private InputAction jumpAction;
        private InputAction movementAction;

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
        private LevelEnd levelEnd;
        private Light2D light2D;
        private AchievementManager achievementManager;

        private bool gracePeriod = false;
        private bool killZoneDamageTaken = false;

        private bool isDead = false;
        private bool hasBeenHit = false;

        private Timer levelCompletionTimer = new Timer();

        private bool controllerDisabled = false;

        struct Powerups
        {
            public bool jumpPowerEnabled;
            public bool speedPowerEnabled;
        }
        private Powerups powerups;

        private void Awake()
        {
            DOTween.Init();

            spawnPoint = GameObject.Find("SpawnPoint");
            controller = GetComponent<Controller2D>();

            //ResetMovement();

            score = GetComponent<PlayerScore>();
            health = GetComponent<PlayerHealth>();
            anim = GetComponent<PlayerAnimation>();
            wallSliding = GetComponent<PlayerWallSliding>();
            light2D = GetComponent<Light2D>();
            //cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
            cameraFollow = Camera.main.GetComponent<CameraFollow>();

            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            uiCanvas = GameObject.Find("UICanvas");
            levelEnd = GameObject.Find("UICanvas/LevelEnd").GetComponent<LevelEnd>();

            achievementManager = GameObject.Find("AchievementManager").GetComponent<AchievementManager>();

            PlayerStats.SceneIndex = LevelLoader.GetCurrentSceneIndex();

#if UNITY_EDITOR
            // Try to load level objectives and achievements when played in Unity editor
            // This way independently played levels can still show them
            DataLoader.ParseData();
#endif
            playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            jumpAction = playerInputActions.PlayerControls.Jump;
            jumpAction.Enable();

            movementAction = playerInputActions.PlayerControls.Movement;
            movementAction.Enable();
        }

        private void OnDisable()
        {
            jumpAction.Disable();
            movementAction.Disable();
        }

        private void Start()
        {
            Spawn();
            SaveSystem.Save();
        }

        private void ResetMovement()
        {
            powerups.jumpPowerEnabled = false;
            movement = new Movement(
                moveSpeed,
                maxJumpHeight,
                timeToJumpApex
            );
        }

        private void Spawn(bool resetHealth = true, bool resetScore = true)
        {

            controllerDisabled = false;
            isDead = false;
            cameraFollow.Reset();
            anim.Reset();
            if (resetHealth)
            {
                health.Reset();
            }
            score.Reset();
            levelCompletionTimer.Reset();
            ResetMovement();
            hasBeenHit = false;
            light2D.enabled = false;

            Assert.IsNotNull(spawnPoint);
            if (spawnPoint)
            {
                transform.position = spawnPoint.transform.position;
                print("Spawning at: " + transform.position);
            }
        }

        private void Update()
        {
            levelCompletionTimer.Update(Time.deltaTime);

            if (!controllerDisabled)
            {
                input = movementAction.ReadValue<Vector2>();
            }
            else
            {
                input.x = 0;
                input.y = 0;

                if (jumpAction.WasPerformedThisFrame() && levelEnd.LevelEndReady())
                {
                    LoadNextScene();
                }
            }

            movement.CalculateVelocityX(input.x, controller.collisions.below ? accTimeGrounded : accTimeAirborne);

            if (controller.collisions.below && jumpAction.WasPressedThisFrame())
            {
                movement.Jump(transform.position.y);
            }

            if (jumpAction.WasReleasedThisFrame())
            {
                movement.DoubleGravity();
            }

            controller.Move(movement.CalculateVelocity(Time.deltaTime, transform.position.y), input);

            // Removes the accumulation of gravity
            if (controller.collisions.above || controller.collisions.below)
            {
                movement.ZeroVelocityY();
            }

            anim.HandleAnimation(controller, movement.Velocity);

            // TODO: kill player if falling away from platform
            if (transform.position.y < stopFollowingPlayerY)
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
            Powerup.Respawn();
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
            Invoke("EndGracePeriod", Defs.GRACE_PERIOD_LENGTH);
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
                hasBeenHit = true;
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

        public void CollectedPowerup(Powerup.Type type)
        {
            switch(type)
            {
                case Powerup.Type.Jump:
                {
                    if (!powerups.jumpPowerEnabled)
                    {
                        movement.SetMaxJumpHeight(maxJumpHeight + Defs.POWERUP_EXTRA_JUMP_POWER);
                        anim.CollectPowerup(type);
                        light2D.enabled = true;
                        light2D.color = Color.red;
                        powerups.jumpPowerEnabled = true;
                    }
                    break;
                }
                case Powerup.Type.Speed:
                {
                    if (!powerups.speedPowerEnabled)
                    {
                        movement.SetSpeed(10f);
                        print("collected speed powerup!");
                    }
                    break;
                }
            }
        }

        public void PowerupExpired(Powerup.Type type)
        {
            switch(type)
            {
                case Powerup.Type.Jump:
                {
                    if (powerups.jumpPowerEnabled)
                    {
                        movement.SetMaxJumpHeight(maxJumpHeight);
                        light2D.enabled = false;
                        powerups.jumpPowerEnabled = false;
                        anim.PowerupExpired(type);
                    }
                    break;
                }
                case Powerup.Type.Speed:
                {
                    print("expire speed powerup");
                    break;
                }
            }
        }

        private void LoadNextScene()
        {
            // TODO: debug, if we reach the last scene, just go to main menu...
            // In the real game show end screen
            if (LevelLoader.GetCurrentSceneIndex() == levelLoader.GetTotalSceneCount()-1)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.Credits);
            }
            else
            {
                // save player score only when transitioning to next level
                print("***** Transition to next level *****");
                print("level score: " + score.GetScore());
                print("current level: " + PlayerStats.GetCurrentLevel());
                if(PlayerStats.BestScores[PlayerStats.GetCurrentLevel()] < score.GetScore())
                {
                    PlayerStats.BestScores[PlayerStats.GetCurrentLevel()] = score.GetScore();
                }


                SaveSystem.Save();

                levelLoader.LoadNextScene();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Trap")
            {
                Trap trap = collision.gameObject.GetComponent<Trap>();
                Trap.Type type;
                if(trap != null)
                {
                    type = trap.type;
                }
                else
                {
                    trap = collision.gameObject.GetComponentInParent<Trap>();
                    type = trap.type;
                }

                if (!isGracePeriod() && !isDead)
                {
                    int currentHealth = health.TakeDamage(type);
                    HandleDamage(currentHealth);
                    if (currentHealth > 0)
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
            if (collision.gameObject.tag == "Finish")
            {
                levelCompletionTimer.Stop();

                print("finished level");
                controllerDisabled = true;

                float timerMs = levelCompletionTimer.Elapsed * 1000.0f;
                levelEnd.ShowLevelEnd(hasBeenHit, score.GetScore(), timerMs);

                StatisticsManager.SetCompletedLevel(PlayerStats.GetCurrentLevel());
                achievementManager.CheckCompletedLevelsAchievement();
                if(!hasBeenHit)
                {
                    StatisticsManager.SetCompletedLevelWithoutHits(PlayerStats.GetCurrentLevel());
                    achievementManager.CheckCompletedLevelsWihtoutHitsAchievement();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "SpawnPoint")
            {
                levelCompletionTimer.Start();
            }
        }
    }
}