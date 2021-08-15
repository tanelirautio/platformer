using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    private Rigidbody2D m_body;

    private void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        m_body.velocity = new Vector2(Input.GetAxis("Horizontal") * m_speed, m_body.velocity.y);  

        if(Input.GetKey(KeyCode.Space))
        {
            m_body.velocity = new Vector2(m_body.velocity.x, m_speed);
        }
    }
}
