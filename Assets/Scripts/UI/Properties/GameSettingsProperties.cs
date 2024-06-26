using System;
using SaveSystem.SaveData;
using WizardUI.Window;

namespace UI.Properties
{
    [Serializable]
    public class GameSettingsProperties : WindowProperties
    {
        public PlayerSettingsData PlayerSettingsData { get; private set; }

        public GameSettingsProperties(PlayerSettingsData data)
        {
            PlayerSettingsData = data;
        }
    }
}