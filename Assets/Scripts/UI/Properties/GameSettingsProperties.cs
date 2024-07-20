using System;
using WizardSave;
using WizardUI.Window;

namespace UI.Properties
{
    [Serializable]
    public class GameSettingsProperties : WindowProperties
    {
        public AutoSaveKeyValueStoreWrapper KeyValueStore { get; private set; }
        
        public GameSettingsProperties(AutoSaveKeyValueStoreWrapper data)
        {
            KeyValueStore = data;
        }
    }
}