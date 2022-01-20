using UnityEngine;

public class Trampoline : MonoBehaviour
{
    private const string IDLE_ANIMATION_NAME = "trampoline_idle";
    private const string TRIGGER_ANIMATION_NAME = "trampoline_trigger";

    private Animator anim;
    private float accumulator;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collided from above
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(IDLE_ANIMATION_NAME)) {
            anim.Play(TRIGGER_ANIMATION_NAME);
        }
    }
}
