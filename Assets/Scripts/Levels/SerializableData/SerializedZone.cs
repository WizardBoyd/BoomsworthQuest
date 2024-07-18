using System;
using UnityEngine.AddressableAssets;

namespace Levels.SerializableData
{
    [Serializable]
    public class SerializedZone
    {
        public string ZoneSoAssetKey { get; set; }
        public int ZoneIndex { get;  set; }
        public SerializedLevel[] Levels;
        public SerializedZone(string assetReference, int zoneIndex)
        {
            ZoneSoAssetKey = assetReference;
            ZoneIndex = zoneIndex;
        }
        
        public SerializedZone()
        {
        }
    }
}