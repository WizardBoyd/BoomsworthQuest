using System;

namespace Levels.SerializableData
{
    [Serializable]
    public class SerializableLevelDescription
    {
        public int LevelId;
        public int LevelStarRating;
        public bool Locked;
        public bool Completed;

        public SerializableLevelDescription()
        {
            LevelId = 0;
            LevelStarRating = 0;
            Locked = true;
            Completed = false;
        }
    }
}