using Factory;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AssetManagment.ConcreteReferences
{
    public class AssetReferenceComponentPool<T> : AssetReferenceT<ComponentPoolSO<T>> where T : Component 
    {
        public AssetReferenceComponentPool(string guid) : base(guid)
        {
        }
    }
}