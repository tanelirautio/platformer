using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Checkpoint : MonoBehaviour
    {
        private Animator anim;
        private AudioManager audioManager;

        //animation states
        const string CHECKPOINT_IDLE = "checkpoint_idle";
        const string CHECKPOINT_FLAG_OUT = "checkpoint_flag_out";
        const string CHECKPOINT_FLAG_IDLE = "checkpoint_flag_idle";

        private enum State
        {
            Idle,
            PlayerTouch,
            FlagOut
        }

        private State state;
        private string currentAnimation;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();
            state = State.Idle;
        }

        // Start is called before the first frame update
        void Start()
        {
            currentAnimation = CHECKPOINT_IDLE;
        }

        // Update is called once per frame
        void Update()
        {
            switch(state)
            {
                case State.PlayerTouch:
                {
                    if (currentAnimation != CHECKPOINT_FLAG_OUT)
                    {
                        currentAnimation = CHECKPOINT_FLAG_OUT;
                    }
                    else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
                    {
                        state = State.FlagOut;
                    }
                    break;
                }
                case State.FlagOut:
                {
                    if(currentAnimation != CHECKPOINT_FLAG_IDLE)
                    {
                        currentAnimation = CHECKPOINT_FLAG_IDLE;
                    }
                    break;
                }
            }
            anim.Play(currentAnimation);
        }

        public void PlayerTouch()
        {
            if (state == State.Idle)
            {
                state = State.PlayerTouch;
                audioManager.PlaySound2D("Checkpoint");
            }
        }

        public bool IsTriggered()
        {
            return state != State.Idle;
        }
    }

}