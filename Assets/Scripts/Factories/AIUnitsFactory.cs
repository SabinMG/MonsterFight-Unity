using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceOrigin.ObjectPool;


namespace SpaceOrigin.MonsterFight
{
    public class AIUnitsFactory : GameFactory
    {
        public override AIUnit GetAIUnit()
        {
            return PoolManager.Instance.GetObjectFromPool("AIUnit").GetComponent<AIUnit>();
        }

        public override void RecycleAIUnit(AIUnit aiUnit)
        {
            PoolManager.Instance.PutObjectBacktoPool(aiUnit.gameObject);
        }
    }
}
