using System;
using System.Collections.Generic;
using Levels.Enums;
using UnityEngine;

namespace SaveSystem.SaveData
{
    [Serializable]
    public abstract class GameData
    {
        public string Name;
    }

    [Serializable]
    public class PlayerSettingsData : GameData
    {
        [field: SerializeField]
        public bool SoundOn { get; set; }
        [field: SerializeField]
        public bool MusicOn { get; set; }
        [field: SerializeField]
        public bool SFXOn { get; set; }
    }

    [Serializable]
    public class PlayerResourceData : GameData
    {
        
    }


    [Serializable]
    public class GameProgressData : GameData
    {
        private List<GameLevelData> m_Levels = new List<GameLevelData>();
        public int LastLevelSelectedIndex { get; set; } = 0;
    }

    [Serializable]
    public class GameLevelData
    {
        public int LevelIndex { get; set; }
        public LevelCompletionStatus LevelCompletionStatus { get; set; }
        public bool LevelComplete { get; set; }
    }

    [Serializable]
    public class ApplicationStatus : GameData
    {
        public bool FirstLaunch { get; set; }
        public bool CoreFilesCreated { get; set; }
    }
}