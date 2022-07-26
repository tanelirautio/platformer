using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using System.Collections;

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

        private PlayerInputActions playerInputActions;
        private InputAction jumpAction;
        private InputAction movementAction;
        private InputAction pauseAction;

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
        private PauseGame pauseGame;
        private Light2D light2D;
        private float light2DBaseIntensity;
        private AchievementManager achievementManager;
        private CheckpointManager checkpointManager;
        private AudioManager audioManager;
        private Music music;

        private bool gracePeriod = false;

        private bool isInvulnerable = false;
        public bool IsTeleporting { get; internal set; }
        public bool IsDead { get; internal set; }
        //private bool isDead = false;
        //private bool trampolineJump = false;

        private struct TrampolineJump
        {
            public bool IsJumping { get; set; }
            public float Multiplier { get; set; }
        }
        private TrampolineJump trampolineJump;

        private Timer levelCompletionTimer = new Timer();

        private bool controllerDisabled = false;
        private bool paused = false;

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

            score = GetComponent<PlayerScore>();
            health = GetComponent<PlayerHealth>();
            anim = GetComponent<PlayerAnimation>();
            wallSliding = GetComponent<PlayerWallSliding>();
            light2D = GetComponent<Light2D>();
            light2DBaseIntensity = light2D.intensity;
            cameraFollow = Camera.main.GetComponent<CameraFollow>();

            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            uiCanvas = GameObject.Find("UICanvas");
            levelEnd = GameObject.Find("UICanvas/LevelEnd").GetComponent<LevelEnd>();
            pauseGame = GameObject.Find("UICanvas/PauseMenu").GetComponent<PauseGame>();

            achievementManager = GameObject.Find("AchievementManager").GetComponent<AchievementManager>();
            
            if(GameObject.Find("CheckpointManager"))
            {
                checkpointManager = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
            }

            if (GameObject.Find("AudioSystem"))
            {
                music = GameObject.Find("AudioSystem").GetComponent<Music>();
                audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();
            }

            PlayerStats.SceneIndex = LevelLoader.GetCurrentSceneIndex();

#if UNITY_EDITOR
            // Try to load level objectives and achievements when played in Unity editor
            // This way independently played levels can still show them
            DataLoader.ParseData();
