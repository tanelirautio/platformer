using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public LayerMask m_collisionMask;

    const float SKINWIDTH = 0.015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float m_horizontalRaySpacing;
    float m_verticalRaySpacing;

    BoxCollider2D m_collider;
    RaycastOrigins m_raycastOrigins;
    
    public CollisionInfo collisions;

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    private void Start()
    {

    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? m_raycastOrigins.bottomLeft : m_raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (m_horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, m_collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.yellow);

            if (hit)
            {
                velocity.x = (hit.distance - SKINWIDTH) * directionX;
                rayLength = hit.distance;
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? m_raycastOrigins.bottomLeft : m_raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (m_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, m_collisionMask);
            
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if(hit)
            {
                velocity.y = (hit.distance - SKINWIDTH) * directionY;
                rayLength = hit.distance;
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(SKINWIDTH * -2);

        m_raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        m_raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        m_raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        m_raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    void CalculateRaySpacing()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(SKINWIDTH * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        m_horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        m_verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
