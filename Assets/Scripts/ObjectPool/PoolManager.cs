using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpaceOrigin.ObjectPool
{
    public class PoolManager
    {
        Dictionary<string, ObjectPool> m_objectsPools;
        static PoolManager ms_sharedInstance;

        private PoolManager()
        {
            this.m_objectsPools = new Dictionary<string, ObjectPool>();
        }

        public static PoolManager Instance
        {
            get
            {
                if (ms_sharedInstance == null)
                {
                    ms_sharedInstance = new PoolManager();
                }
                return ms_sharedInstance;
            }
        }

        public void CreatePool(GameObject obj, int maxSize)
        {
            ObjectPool objPool = new ObjectPool();

            if (objPool.CreateObjectPool(obj, maxSize))
            {
                m_objectsPools[obj.name] = objPool;
            }
        }

        public GameObject GetObjectFromPool(string objectName)
        {
            ObjectPool specificObjectPool = m_objectsPools[objectName];
            return specificObjectPool.GetObject();
        }

        public void PutObjectBacktoPool(GameObject go)
        {
            ObjectPool specificObjectPool = m_objectsPools[go.name];
            specificObjectPool.PutObject(go);
        }
    }
}
