using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
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