using UnityEngine;
using System.Collections;

namespace SpaceOrigin.ObjectPool
{
    public class PoolParent : MonoBehaviour
    {
        static PoolParent ms_sharedInstance;
        static Transform m_poolParentTrans;

        public static PoolParent Instance
        {
            get
            {
                if (ms_sharedInstance == null)
                {
                    ms_sharedInstance = new PoolParent(); // rewtrite
                }

                return ms_sharedInstance;
            }
        }

        public Transform GetPoolParent()
        {
            if (m_poolParentTrans == null)
            {
                GameObject poolParentGO = new GameObject();
                poolParentGO.name = "Object Pools";
                m_poolParentTrans = poolParentGO.transform;
            }
            return m_poolParentTrans;
        }

    }
}
