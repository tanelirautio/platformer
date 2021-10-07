using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private PlayerHealth playerHealth;

    public enum Type
    {
        Spike,
    }

    public Type type;

    void Awake()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("hit player");
            playerHealth.TakeDamage(type);
        }
    }
}
