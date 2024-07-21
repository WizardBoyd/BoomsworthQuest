using System;
using Life.SerializableData;
using WizardUI.Window;

namespace UI.Properties
{
    [Serializable]
    public class LevelFailWindowProperties : WindowProperties
    {
        public CurrentLifeData CurrentLifeData { get; private set; }
        
        public LevelFailWindowProperties(CurrentLifeData currentLifeData)
        {
            CurrentLifeData = currentLifeData;
        }
    }
}