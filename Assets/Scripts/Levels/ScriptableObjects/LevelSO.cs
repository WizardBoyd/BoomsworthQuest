using System;
using Levels.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Level", menuName = "BoomsWorth/Level/New Level", order = 0)]
    public class LevelSO : ScriptableObject, IComparable<LevelSO>
    {
        [SerializeField] 
        private AssetReference m_scene;
        public AssetReference Scene
        {
            get => m_scene;
        }

        [SerializeField] private int m_levelIndex;
        public int LevelIndex {get => m_levelIndex;}
        
        public LevelCompletionStatus LevelCompletionStatus = LevelCompletionStatus.Unkown;
        
        private bool m_levelLocked = true;
        public bool LevelLocked{get=> m_levelLocked;}
        

        public int CompareTo(LevelSO other)
        {
            if (LevelIndex > other.LevelIndex)
                return 1;
            else if (LevelIndex < other.LevelIndex)
                return -1;
            else
                return 0;
        }
    }
}