using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.DesignPatterns
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        public virtual void Awake()
        {
            instance = GetComponent<T>();
        }
    }
}
