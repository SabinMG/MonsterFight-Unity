using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// produces different factories
/// </summary>
namespace SpaceOrigin.MonsterFight
{
    public class GameFactoryProducer
    {
        public static GameFactory GetFactory(string factoryName)
        {
            switch (factoryName)
            {
                case "PlayerUnitsFactory":
                    return new PlayerUnitsFactory();

                case "AIUnitsFactory":
                    return new AIUnitsFactory();

                default:
                    return null;
            }
        }
    }
}
