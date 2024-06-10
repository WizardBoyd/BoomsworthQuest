using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "Level/LevelScene", order = 0)]
    public class LevelSceneSO : ScriptableObject
    {
        [SerializeField]
        private AssetReference _SceneReference;
        public AssetReference SceneReference
        {
            get => _SceneReference;
        }

        [SerializeField] 
        private int _LevelIndex;
        public int LevelIndex
        {
            get => _LevelIndex;
        }
    }
}