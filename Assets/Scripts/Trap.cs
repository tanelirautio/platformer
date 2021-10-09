using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Player player;
    private PlayerHealth playerHealth;

    public enum Type
    {
        Spike,
    }

    public Type type;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
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
            player.TakeDamage(collision.transform.position);
        }
    }
}
