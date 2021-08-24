using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;

    public const float SKINWIDTH = 0.015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    [HideInInspector] public float horizontalRaySpacing;
    [HideInInspector] public float verticalRaySpacing;

    [HideInInspector] public BoxCollider2D m_collider;
    public RaycastOrigins m_raycastOrigins;

    public virtual void Start()
    {
        m_collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(SKINWIDTH * -2);

        m_raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        m_raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        m_raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        m_raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    public void CalculateRaySpacing()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(SKINWIDTH * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
