using UnityEngine;
using UnityEngine.Assertions;

namespace pf
{
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

        private GameObject[] uiHealth = new GameObject[MAX_HEALTH];
        private Animator[] uiHealthAnim = new Animator[MAX_HEALTH];

        private LevelLoader levelLoader;

        void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

            GameObject health = GameObject.Find("UICanvas/Health");
            int children = health.transform.childCount;
            Assert.AreEqual(children, MAX_HEALTH);
            for (int i = 0; i < children; ++i)
            {
                uiHealth[i] = health.transform.GetChild(i).gameObject;
            }
        }

        public void Reset()
        {
            print("Reset player health");

            if (levelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.StartLevel &&
                LevelLoader.GetPreviousSceneIndex() != -1)
            {
                print("get value from PlayerStats.CurrentHealth");
                startingHealth = PlayerStats.Health;
            }
            else
            {
                PlayerStats.Health = startingHealth;
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
                    //uiHealth[i].SetActive(false);
                    uiHealth[i].SetActive(true);
                    Animator anim = uiHealthAnim[i];
                    anim.Play(HEALTH_ZERO);
                }
            }
        }

        public bool AddHealth()
        {
            print("Adding health to player");
            if(currentHealth + 1 > MAX_HEALTH)
            {
                //give score instead of health
                print("Should give extra score");
                return false;
            }
            else
            {
                print("add 1 health");
                Animator anim = uiHealthAnim[currentHealth];
                anim.Play(HEALTH_ADD);
                currentHealth = currentHealth + 1;
                PlayerStats.Health = currentHealth;
            }
            return true;
        }

        public int TakeDamage(Trap.Type trapType)
        {
            print("Trap type: " + trapType.ToString());

            currentHealth = Mathf.Clamp(currentHealth - DAMAGE, 0, MAX_HEALTH);
            Animator anim = uiHealthAnim[currentHealth];
            anim.Play(HEALTH_REMOVE);
            PlayerStats.Health = currentHealth;
            return currentHealth;
        }

    }
}
