using System;
using Levels.Enums;
using WizardUI.Window;

namespace UI.Properties
{
    [Serializable]
    public class LevelCompleteWindowProperties : WindowProperties
    {
        public LevelCompletionStatus LevelCompletionStatus { get; private set; }

        public LevelCompleteWindowProperties(LevelCompletionStatus status)
        {
            LevelCompletionStatus = status;
        }
    }
}