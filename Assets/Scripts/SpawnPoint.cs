using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float radius = 1.0f;
        Gizmos.DrawSphere(transform.position, radius);       
    }
}
