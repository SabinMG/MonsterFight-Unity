using UnityEngine;
using System.Collections;

namespace SpaceOrigin.ObjectPool
{
    public class PoolInitializer : MonoBehaviour
    {
        public PoolInitializeObject[] m_objectsTobePooled;

        public void Awake()
        {
            PoolManager poolInstance = PoolManager.Instance;
            for (int i = 0; i < m_objectsTobePooled.Length; i++)
            {
                PoolInitializeObject objectTobeInitialized = m_objectsTobePooled[i];
                poolInstance.CreatePool(objectTobeInitialized.m_poolObjectPrefab, objectTobeInitialized.m_poolMaxSize);
            }
        }

    }
}

[System.Serializable]
public class PoolInitializeObject
{
	public GameObject m_poolObjectPrefab;
	public int m_poolMaxSize;
}