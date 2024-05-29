#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SaveSystem
{
    
    public class SerializableScriptableObject : ScriptableObject
    {
        [SerializeField, HideInInspector] 
        private string _guid;

        public string Guid => _guid;

#if UNITY_EDITOR
        private void OnValidate()
        {
            string path = AssetDatabase.GetAssetPath(this);
            _guid = AssetDatabase.AssetPathToGUID(path);
        }
#endif
    }
}