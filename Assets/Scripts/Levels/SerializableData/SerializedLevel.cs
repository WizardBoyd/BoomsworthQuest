using Levels.Enums;

namespace Levels.SerializableData
{
    public class SerializedLevel
    {
        public int LevelIndex;
        public LevelCompletionStatus CompletionStatus;
        public bool CurrentlyLocked;
    }
}