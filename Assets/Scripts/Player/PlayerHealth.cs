using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    const int DAMAGE = 1;
    const int MAX_HEALTH = 5;

    //animation states
    const string HEALTH_ADD = "health_add";
    const string HEALTH_REMOVE = "health_remove";
    const string HEALTH_FULL = "health_full";
    const string HEALTH_ZERO = "health_zero";

    [SerializeField] private int startingHealth;
    private int currentHealth;

    public GameObject[] uiHealth = new GameObject[MAX_HEALTH];
    private Animator[] uiHealthAnim = new Animator[MAX_HEALTH];

    void Start()
    {
        currentHealth = startingHealth;

        for(int i=0; i < MAX_HEALTH; i++)
        {
            uiHealthAnim[i] = uiHealth[i].GetComponent<Animator>();

            if(i < startingHealth)
            {
                uiHealth[i].SetActive(true);
            }
            else
            {
                uiHealth[i].SetActive(false);
            }
        }
    }

    public int TakeDamage(Trap.Type trapType)
    {
        currentHealth = Mathf.Clamp(currentHealth - DAMAGE, 0, startingHealth);
        Animator anim = uiHealthAnim[currentHealth];
        anim.Play(HEALTH_REMOVE);
        return currentHealth;

        /*
        if (currentHealth > 0)
        {
            playerAnim.TakeDamage();
            player.setGracePeriod();
            print("player hurt!");
            //player hurt
        }
        else
        {
            playerAnim.Die();
            print("player dead!");

            //TODO this is just for testing
            //Move this to player class - play death anim and disable controller
            uiController.Fade(true, 0.5f);

        }
        */
    }

}
