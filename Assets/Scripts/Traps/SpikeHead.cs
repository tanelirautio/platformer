using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class SpikeHead : MonoBehaviour
    {
        private Animator anim;

        //animation states
        const string SPIKE_HEAD_IDLE = "spike_head_idle";
        const string SPIKE_HEAD_BLINK = "spike_head_blink";
        const string SPIKE_HEAD_BOTTOM_HIT = "spike_head_bottom_hit";
        const string SPIKE_HEAD_TOP_HIT = "spike_head_top_hit";
        const string SPIKE_HEAD_LEFT_HIT = "spike_head_left_hit";
        const string SPIKE_HEAD_RIGHT_HIT = "spike_head_right_hit";

        private enum State
        {
            Idle,
            Hit
        }

        private enum HitDirection
        {
            Top,
            Bottom,
            Left,
            Right,
            None
        }

        public BoxCollider2D top;
        public BoxCollider2D bottom;
        public BoxCollider2D left;
        public BoxCollider2D right;

        private State state;
        private HitDirection hitDirection;
        private float idleTime = 0;
        private float nextBlink = 2f;
        private string currentAnimation;
        private bool runningHitAnimation = false;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            state = State.Idle;
            hitDirection = HitDirection.None;
        }

        // Start is called before the first frame update
        void Start()
        {
            currentAnimation = SPIKE_HEAD_IDLE;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Idle:
                {
                    if (currentAnimation == SPIKE_HEAD_IDLE)
                    {
                        idleTime += Time.deltaTime;
                        if (idleTime >= nextBlink)
                        {
                            currentAnimation = SPIKE_HEAD_BLINK;
                            idleTime = 0;
                            nextBlink = Random.Range(1f, 4f);
                        }
                    }
                    else
                    {
                        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
                        {
                            currentAnimation = SPIKE_HEAD_IDLE;
                        }
                    }

                    anim.Play(currentAnimation);
                    break;
                }
                case State.Hit:
                {
                    if (!runningHitAnimation)
                    {
                        switch (hitDirection)
                        {
                            case HitDirection.Top:
                            {
                                currentAnimation = SPIKE_HEAD_TOP_HIT;
                                break;
                            }
                            case HitDirection.Bottom:
                            {
                                currentAnimation = SPIKE_HEAD_BOTTOM_HIT;
                                break;
                            }
                            case HitDirection.Left:
                            {
                                currentAnimation = SPIKE_HEAD_LEFT_HIT;
                                break;
                            }
                            case HitDirection.Right:
                            {
                                currentAnimation = SPIKE_HEAD_RIGHT_HIT;
                                break;
                            }
                        }
                        runningHitAnimation = true;
                    }
                    else
                    {
                        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
                        {
                            currentAnimation = SPIKE_HEAD_IDLE;
                            state = State.Idle;
                            hitDirection = HitDirection.None;
                            runningHitAnimation = false;
                        }
                    }

                    anim.Play(currentAnimation);
                    break;
                }
            }
        }

        public void Collide(Collider2D c)
        {     
            state = State.Hit;
            if(c == top)
            {
                //Debug.Log("TOP HIT");
                hitDirection = HitDirection.Top;
            }
            else if(c == bottom)
            {
                //Debug.Log("BOTTOM HIT");
                hitDirection = HitDirection.Bottom;
            }
            else if(c == left)
            {
                //Debug.Log("LEFT HIT");
                hitDirection = HitDirection.Left;
            }
            else if(c == right)
            {
                //Debug.Log("RIGHT HIT");
                hitDirection = HitDirection.Right;
            }
            else
            {
                state = State.Idle;
                hitDirection = HitDirection.None;
            }
        }
    }
}
