using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private float m_jumpPower;
    [SerializeField] private LayerMask m_groundLayer;
    private Rigidbody2D m_body;
    private Animator m_anim;
    private BoxCollider2D m_boxCollider;
    private float horizontalInput;

    private float wallJumpCooldown;

    private void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
        m_boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // flip player when moving left-right
        if(horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if(horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        m_body.velocity = new Vector2(horizontalInput * m_speed, m_body.velocity.y);

        if (Input.GetKey(KeyCode.Space) && isGrounded())
        {
            Jump();
        }

        //Set animator params
        m_anim.SetBool("run", horizontalInput != 0);
        m_anim.SetBool("grounded", isGrounded());
    }

    private void Jump()
    {
        if (isGrounded())
        {
            m_body.velocity = new Vector2(m_body.velocity.x, m_jumpPower);
            m_anim.SetTrigger("jump");
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(m_boxCollider.bounds.center, m_boxCollider.bounds.size, 0, Vector2.down, 0.1f, m_groundLayer);
        return raycastHit.collider != null;
    }   
    
}
