using Events.ScriptableObjects;
using UI.Properties;
using UI.Widgets;
using UnityEngine;
using WizardSave;
using WizardUI.Panel;

namespace UI.PanelControllers
{
    public class GameplayPauseSlideOut :  APanelController<GameSettingsProperties>
    {
        
        [Header("ReferencedButtons")] 
        [SerializeField]
        private ToggleButton m_SoundEffectBtn;
        
        protected override void AddListeners()
        {
            m_SoundEffectBtn.Button.onClick.AddListener(ToggleSoundButtonPressed);
        }
        
        protected override void RemoveListeners()
        {
            m_SoundEffectBtn.Button.onClick.RemoveListener(ToggleSoundButtonPressed);
        }

        protected override void OnPropertiesSet()
        {
            bool soundOn = false;
            if(Properties.KeyValueStore.TryGetBool("SoundOn", out soundOn) == false)
            {
                m_SoundEffectBtn.OnToggle(soundOn);
            }
        }

        protected override void WhileHiding()
        {
           
        }

        protected override void HierarchyFixOnShow()
        {
            
        }
        
        private void ToggleSoundButtonPressed()
        {
            bool soundOn = !Properties.KeyValueStore.GetBool("SoundOn", true);
            m_SoundEffectBtn.OnToggle(soundOn);
            Properties.KeyValueStore.SetBool("SoundOn", soundOn);
        }
    }
}