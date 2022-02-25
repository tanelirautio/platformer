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

        private State state;
        private float idleTime = 0;
        private float nextBlink = 2f;
        private string currentAnimation;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            state = State.Idle;
        }

        // Start is called before the first frame update
        void Start()
        {
            currentAnimation = SPIKE_HEAD_IDLE;
        }

        // Update is called once per frame
        void Update()
        {
            switch(state)
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
                    break;
                }
            }
        }
    }
}
