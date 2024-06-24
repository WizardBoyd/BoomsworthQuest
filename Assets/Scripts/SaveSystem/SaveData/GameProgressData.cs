using System.Collections.Generic;
using System.IO;
using Levels.Enums;
using SaveSystem.Interface;

namespace SaveSystem.SaveData
{
    public class GameProgressData: GameData, IDataSave
    {
        private List<GameLevelData> m_Levels;
        public int LastLevelSelectedIndex { get; set; }

        public GameProgressData()
        {
            m_Levels = new List<GameLevelData>();
            LastLevelSelectedIndex = 0;
        }
        
        public void WriteDataToFile(BinaryWriter writer)
        {
            writer.Write(LastLevelSelectedIndex);
            writer.Write(m_Levels.Count);
            foreach (GameLevelData level in m_Levels)
            {
                level.Write(writer);
            }
        }

        public void ReadDataFromFile(BinaryReader reader)
        {
            LastLevelSelectedIndex = reader.ReadInt32();
            int levelCount = reader.ReadInt32();
            for (int i = 0; i < levelCount; i++)
            {
                GameLevelData currentRead = new GameLevelData();
                currentRead.Read(reader);
                m_Levels.Add(currentRead);
            }
        }
    }

    public class GameLevelData : IBinarySerializable
    {
        public int LevelIndex { get; set; }
        public LevelCompletionStatus LevelCompletionStatus { get; set; }
        public bool LevelComplete { get; set; }
        public void Write(BinaryWriter writer)
        {
            writer.Write(LevelIndex);
            writer.Write((int)LevelCompletionStatus);
            writer.Write(LevelComplete);
        }

        public void Read(BinaryReader reader)
        {
            LevelIndex = reader.ReadInt32();
            LevelCompletionStatus = (LevelCompletionStatus)reader.ReadInt32();
            LevelComplete = reader.ReadBoolean();
        }
    }
}