using UnityEngine;
using UnityEngine.Events;

namespace SpaceOrigin.Data
{
    /// <summary>
    /// Base class for the scriptable objects
    /// </summary>
    public class BaseSO<T> : ScriptableObject 
    {
        [System.Serializable]
        public class GenericEvent : UnityEvent<T>
        {
        }

        public GenericEvent onValueChanged;

        private T m_value;
        public T Value
        {
            get { return m_value; }
            set
            {
                
                m_value = value;
                onValueChanged.Invoke(m_value);
            }
        }

        public void OnEnable()
        {
            onValueChanged = new GenericEvent();
        } 
    }
}
