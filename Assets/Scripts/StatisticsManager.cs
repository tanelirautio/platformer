using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf {
    public class StatisticsManager 
    {

        static void AddCollectedFruit(Collectable.Type type)
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

        static int GetCollectedFruits(Collectable.Type type)
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

        static void SetCompletedLevel(int level)
        {
            PlayerStats.LevelsCompleted[level] = true;
        }

        static int GetCompletedLevelsAmount()
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
        
        static void SetCompletedLevelWithoutHits(int level)
        {
            PlayerStats.LevelsCompletedWithoutHits[level] = true;
        }

        static int GetCompletedLevelsWithoutHitsAmount()
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
