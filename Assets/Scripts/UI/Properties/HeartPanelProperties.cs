using Life.SerializableData;
using WizardOptimizations.Runtime.Timer;
using WizardUI;

namespace UI.Properties
{
    public class HeartPanelProperties : IScreenProperties
    {
        public ScreenPriority Priority { get; set; } = ScreenPriority.ForceForeground;
        public bool HideOnForegroundLost { get; set; } = true;
        public bool IsPopup { get; set; } = false;
        public bool SuppressPrefabProperties { get; set; } = false;
        
        public Timer Timer { get; private set; }
        
        public CurrentLifeData CurrentLifeData { get; private set; }
        
        public HeartPanelProperties(Timer timer, CurrentLifeData currentLifeData)
        {
            Timer = timer;
            CurrentLifeData = currentLifeData;
        }
    }
}