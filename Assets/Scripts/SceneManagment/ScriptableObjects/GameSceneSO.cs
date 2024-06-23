

using System.Collections.Generic;
using Audio.AudioData;
using BaseClasses;
using SceneManagment.ScriptableObjects.Actions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SceneManagment.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scene/Game Scene")]
    public class GameSceneSO : ScriptableObject
    {
        public GameSceneType SceneType;
        public AssetReference SceneReference;

        [Header("Actions To Perform On Entering")] [SerializeField]
        public List<BaseSceneChangeAction> OnEnterSceneActions = new List<BaseSceneChangeAction>();
        [Header("Actions to Perform On Leaving")]
        [SerializeField]  public List<BaseSceneChangeAction> OnLeaveSceneActions = new List<BaseSceneChangeAction>();
        
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