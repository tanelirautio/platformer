using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    const int DAMAGE = 1;
    const int MAX_HEALTH = 5;
    [SerializeField] private int startingHealth = 3;

    //animation states
    const string HEALTH_ADD = "health_add";
    const string HEALTH_REMOVE = "health_remove";
    const string HEALTH_FULL = "health_full";
    const string HEALTH_ZERO = "health_zero";

    private int currentHealth = 0;

    public GameObject[] uiHealth = new GameObject[MAX_HEALTH];
    private Animator[] uiHealthAnim = new Animator[MAX_HEALTH];

    private LevelLoader levelLoader;

    void Awake()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    public void Reset()
    {
        print("Reset player health");

        if (levelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.StartLevel &&
            LevelLoader.GetPreviousSceneIndex() != -1)
        {
            print("get value from PlayerStats.CurrentHealth");
            startingHealth = PlayerStats.CurrentHealth;
        }
        else
        {
            PlayerStats.CurrentHealth = startingHealth;
        }

        currentHealth = startingHealth;

        for (int i = 0; i < MAX_HEALTH; i++)
        {
            uiHealthAnim[i] = uiHealth[i].GetComponent<Animator>();

            if (i < startingHealth)
            {
                uiHealth[i].SetActive(true);
                Animator anim = uiHealthAnim[i];
                anim.Play(HEALTH_FULL);
            }
            else
            {
                uiHealth[i].SetActive(false);
            }
        }
    }

    public int TakeDamage(Trap.Type trapType)
    {
        print("Trap type: " + trapType.ToString());

        currentHealth = Mathf.Clamp(currentHealth - DAMAGE, 0, startingHealth);
        Animator anim = uiHealthAnim[currentHealth];
        anim.Play(HEALTH_REMOVE);
        PlayerStats.CurrentHealth = currentHealth;
        return currentHealth;
    }

}