#endif
            playerInputActions = new PlayerInputActions();

            trampolineJump.IsJumping = false;
            trampolineJump.Multiplier = 0;
        }

        private void OnEnable()
        {
            jumpAction = playerInputActions.PlayerControls.Jump;
            jumpAction.Enable();

            movementAction = playerInputActions.PlayerControls.Movement;
            movementAction.Enable();

            pauseAction = playerInputActions.PlayerControls.Pause;
            pauseAction.Enable();
        }

        private void OnDisable()
        {
            jumpAction.Disable();
            movementAction.Disable();
            pauseAction.Disable();
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }

        private void Start()
        {
            Spawn();
            SaveSystem.Save();
            if(music)
            {
                music.Play(LevelLoader.GetCurrentSceneName());
            }
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
            SpawnCommon();
            if (resetHealth)
            {
                health.Reset();
            }
            if (resetScore)
            {
                score.Reset();
            }
            levelCompletionTimer.Reset();
            Assert.IsNotNull(spawnPoint);
            if (spawnPoint)
            {
                transform.position = spawnPoint.transform.position;
                //print("Spawning at: " + transform.position);
            }
        }

        public IEnumerator SpawnAtCheckpoint(float time, Transform checkpoint)
        {
            yield return new WaitForSeconds(time);
            SpawnCommon();
            Powerup.Respawn();
            Assert.IsNotNull(checkpoint);
            if (checkpoint)
            {
                transform.position = checkpoint.position;
                //print("Spawning at: " + transform.position);
            }
        }

        private void SpawnCommon()
        {
            controllerDisabled = false;
            IsDead = false;
            cameraFollow.Reset();
            anim.Reset();

            ResetMovement();
            light2D.enabled = false;

            pauseGame.gameObject.SetActive(false);
        }

        private bool CheckPause()
        {

            //if ((Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame) || 
            //    (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) && 
            //    !pauseGame.Paused)
            if (pauseAction.WasPerformedThisFrame() && !pauseGame.Paused)
            {
                controllerDisabled = true;
                Time.timeScale = 0;
                pauseGame.gameObject.SetActive(true);
                pauseGame.ShowPause();
                return true;
            }

            if (pauseGame.Paused)
            {
                return true;
            }
            else if (pauseGame.ContinuedFromPause)
            {
                //print("Continuing from pause game");
                controllerDisabled = false;
                pauseGame.ContinuedFromPause = false;
                pauseGame.gameObject.SetActive(false);
                return true;
            }
            return false;
        }

        private void Update()
        {
            if(CheckPause())
            {
                return;
            }

            levelCompletionTimer.Update(Time.deltaTime);

            if (!controllerDisabled)
            {
                input = movementAction.ReadValue<Vector2>();
            }
            else
            {
                input.x = 0;
                input.y = 0;

                if(jumpAction.WasPerformedThisFrame())
                {
                    if(levelEnd.LevelEndReady())
                    {
                        print("*** level end - load next scene ***");
                        levelEnd.gameObject.SetActive(false);
                        LoadNextScene();
                    }
                }

                return;
            }

            if(IsTeleporting == true)
            {
                input.x = 0;
                input.y = 0;
            }

            movement.CalculateVelocityX(input.x, controller.collisions.below ? accTimeGrounded : accTimeAirborne);

            if(trampolineJump.IsJumping)
            {
                movement.TrampolineJump(transform.position.y, trampolineJump.Multiplier);
                trampolineJump.IsJumping = false;
                trampolineJump.Multiplier = 0;
                if (audioManager != null)
                {
                    audioManager.PlaySound2D("Jump");
                }
                return;
            }

            if (IsTeleporting == false)
            {

                if (controller.collisions.below && jumpAction.WasPressedThisFrame())
                {
                    movement.Jump(transform.position.y);
                    if (audioManager != null)
                    {
                        audioManager.PlaySound2D("Jump");
                    }
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
            }
            
            anim.HandleAnimation(controller, movement.Velocity);
        }

        /*
        public void DamageMove(Vector2 hitPosition)
        {
            Vector3 dir = transform.position - (Vector3)hitPosition;
            Vector3 movePos = transform.position + (dir * 1.1f);
            transform.DOMove(movePos, 0.3f); //TODO remove magic number

            //TODO: disable controller when tween is playing(?)
            //Check if it is needed
        }*/

        public void DeathMove()
        {
            // TODO: tween movement should arc?
            transform.DOMove(transform.position + (-Vector3.up * 10), 2.0f);


            //myTransform.DOMoveX(3, 2).SetEase(Ease.OutQuad);
            //myTransform.DOMoveY(3, 2).SetEase(Ease.InQuad);
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
            Invoke("EndGracePeriod", Defs.PLAYER_GRACE_PERIOD_LENGTH);
        }

        public bool isGracePeriod()
        {
            return gracePeriod;
        }

        private void EndGracePeriod()
        {
            gracePeriod = false;
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
                        powerups.jumpPowerEnabled = true;
                        light2D.enabled = true;
                        light2D.color = new Color(1f, 0f, 0f);
                        light2D.intensity = light2DBaseIntensity + 0.6f;
                    }
                    break;
                }
                case Powerup.Type.Speed:
                {
                    if (!powerups.speedPowerEnabled)
                    {
                        movement.SetSpeed(moveSpeed + Defs.POWERUP_EXTRA_SPEED);
                        powerups.speedPowerEnabled = true;
                        light2D.enabled = true;
                        light2D.color = new Color(0f, 0f, 1f);
                        light2D.intensity = light2DBaseIntensity + 0.6f;
                    }
                    break;
                }
            }
            anim.CollectPowerup(type);
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
                    //print("expire speed powerup");
                    break;
                }
            }
        }

        private void LoadNextScene()
        {
#if UNITY_EDITOR
            // For easier debugging
            if(LevelLoader.GetCurrentSceneIndex() == (int)LevelLoader.Scenes.TestLevel)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.TestLevel);
                return;
            }
