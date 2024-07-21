using System;
using Levels.Enums;
using Life.SerializableData;
using WizardUI.Window;

namespace UI.Properties
{
    [Serializable]
    public class LevelCompleteWindowProperties : WindowProperties
    {
        public LevelCompletionStatus LevelCompletionStatus { get; private set; }
        public CurrentLifeData CurrentLifeData { get; private set; }

        public LevelCompleteWindowProperties(LevelCompletionStatus status, CurrentLifeData currentLifeData)
        {
            LevelCompletionStatus = status;
            CurrentLifeData = currentLifeData;
        }
    }
}