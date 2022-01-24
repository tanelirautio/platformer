using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace pf {

    [RequireComponent (typeof(AchievementSpriteManager))]
    public class AchievementManager : MonoBehaviour
    {
        //Achievement data is loaded in DataParser
        //Achievement completed data is loaded from save game

        private Animator animator;
        private Image image;
        private TextLocalizerUI titleLocalizer;
        private TextLocalizerUI descLocalizer;

        // Animation states
        const string ACHIEVEMENT_IDLE = "achievement_idle";
        const string ACHIEVEMENT_IN = "achievement_in";
        const string ACHIEVEMENT_OUT = "achievement_out";

        private AchievementSpriteManager spriteManager;

        private static Queue<Achievements> achievementsToShow = new Queue<Achievements>();
        private bool canShowAchievement = true;

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
            image = GameObject.Find("UICanvas/AchievementPanel/AchieveImgBg/AchieveImg").GetComponent<Image>();
            titleLocalizer = GameObject.Find("UICanvas/AchievementPanel/AchieveTitle").GetComponent<TextLocalizerUI>();
            descLocalizer = GameObject.Find("UICanvas/AchievementPanel/AchieveDesc").GetComponent<TextLocalizerUI>();
            spriteManager = GetComponent<AchievementSpriteManager>();
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
                            achievementsToShow.Enqueue(ach);
                            PlayerStats.CompletedAchievements[ach.id] = true;
                            print("Completed achievement id: " + ach.id + " - " + ach.title);
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
                            achievementsToShow.Enqueue(ach);
                            PlayerStats.CompletedAchievements[ach.id] = true;
                            print("Completed achievement id: " + ach.id + " - " + ach.title);
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
                        achievementsToShow.Enqueue(ach);
                        PlayerStats.CompletedAchievements[ach.id] = true;
                        print("Completed achievement id: " + ach.id + " - " + ach.title);
                    }
                }
            }
        }


        private void Update()
        {
            if(achievementsToShow.Count > 0 && canShowAchievement)
            {
                Achievements ach = achievementsToShow.Dequeue();
                titleLocalizer.key = ach.title;
                titleLocalizer.Localize();
                descLocalizer.key = ach.desc;
                descLocalizer.Localize();
                image.sprite = spriteManager.GetAchievementSprite(ach.img);
                animator.Play(ACHIEVEMENT_IN);
                StartCoroutine(HideAchievement());
                canShowAchievement = false;
                print("AchievementsToShow: " + achievementsToShow.Count);
                print("Showing achievement id: " + ach.id + " - " + ach.title);
            }
        }

        private IEnumerator HideAchievement()
        {
            yield return new WaitForSeconds(Defs.ACHIEVEMENT_SHOW_TIME);
            animator.Play(ACHIEVEMENT_OUT);
            yield return new WaitForSeconds(Defs.ACHIEVEMENT_WAIT_TIME);
            canShowAchievement = true;
            yield return null;
        } 
    }
}