#endif

            // TODO: debug, if we reach the last scene, just go to main menu...
            // In the real game show end screen
            if (PlayerStats.GetCurrentLevel() == Defs.LEVEL_AMOUNT-1)
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.Credits);
            }
            else
            {
                // save player score only when transitioning to next level
                //print("***** Transition to next level *****");
                //print("level score: " + score.GetScore());
                //print("current level: " + PlayerStats.GetCurrentLevel());
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
            if(IsDead)
            {
                return;
            }

            if (collision.gameObject.tag == "Trap")
            {
                Trap trap = collision.gameObject.GetComponent<Trap>();
                Trap.Type type;

                if (trap != null)
                {
                    type = trap.type;
                }
                else
                {
                    trap = collision.gameObject.GetComponentInParent<Trap>();
                    type = trap.type;
                }
                Debug.Log("Collision with " + trap.type + " Object name: " + trap.gameObject.name);

                if (isInvulnerable)
                {
                    return;
                }

                if (type == Trap.Type.SpikeHead)
                {
                    SpikeHead spikeHead = collision.gameObject.GetComponent<SpikeHead>();
                    if (spikeHead)
                    {
                        spikeHead.Collide(collision);
                    }
                }
                
                if (!IsDead && type != Trap.Type.FallingPlatform && type != Trap.Type.RockHead)
                {
                    int currentHealth = health.TakeDamage(type);
                    anim.Die();
                    IsDead = true;
                    controllerDisabled = true;

                    if (currentHealth > 0)
                    {
                        setGracePeriod();
                        audioManager.PlaySound2D("Hit");
                        DeathMove();
                        Invoke("FadeToBlack", 1.0f);

                        Transform currentCheckpoint = null;
                        currentCheckpoint = checkpointManager.GetLatest();
                        
                        StartCoroutine(SpawnAtCheckpoint(Defs.PLAYER_RESPAWN_TIME, currentCheckpoint));
                        Invoke("FadeFromBlack", Defs.PLAYER_RESPAWN_TIME);
                    }
                    else
                    {
                        DeathMove();
                        levelLoader.LoadScene((int)LevelLoader.Scenes.Continue);
                    }
                }
            }
        }

        private void PlayTrophySound()
        {
            audioManager.PlaySound2D("Trophy");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(IsDead)
            {
                return;
            }

            if (collision.gameObject.tag == "Finish")
            {
                levelCompletionTimer.Stop();

                music.StopFade(0.5f);
                Invoke("PlayTrophySound", 0.5f);
                //audioManager.PlaySound2D("Trophy");

                controllerDisabled = true;
                anim.Stop();

                float timerMs = levelCompletionTimer.Elapsed * 1000.0f;
                levelEnd.ShowLevelEnd(health.Hits(), score.GetScore(), timerMs, health.Health());

                if (LevelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.TestLevel)
                {
                    if (LevelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.TestLevel)
                    {
                        StatisticsManager.SetCompletedLevel(PlayerStats.GetCurrentLevel());
                    }
                    achievementManager.CheckCompletedLevelsAchievement();
                    if (health.Hits() == 0)
                    {
                        StatisticsManager.SetCompletedLevelWithoutHits(PlayerStats.GetCurrentLevel());
                        achievementManager.CheckCompletedLevelsWihtoutHitsAchievement();
                    }
                }
            }
            else if(collision.gameObject.tag == "Checkpoint")
            {
                Checkpoint c = collision.gameObject.GetComponent<Checkpoint>();
                if(c)
                {
                    c.PlayerTouch();
                }
            }
            else if(collision.gameObject.tag == "Trap")
            {
                Trap trap = collision.gameObject.GetComponent<Trap>();
                if(trap && trap.type == Trap.Type.RockHead)
                {
                    RockHead rockHead = collision.gameObject.GetComponent<RockHead>();
                    if (rockHead)
                    {
                        rockHead.PlayerTouch();
                    }
                }
            }
            else if(collision.gameObject.tag == "Teleport")
            {
                Teleport teleport = collision.gameObject.GetComponent<Teleport>();
                if(teleport && !teleport.Activated)
                {
                    isInvulnerable = true;
                    IsTeleporting = true;
                    anim.Disappear();
                    teleport.Activate();
                    //Debug.Log("*** Moving player to: " + teleport.GetTargetPosition());        
                    StartCoroutine(Teleport(teleport));
                }
            }
            else if(collision.gameObject.tag == "Trampoline")
            {
                print("**** TRAMPOLINE! *******");
               

                Trampoline t = collision.gameObject.GetComponent<Trampoline>();
                if (t != null) {
                    trampolineJump.IsJumping = true;
                    trampolineJump.Multiplier = t.GetForceMultiplier();
                }

            }
        }


        private IEnumerator Teleport(Teleport teleport)
        {
            //Debug.Log("*** Teleport() ***");
            yield return new WaitForSeconds(1f);

            transform.DOMove(new Vector3(teleport.GetTargetPosition().x, teleport.GetTargetPosition().y, 0), Defs.TELEPORT_TIME); 
            teleport.DestroyOrReactivate();
            Invoke("TeleportAppear", Defs.TELEPORT_TIME);
        }

        private void TeleportAppear()
        {
            //Debug.Log("*** TeleportAppear() ***");
            anim.Appear();
        }

        public void TeleportAnimationDone()
        {
            //Debug.Log("*** TeleportAnimationDone() ***");
            IsTeleporting = false;
            isInvulnerable = false;
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