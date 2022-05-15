using UnityEngine;
using UnityEngine.Assertions;

namespace pf
{
    public class PlayerHealth : MonoBehaviour
    {
        private int startingHealth = Defs.HEALTH_START;

        //animation states
        const string HEALTH_ADD = "health_add";
        const string HEALTH_REMOVE = "health_remove";
        const string HEALTH_FULL = "health_full";
        const string HEALTH_ZERO = "health_zero";

        private int currentHealth = 0;
        //private bool hasBeenHit = false;
        private int hits = 0;

        private GameObject[] uiHealth = new GameObject[Defs.HEALTH_MAX];
        private Animator[] uiHealthAnim = new Animator[Defs.HEALTH_MAX];

        private LevelLoader levelLoader;

        void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

            GameObject health = GameObject.Find("UICanvas/Health");
            int children = health.transform.childCount;
            Assert.AreEqual(children, Defs.HEALTH_MAX);
            for (int i = 0; i < children; ++i)
            {
                uiHealth[i] = health.transform.GetChild(i).gameObject;
            }
        }

        public void Reset()
        {
            print("Reset player health");

            if (LevelLoader.GetCurrentSceneIndex() != (int)LevelLoader.Scenes.StartLevel &&
                LevelLoader.GetPreviousSceneIndex() != (int)LevelLoader.Scenes.Continue &&
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
            //hasBeenHit = false;
            hits = 0;

            for (int i = 0; i < Defs.HEALTH_MAX; i++)
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
            if(currentHealth + 1 > Defs.HEALTH_MAX)
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

        /*
        public bool HasBeenHit()
        {
            return hasBeenHit;
        }
        */

        public int Hits()
        {
            return hits;
        }

        public int TakeDamage(Trap.Type trapType)
        {

            print("Trap type: " + trapType.ToString());
            //hasBeenHit = true;
            hits++;

            currentHealth = Mathf.Clamp(currentHealth - Defs.HEALTH_DAMAGE, 0, Defs.HEALTH_MAX);
            Animator anim = uiHealthAnim[currentHealth];
            anim.Play(HEALTH_REMOVE);
            PlayerStats.Health = currentHealth;
            return currentHealth;
        }

    }
}
