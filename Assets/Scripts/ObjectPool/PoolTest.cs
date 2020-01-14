using UnityEngine;
using System.Collections;

namespace SpaceOrigin.ObjectPool
{
    public class PoolTest : MonoBehaviour
    {
        public GameObject m_objectToPool;

        public int m_poolSize;
        // Use this for initialization
        void Start()
        {
            //PoolManager.Instance.CreatePool (m_objectToPool,m_poolSize);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}