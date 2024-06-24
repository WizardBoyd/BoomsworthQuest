#if UNITY_EDITOR

using DependencyInjection.attributes;
using UnityEngine;

namespace DependencyInjection.Tests
{
    public class ClassB : MonoBehaviour
    {
        [Inject]
        private ServiceA ServiceA;
        [Inject]
        private ServiceB ServiceB;
    }
}
#endif