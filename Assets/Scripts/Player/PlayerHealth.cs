using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    const float GRACE_PERIOD_LENGTH = 0.5f;

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

    private PlayerAnimation playerAnim;

    private bool gracePeriod = false;

    private void Awake()
    {
        playerAnim = GetComponent<PlayerAnimation>();
    }

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

    public void TakeDamage(Trap.Type trapType)
    {
        if(gracePeriod)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - DAMAGE, 0, startingHealth);

        Animator anim = uiHealthAnim[currentHealth];
        anim.Play(HEALTH_REMOVE);

        if (currentHealth > 0)
        {
            gracePeriod = true;
            Invoke("EndGracePeriod", GRACE_PERIOD_LENGTH);
            playerAnim.TakeDamage();
            print("player hurt!");
            //player hurt
        }
        else
        {
            print("player dead!");
            //dead
        }
    }

    private void EndGracePeriod()
    {
        gracePeriod = false;
    }
}
