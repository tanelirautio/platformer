using System.Collections.Generic;

namespace pf
{
    public static class PlayerStats
    {
        public static int SelectedCharacter { get; set; }
        public static int SceneIndex { get; set; }

        public static int Level
        {
            get
            {
                return SceneIndex - (int)LevelLoader.Scenes.StartLevel;
            }
        }
        public static int Health { get; set; }
        public static List<int> BestScores = new List<int>(new int[Defs.LEVEL_AMOUNT]);
        public static List<int> Scores = new List<int>(new int[Defs.LEVEL_AMOUNT]);
        public static List<LevelObjectives> CompletedObjectives = new List<LevelObjectives>();
        public static List<Achievements> Achievements = new List<Achievements>();

        // Statistics
        public static List<bool> LevelsCompleted = new List<bool>(new bool[Defs.LEVEL_AMOUNT]);
        public static List<bool> LevelsCompletedWithoutHits = new List<bool>(new bool[Defs.LEVEL_AMOUNT]);
        public static int CollectedApples { get; set; }
        public static int CollectedBananas { get; set; }
        public static int CollectedStrawberries { get; set; }
        public static int CollectedCherries { get; set; }
        public static int CollectedKiwis { get; set; }
        public static int CollectedMelons { get; set; }
        public static int CollectedOranges { get; set; }
        public static int CollectedPineapples { get; set; }
    }
}
