using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int startingHealth;
    private float currentHealth;

    void Awake()
    {
        currentHealth = startingHealth;
    }

   public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        if(currentHealth > 0)
        {
            //player hurt
        }
        else
        {
            //dead
        }
    }
}
