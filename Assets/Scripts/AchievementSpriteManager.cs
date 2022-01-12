using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class AchievementSpriteManager : MonoBehaviour
    {

        [SerializeField] private Sprite ach_sprite_apple;
        [SerializeField] private Sprite ach_sprite_banana;
        [SerializeField] private Sprite ach_sprite_strawberry;
        [SerializeField] private Sprite ach_sprite_finish_level;

        public Sprite GetAchievementSprite(string img)
        {
            switch (img)
            {
                case "apple":
                    return ach_sprite_apple;
                case "banana":
                    return ach_sprite_banana;
                case "strawberry":
                    return ach_sprite_strawberry;
                case "finish_level":
                default:
                    return ach_sprite_finish_level;
            }
        }
    }
}
