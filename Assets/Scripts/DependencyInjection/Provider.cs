using DependencyInjection.attributes;
using UnityEngine;

namespace DependencyInjection
{
    public class Provider : MonoBehaviour, IDependencyProvider
    {
#if UNITY_EDITOR
        //Tests

        [Provide]
        public ServiceB ProvideServiceB()
        {
            return new ServiceB();
        }
        
        [Provide]
        public ServiceA ProvideServiceA()
        {
            return new ServiceA();
        }
        
#endif
    }

#if UNITY_EDITOR
    #region TestClass

    public class ServiceA
    {
        public void Init(string message = null)
        {
            Debug.Log($"ServiceA.Init({message})");
        }
    }
    
    public class ServiceB
    {
        public void Init(string message = null)
        {
            Debug.Log($"ServiceB.Init({message})");
        }
    }

    #endregion
#endif
}