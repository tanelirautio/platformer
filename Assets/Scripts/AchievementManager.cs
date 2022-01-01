using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf {

    public class AchievementManager : MonoBehaviour
    {

        void Start()
        {
            // load achievements from playerprefs
            for(int i = 0; i < PlayerStats.Achievements.Count; i++)
            {
                if(PlayerPrefs.GetInt(PlayerStats.Achievements[i].title, 0) == 1)
                {
                    PlayerStats.Achievements[i].Completed = true;
                }
            }
        }

        void Update()
        {
            
        }
    }
}
