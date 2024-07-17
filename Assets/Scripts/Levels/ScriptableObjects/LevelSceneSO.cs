using System;
using Levels.Enums;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "Level/LevelScene", order = 0)]
    public class LevelSceneSO : GameSceneSO
    {
        [SerializeField]
        private int m_levelIndex;
        public int LevelIndex
        {
            get => m_levelIndex;
        }

        public bool LevelLocked { get; set; } = false;
        
        public LevelCompletionStatus LevelCompletionStatus { get; set; }
        
        private void OnValidate()
        {
            SceneType = GameSceneType.Level;
        }
    }
}