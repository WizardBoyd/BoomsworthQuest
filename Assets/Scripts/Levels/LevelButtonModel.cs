using BaseClasses;
using Levels.ScriptableObjects;
using UnityEngine;

namespace Levels
{
    public enum LevelState {Locked, UnCompleted, Completed}
    
    [System.Serializable]
    public class LevelButtonModel : BaseModel
    {
        public LevelSceneSO level;
        public LevelState LevelState;

        public int StarCount = -1;

    }
    
}