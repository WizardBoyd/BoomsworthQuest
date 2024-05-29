using SaveSystem;
using UnityEngine;

namespace BaseClasses
{
    public class DescriptionBaseSO : SerializableScriptableObject
    {
        [TextArea]
        public string description;
    }
}