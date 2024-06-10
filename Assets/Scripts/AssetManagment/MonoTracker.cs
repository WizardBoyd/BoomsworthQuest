using System;
using UnityEngine;

namespace AssetManagment
{
    public class MonoTracker : MonoBehaviour
    {
        public delegate void DelegateDestroyed(MonoTracker tracker);
        public event DelegateDestroyed OnDestroyed;
        
        public object key { get; set; }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}