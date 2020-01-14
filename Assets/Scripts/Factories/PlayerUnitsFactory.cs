using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceOrigin.ObjectPool;

namespace SpaceOrigin.MonsterFight
{
    /// <summary>
    /// Factory for creating and recycling player units
    /// </summary>
    public class PlayerUnitsFactory : GameFactory
    {
        public override PlayerUnit GetPlayerUnit()
        {
            return PoolManager.Instance.GetObjectFromPool("PlayerUnit").GetComponent<PlayerUnit>();
        }

        public override void RecyclePlayerUnit(PlayerUnit playerUnit)
        {
            PoolManager.Instance.PutObjectBacktoPool(playerUnit.gameObject);
        }
    }
}
