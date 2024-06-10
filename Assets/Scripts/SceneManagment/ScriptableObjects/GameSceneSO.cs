

using Audio.AudioData;
using BaseClasses;
using UnityEngine.AddressableAssets;

namespace SceneManagment.ScriptableObjects
{
    public class GameSceneSO : DescriptionBaseSO
    {

        public GameSceneType SceneType;
        public AssetReference SceneReference;
        public AudioCueSO musicTrack;
        
        
        public enum GameSceneType
        {
            //Special Scenes
            Initialisation,
            PersistentManagers,
            Gameplay,
            MainMenu,
            
            //Status Scenes
            FailedLevelScene,
            WinLevelScene,
            
        }
    }
}