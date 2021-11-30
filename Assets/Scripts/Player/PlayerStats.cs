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
    }
}
