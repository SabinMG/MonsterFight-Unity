using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpaceOrigin.ObjectPool
{
    public class ObjectPool
    {
        public ObjectPool()
        {
        }

        private GameObject m_pooledObj;
        private List<GameObject> m_pooledObjects;

        private List<GameObject> m_usedObjects;
        private int m_poolCurrentSize;
        private string m_poolName;

        GameObject m_poolParentGO;

        private Vector3 m_poolLocation = new Vector3(100.0f, 0, 0);


        public bool CreateObjectPool(GameObject obj, int maxSize)
        {
            m_poolName = obj.name;
            m_pooledObj = obj;
            m_pooledObjects = new List<GameObject>();
            m_poolParentGO = new GameObject();
            m_poolParentGO.name = obj.name + " (Pool " + maxSize + ")";

            m_poolParentGO.transform.position = m_poolLocation;// to fix object activate deactive spike issue

            m_poolParentGO.transform.parent = PoolParent.Instance.GetPoolParent();

            for (int i = 0; i < maxSize; i++)
            {
                GameObject newGO = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
                newGO.SetActive(false);
                newGO.transform.parent = m_poolParentGO.transform;
                newGO.transform.localPosition = Vector3.zero;
                newGO.name = obj.name;
                m_pooledObjects.Add(newGO);
            }

            m_poolCurrentSize = maxSize;
            return true;
        }


        public GameObject GetObject()
        {
            if (m_pooledObjects.Count == 0)
            {
                Debug.LogError("There is no remiang item in the pool, please initialize pool with biggger size");
            }
            GameObject go = m_pooledObjects[m_pooledObjects.Count - 1];
            m_pooledObjects.RemoveAt(m_pooledObjects.Count - 1);
            go.SetActive (false);
            return go;
        }

        public void PutObject(GameObject go)
        {
            go.SetActive (false);
            go.transform.parent = m_poolParentGO.transform;
            m_pooledObjects.Add(go);
        }
    }
}




