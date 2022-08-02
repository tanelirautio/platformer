using System.Collections.Generic;
using System.Collections;
using System;
using System.Diagnostics;

namespace pf
{
    public static class PlayerStats
    {
        public static int SelectedCharacter { get; set; }
        public static int SceneIndex { get; set; } = -1;
        public static float MusicVolume { get; set; } = 1f;
        public static float SoundVolume { get; set; } = 1f;
        public static int Language { get; set; } = 0; 

        //TODO: Make CurrentLevel separate from SceneIndex
        //SceneIndex is changed with every scene change
        //CurrentLevel is only changed when player plays actual levels!
       
        public static int GetCurrentLevel()
        {
            return SceneIndex - (int)LevelLoader.Scenes.StartLevel;
        }
       

        public static int Health { get; set; }
        public static int[] BestScores = new int[Defs.LEVEL_AMOUNT];
        public static CompletedObjectives[] CompletedObjectives = new CompletedObjectives[Defs.LEVEL_AMOUNT];
        public static bool[] CompletedAchievements = new bool[Defs.ACHIEVEMENT_COUNT]; 

        // Statistics
        public static bool[] LevelsCompleted = new bool[Defs.LEVEL_AMOUNT];
        public static bool[] LevelsCompletedWithoutHits = new bool[Defs.LEVEL_AMOUNT];
        public static int CollectedHearts { get; set; }
        public static int CollectedApples { get; set; }
        public static int CollectedBananas { get; set; }
        public static int CollectedStrawberries { get; set; }
        public static int CollectedCherries { get; set; }
        public static int CollectedKiwis { get; set; }
        public static int CollectedMelons { get; set; }
        public static int CollectedOranges { get; set; }
        public static int CollectedPineapples { get; set; }

        // from JSON, read only
        public static List<LevelObjectives> LevelObjectives = new List<LevelObjectives>();
        public static List<Achievements> Achievements = new List<Achievements>();

        
        static PlayerStats() 
        {
            for(int i=0; i<Defs.LEVEL_AMOUNT; i++)
            {
                CompletedObjectives[i] = new CompletedObjectives();
            }
        }
       
    }
}
