using UnityEngine;

public class Trampoline : MonoBehaviour
{
    private const string IDLE_ANIMATION_NAME = "trampoline_idle";
    private const string TRIGGER_ANIMATION_NAME = "trampoline_trigger";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_ANIMATION_NAME)) {
            animator.Play(TRIGGER_ANIMATION_NAME);
        }
    }
}
