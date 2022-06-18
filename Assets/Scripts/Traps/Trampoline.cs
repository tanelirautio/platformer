using UnityEngine;

namespace pf
{
    public class Trampoline : MonoBehaviour
    {
        private const string IDLE_ANIMATION_NAME = "trampoline_idle";
        private const string TRIGGER_ANIMATION_NAME = "trampoline_trigger";

        private Animator animator;

        [SerializeField]
        private float jumpForceMultiplier = 1.3f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public float GetForceMultiplier()
        {
            return jumpForceMultiplier;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_ANIMATION_NAME))
            {
                animator.Play(TRIGGER_ANIMATION_NAME);
            }
        }
    }

}
