using System;
using Levels;
using UnityEngine.AddressableAssets;

namespace AssetManagment.ConcreteReferences
{
    [Serializable]
    public class AssetReferenceLevelZone : AssetReferenceT<ZoneRuntime>
    {
        public AssetReferenceLevelZone(string guid) : base(guid)
        {
        }
    }
}