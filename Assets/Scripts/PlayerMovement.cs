using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    private bool m_grounded;

    private Rigidbody2D m_body;
    private Animator m_anim;

    private void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        m_body.velocity = new Vector2(horizontalInput * m_speed, m_body.velocity.y);  

        // flip player when moving left-right
        if(horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if(horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if(Input.GetKey(KeyCode.Space) && m_grounded)
        {
            Jump();
        }

        //Set animator params
        m_anim.SetBool("run", horizontalInput != 0);
        m_anim.SetBool("grounded", m_grounded);
    }

    private void Jump()
    {
        m_body.velocity = new Vector2(m_body.velocity.x, m_speed);
        m_anim.SetTrigger("jump");
        m_grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            m_grounded = true;
        }
    }
}
