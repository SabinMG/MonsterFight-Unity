using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.MonsterFight
{
    public abstract class GameAbstractFactory
    {
        public abstract PlayerUnit GetPlayerUnit();
        public abstract AIUnit GetAIUnit();
       
        public abstract void RecyclePlayerUnit(PlayerUnit playerUnit);
        public abstract void RecycleAIUnit(AIUnit aiUnit);
    }
}
