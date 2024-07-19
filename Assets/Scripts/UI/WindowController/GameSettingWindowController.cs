using Events.ScriptableObjects;
using UI.Properties;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using WizardSave;
using WizardUI.Window;

namespace UI.WindowController
{
    public class GameSettingWindowController : AWindowController<GameSettingsProperties>
    {
        [Header("Listening On")] 
        [SerializeField]
        private VoidEventChannelSO m_toggleSound = default;
        [SerializeField]
        private VoidEventChannelSO m_toggleSFX = default;

        [Header("Referenced Buttons")] 
        [SerializeField] private ToggleButton m_SFXButton;
        [SerializeField] private ToggleButton m_SoundButton;
        //[SerializeField] private ToggleButton m_NotificationButton;
        
        
        protected override void AddListeners()
        {
            m_toggleSound.OnEventRaised += ToggleSound;
            m_toggleSFX.OnEventRaised += ToggleSFX;
            
        }
        
        protected override void RemoveListeners()
        {
            m_toggleSound.OnEventRaised -= ToggleSound;
            m_toggleSFX.OnEventRaised -= ToggleSFX;
        }

        protected override void OnPropertiesSet()
        {
            bool isSoundOn = Properties.KeyValueStore.GetBool("SoundOn");
            m_SoundButton.OnToggle(isSoundOn);
        }

        protected override void WhileHiding()
        {
          
        }
        
        
        private void ToggleSound()
        {
            bool isSoundOn = !Properties.KeyValueStore.GetBool("SoundOn");
            m_SoundButton.OnToggle(isSoundOn);
            Properties.KeyValueStore.SetBool("SoundOn", isSoundOn);
        }
        
        private void ToggleSFX()
        {
            //Properties.PlayerSettingsData.SFXOn = !Properties.PlayerSettingsData.SFXOn;
           // m_SFXButton.OnToggle(Properties.PlayerSettingsData.SFXOn);
        }

    }
}