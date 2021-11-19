using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class GameManager : MonoBehaviour
    {
        static bool levelObjectivesLoaded = false;
        
        public List<LevelObjectives> objectives = new List<LevelObjectives>();

        public void Awake()
        {
            LoadLevelObjectives();
        }

        public void LoadLevelObjectives()
        {
            // Do this only once but keep all the scenes playable
            if(levelObjectivesLoaded)
            {
                return;
            }


            LevelObjectives test = new LevelObjectives(0, 300, 30000);
            LevelObjectives test2 = new LevelObjectives(1, 200, 20000);

            objectives.Add(test);
            objectives.Add(test2);

            levelObjectivesLoaded = true;



        }

        void Update()
        {
            // TODO: this is for debugging
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

    }
}
