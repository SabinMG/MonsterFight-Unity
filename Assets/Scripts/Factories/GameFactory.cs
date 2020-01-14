using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.MonsterFight
{
    public class GameFactory : GameAbstractFactory
    {
        public override PlayerUnit GetPlayerUnit()
        {
            return null;
        }

        public override AIUnit GetAIUnit()
        {
            return null;
        }

        public override void RecyclePlayerUnit(PlayerUnit playerUnit)
        {  
        }

        public override void RecycleAIUnit(AIUnit aiUnit)
        {
        }
    }
}
