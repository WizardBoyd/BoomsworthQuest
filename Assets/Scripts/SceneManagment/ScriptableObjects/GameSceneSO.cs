using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SceneManagment.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scene/GameScene")]
    public class GameSceneSO : ScriptableObject
    {
        public GameSceneType SceneType;
        public AssetReference SceneReference;
        
        public enum GameSceneType
        {
            //Special Scenes
            Initialisation,
            PersistentManagers,
            Gameplay,
            
            //Status Scenes
            Level,
            Menu
            
        }
    }
}