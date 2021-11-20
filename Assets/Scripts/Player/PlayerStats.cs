using System.Collections.Generic;
public static class PlayerStats
{
    public static int SelectedCharacter { get; set; }
    public static int SceneIndex { get; set; }
    public static int Health { get; set; }
    public static int Score { get; set; }

    public static List<LevelObjectives> completedObjectives = new List<LevelObjectives>();
}
