#if UNITY_EDITOR
using DependencyInjection.attributes;
using UnityEngine;

namespace DependencyInjection.Tests
{
    public class ClassA : MonoBehaviour
    {
        private ServiceA ServiceA;

        [Inject]
        public void Init(ServiceA a)
        {
            this.ServiceA = a;
        }
    }
}
#endif