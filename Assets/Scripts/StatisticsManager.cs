using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf {
    public class StatisticsManager 
    {
        /*
        private static Dictionary<Collectable.Type, int> collectedFruits = new Dictionary<Collectable.Type, int>() 
        {
            { Collectable.Type.Apple, 0 },
            { Collectable.Type.Bananas, 0 },
            { Collectable.Type.Cherries, 0 },
            { Collectable.Type.Kiwi, 0 },
            { Collectable.Type.Melon, 0 },
            { Collectable.Type.Orange, 0 },
            { Collectable.Type.Pineapple, 0 },
            { Collectable.Type.Strawberry, 0 }
        };
        */

        private static Dictionary<Collectable.Type, string> localizationStrings = new Dictionary<Collectable.Type, string>()
        {
            { Collectable.Type.Apple, "apples_collected" },
            { Collectable.Type.Bananas, "bananas_collected" },
            { Collectable.Type.Cherries, "cherries_collected" },
            { Collectable.Type.Kiwi, "kiwis_collected" },
            { Collectable.Type.Melon, "melons_collected" },
            { Collectable.Type.Orange, "oranges_collected" },
            { Collectable.Type.Pineapple, "pineapples_collected" },
            { Collectable.Type.Strawberry, "strawberries_collected" }
        };

        public static void AddCollectedFruit(Collectable.Type type)
        {
            switch (type)
            {
                case Collectable.Type.Apple:
                    PlayerStats.CollectedApples++;
                    break;
                case Collectable.Type.Bananas:
                    PlayerStats.CollectedBananas++;
                    break;
                case Collectable.Type.Cherries:
                    PlayerStats.CollectedCherries++;
                    break;
                case Collectable.Type.Kiwi:
                    PlayerStats.CollectedKiwis++;
                    break;
                case Collectable.Type.Melon:
                    PlayerStats.CollectedMelons++;
                    break;
                case Collectable.Type.Orange:
                    PlayerStats.CollectedOranges++;
                    break;
                case Collectable.Type.Pineapple:
                    PlayerStats.CollectedPineapples++;
                    break;
                case Collectable.Type.Strawberry:
                    PlayerStats.CollectedStrawberries++;
                    break;
            }
        }

        public static int GetCollectedFruits(Collectable.Type type)
        {
            switch(type)
            {
                case Collectable.Type.Apple:
                    return PlayerStats.CollectedApples;
                case Collectable.Type.Bananas:
                    return PlayerStats.CollectedBananas;
                case Collectable.Type.Cherries:
                    return PlayerStats.CollectedCherries;
                case Collectable.Type.Kiwi:
                    return PlayerStats.CollectedKiwis;
                case Collectable.Type.Melon:
                    return PlayerStats.CollectedMelons;
                case Collectable.Type.Orange:
                    return PlayerStats.CollectedOranges;
                case Collectable.Type.Pineapple:
                    return PlayerStats.CollectedPineapples;
                case Collectable.Type.Strawberry:
                    return PlayerStats.CollectedStrawberries;
            }
            return 0;
        }

        public static string GetLocalizationKey(Collectable.Type type)
        {
            if(localizationStrings.ContainsKey(type))
            {
                return localizationStrings[type];
            }
            return string.Empty;
        }

        /*
        public static Dictionary<Collectable.Type, int> GetCollectedFruits()
        {
            collectedFruits[Collectable.Type.Apple] = PlayerStats.CollectedApples;
            collectedFruits[Collectable.Type.Bananas] = PlayerStats.CollectedBananas;
            collectedFruits[Collectable.Type.Cherries] = PlayerStats.CollectedCherries;
            collectedFruits[Collectable.Type.Kiwi] = PlayerStats.CollectedKiwis;
            collectedFruits[Collectable.Type.Melon] = PlayerStats.CollectedMelons;
            collectedFruits[Collectable.Type.Orange] = PlayerStats.CollectedOranges;
            collectedFruits[Collectable.Type.Pineapple] = PlayerStats.CollectedPineapples;
            collectedFruits[Collectable.Type.Strawberry] = PlayerStats.CollectedStrawberries;
            return collectedFruits;
        }
        */
        
        public static void SetCompletedLevel(int level)
        {
            PlayerStats.LevelsCompleted[level] = true;
        }

        
        public static int GetCompletedLevelsAmount()
        {
            int levels = 0;
            foreach (bool value in PlayerStats.LevelsCompleted) 
            { 
                if(value)
                {
                    levels++;
                }
            }
            return levels;
        }        
        
        public static void SetCompletedLevelWithoutHits(int level)
        {
            PlayerStats.LevelsCompletedWithoutHits[level] = true;
        }

        public static int GetCompletedLevelsWithoutHitsAmount()
        {
            int levels = 0;
            foreach (bool value in PlayerStats.LevelsCompletedWithoutHits) 
            { 
                if(value)
                {
                    levels++;
                }
            }
            return levels;
        }
        
    }
    
}
