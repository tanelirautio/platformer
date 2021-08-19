using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public float m_jumpHeight = 4;
    public float m_timeToJumpApex = 0.4f;

    float m_accelerationTimeAirborne = 0.2f;
    float m_accelerationTimeGrounded = 0.1f;
    private float m_moveSpeed = 6;

    private float m_gravity;
    private float m_jumpVelocity;
    private Vector3 m_velocity;
    private float m_velocityXSmoothing;

    private Controller2D m_controller;

    private void Awake()
    {
        m_controller = GetComponent<Controller2D>();

        m_gravity = -(2 * m_jumpHeight) / Mathf.Pow(m_timeToJumpApex, 2);
        m_jumpVelocity = Mathf.Abs(m_gravity) * m_timeToJumpApex;
        print("Gravity: " + m_gravity + "  Jump Velocity: " + m_jumpVelocity);
    }

    private void Update()
    {
        if(m_controller.collisions.above || m_controller.collisions.below)
        {
            m_velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown(KeyCode.Space) && m_controller.collisions.below)
        {
            m_velocity.y = m_jumpVelocity;
        }

        float targetVelocityX = input.x * m_moveSpeed;
        m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVelocityX, ref m_velocityXSmoothing, (m_controller.collisions.below)?m_accelerationTimeGrounded:m_accelerationTimeAirborne);
        m_velocity.y += m_gravity * Time.deltaTime;
        m_controller.Move(m_velocity * Time.deltaTime);
    }
}