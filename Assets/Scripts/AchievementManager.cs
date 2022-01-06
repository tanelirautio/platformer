using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using TMPro;

namespace pf {

    public class AchievementManager : MonoBehaviour
    {

        private Animator animator;
        private TextLocalizerUI titleLocalizer;
        private TextLocalizerUI descLocalizer;

        //animation states
        const string ACHIEVEMENT_IDLE = "achievement_idle";
        const string ACHIEVEMENT_IN = "achievement_in";
        const string ACHIEVEMENT_OUT = "achievement_out";

        private void Awake()
        {
            animator = GameObject.Find("UICanvas/AchievementPanel").GetComponent<Animator>();
            titleLocalizer = GameObject.Find("UICanvas/AchievementPanel/AchieveTitle").GetComponent<TextLocalizerUI>();
            descLocalizer = GameObject.Find("UICanvas/AchievementPanel/AchieveDesc").GetComponent<TextLocalizerUI>();
        }

        void Start()
        {
            //Note:
            //Achievement data is loaded in DataParser
            //Achievement completed data is loaded from save game

            //TODO: put achievements in a list, if player gets multiple achievements simultaneously,
            //show the achievements one at a time
        }

        public void CheckCollectAchievement(Collectable.Type type)
        {
            for(int i=0; i < PlayerStats.Achievements.Count; i++)
            {
                Achievements ach = PlayerStats.Achievements[i];
                if(ach.type == 1 && (int)type == ach.collectableItem)
                {
                    if(StatisticsManager.GetCollectedFruits(type) == ach.count)
                    {
                        if(PlayerStats.CompletedAchievements[ach.id] == false)
                        {
                            print("Completed achievement id: " + ach.id + "!");
                            // TODO: Show Completed Achievement UI

                            print("Ach title: " + ach.title);
                            titleLocalizer.key = "ach_title_collect_100_apples";
                            titleLocalizer.Localize();

                            print("Ach desc: " + ach.desc);
                            descLocalizer.key = ach.desc;
                            descLocalizer.Localize();

                            animator.Play(ACHIEVEMENT_IN);
                            Invoke("HideAchievement", 5f);

                            PlayerStats.CompletedAchievements[ach.id] = true;
                        }
                    }
                }
            }
        }

        private void HideAchievement()
        {
            animator.Play(ACHIEVEMENT_OUT);
        }
    }
}
