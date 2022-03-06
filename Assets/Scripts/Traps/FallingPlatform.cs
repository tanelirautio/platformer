using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace pf
{

    //TODO: Make this more platform controller like, check with raycasts if player is on top of the platform and run the logic based on that information. 
    //Triggering platform via OnTriggerEnter2D and OnTriggerExit2D will lead to bugs (collision from under the platform, player jumping off platform before it falls etc.)

    public class FallingPlatform : MonoBehaviour
    {
        private Animator anim;

        public float fallingHeight = 4.0f;
        public float fallingTime = 0.5f;

        private float startY;

        private enum State
        {
            Up,
            GoingDown,
            Down,
            GoingUp
        }
        private State currentState = State.Up;

        //animation states
        const string FALLING_PLATFORM_ON = "falling_platform_on";
        const string FALLING_PLATFORM_OFF = "falling_platform_off";

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        void Start()
        {
            PlayAnimation(FALLING_PLATFORM_ON);
            startY = transform.position.y;
        }


        void Update()
        {

        }

        public void Trigger(bool fall)
        {
            if(fall && currentState == State.Up)
            {
                Invoke("Fall", Defs.TRAP_FALLING_PLATFORM_WAIT_TIME);
            }
            else if(!fall && currentState == State.Down)
            {
                GoUp();
            }
        }

        private void Fall()
        {
            PlayAnimation(FALLING_PLATFORM_OFF);
            currentState = State.GoingDown;
            transform.DOMoveY(startY - fallingHeight, fallingTime).OnComplete(FallComplete);
        }

        private void FallComplete()
        {
            PlayAnimation(FALLING_PLATFORM_ON);
            currentState = State.Down;
        }

        private void PlayAnimation(string animation)
        {
            anim.Play(animation);
        }

        private void GoUp()
        {
            PlayAnimation(FALLING_PLATFORM_ON);
            transform.DOMoveY(startY, fallingTime).OnComplete(MoveUpComplete);
            currentState = State.GoingUp;
        }

        private void MoveUpComplete()
        {
            currentState = State.Up;
        }
    }

}