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

        // Animation states
        const string ACHIEVEMENT_IDLE = "achievement_idle";
        const string ACHIEVEMENT_IN = "achievement_in";
        const string ACHIEVEMENT_OUT = "achievement_out";

        // Must be the same values as "type" field in achievements.json
        private enum AchievementType
        {
            Collect = 1,
            FinishLevel = 2,
            FinishLevelWithoutHits = 3
        }

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
                if(ach.type == (int)AchievementType.Collect && (int)type == ach.collectableItem)
                {
                    if(StatisticsManager.GetCollectedFruits(type) >= ach.count)
                    {
                        if(PlayerStats.CompletedAchievements[ach.id] == false)
                        {
                            print("Completed achievement id: " + ach.id + "!");
                            // TODO: Show Completed Achievement UI (correct image)

                            titleLocalizer.key = ach.title;
                            titleLocalizer.Localize();

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

        public void CheckCompletedLevelsAchievement()
        {
            for (int i = 0; i < PlayerStats.Achievements.Count; i++)
            {
                Achievements ach = PlayerStats.Achievements[i];
                if (ach.type == (int)AchievementType.FinishLevel)
                {
                    if (StatisticsManager.GetCompletedLevelsAmount() >= ach.count)
                    {
                        if (PlayerStats.CompletedAchievements[ach.id] == false)
                        {
                            print("Completed achievement id: " + ach.id + "!");
                            // TODO: Show Completed Achievement UI (correct image)

                            titleLocalizer.key = ach.title;
                            titleLocalizer.Localize();

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

        public void CheckCompletedLevelsWihtoutHitsAchievement()
        {
            for (int i = 0; i < PlayerStats.Achievements.Count; i++)
            {
                Achievements ach = PlayerStats.Achievements[i];
                if (ach.type == (int)AchievementType.FinishLevelWithoutHits)
                {
                    if (StatisticsManager.GetCompletedLevelsAmount() >= ach.count)
                    {
                        print("Completed achievement id: " + ach.id + "!");
                        // TODO: Show Completed Achievement UI (correct image)

                        titleLocalizer.key = ach.title;
                        titleLocalizer.Localize();

                        descLocalizer.key = ach.desc;
                        descLocalizer.Localize();

                        animator.Play(ACHIEVEMENT_IN);
                        Invoke("HideAchievement", 5f);

                        PlayerStats.CompletedAchievements[ach.id] = true;
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
