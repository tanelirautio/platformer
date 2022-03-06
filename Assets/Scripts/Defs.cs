namespace pf
{
    public static class Defs
    {
        public const int SAVEDATA_VERSION = 1;

        // Game
        public const int LEVEL_AMOUNT = 3;
        public const int OBJECTIVES_PER_LEVEL = 3;

        // Collectables
        public const float COLLECTABLE_FADE_TIME = 1.0f;
        public const float COLLECTABLE_FADE_INVOKE_TIME = 1.0f;

        // Powerups
        public const float POWERUP_FADE_TIME = 2.0f;
        public const float POWERUP_EXTRA_JUMP_POWER = 2.0f;
        public const float POWERUP_EXTRA_SPEED = 4.0f;

        // Credits
        public const float CREDITS_TIME = 40.0f;

        // Achievements
        public const int ACHIEVEMENT_COUNT = 12;
        public const float ACHIEVEMENT_SHOW_TIME = 3.0f;
        public const float ACHIEVEMENT_WAIT_TIME = 0.5f;

        // Menu
        public const float MENU_NORMAL_SCALE = 1.0f;
        public const float MENU_SELECTED_SCALE = 1.1f;
        public const float MENU_SCALE_SPEED = 1f;
        public const float MENU_LEVELSELECT_SCALE_SPEED = 0.5f;

        // Player
        public const float PLAYER_VELOCITY_X_THRESHOLD = 0.5f;
        public const int PLAYER_GRACE_PERIOD_FLASH_AMOUNT = 8;
        public const float PLAYER_GRACE_PERIOD_LENGTH = 2.0f;
        public const float PLAYER_GRACE_PERIOD_OFFSET = 0.3f;

        // Traps
        public const float TRAP_FALLING_PLATFORM_WAIT_TIME = 1.0f;
        public const float TRAP_FIRE_PIT_TURNON_TIME = 1.0f;
    }
}