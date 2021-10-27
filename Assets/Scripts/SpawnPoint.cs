using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float radius = 0.5f;
        Gizmos.DrawWireSphere(transform.position, radius);
        //Gizmos.color = Color.yellow;
        //radius = 0.25f;
        //Gizmos.DrawWireSphere(transform.position, radius);
    }
}
