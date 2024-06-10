using System;
using UnityEngine;

namespace Misc.Singelton
{
    public class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviorSingleton<T>
    {
        public static T Instance
        {
            get;
            protected set;
        }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new SystemException("An instance of this singleton already exists");
            }
            else
            {
                Instance = (T)this;
            }
        }
    }
}