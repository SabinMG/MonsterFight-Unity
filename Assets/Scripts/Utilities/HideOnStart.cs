using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.Utilities
{ 
    public class HideOnStart : MonoBehaviour
    {
        public bool hideOnStart = true;

        void Start()
        {
            if (hideOnStart) gameObject.SetActive(false);
        }
    }
}
