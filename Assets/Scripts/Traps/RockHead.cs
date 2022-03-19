using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class RockHead : MonoBehaviour
    {
        private Animator anim;

        //animation states
        const string ROCK_HEAD_IDLE = "rock_head_idle";
        const string ROCK_HEAD_BLINK = "rock_head_blink";
        const string ROCK_HEAD_BOTTOM_HIT = "rock_head_bottom_hit";
        const string ROCK_HEAD_TOP_HIT = "rock_head_top_hit";
        const string ROCK_HEAD_LEFT_HIT = "rock_head_left_hit";
        const string ROCK_HEAD_RIGHT_HIT = "rock_head_right_hit";

        private State state;
        private string currentAnimation;

        private enum State
        {
            Idle,
            PlayerTouch,
            Hit
        }
        private void Awake()
        {
            anim = GetComponent<Animator>();
            state = State.Idle;
        }

        private void Start()
        {
            currentAnimation = ROCK_HEAD_IDLE;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Idle:
                {
                    if (currentAnimation != ROCK_HEAD_IDLE)
                    {
                        currentAnimation = ROCK_HEAD_IDLE;
                        anim.Play(currentAnimation);
                    }
                    break;
                }
                case State.PlayerTouch:
                {
                    if (currentAnimation != ROCK_HEAD_BLINK)
                    {
                        currentAnimation = ROCK_HEAD_BLINK;
                        anim.Play(currentAnimation);
                    }
                    else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
                    {
                        state = State.Idle;
                    }
                    break;
                }
            }
        }

        public void PlayerTouch()
        {
            Debug.Log("Player Touch!");
            state = State.PlayerTouch;
        }
    }
}
