using System;
using Events.ScriptableObjects;
using UI.Properties;
using UI.Widgets;
using UnityEngine;
using WizardUI.Panel;
using WizardUI.ScreenTransitions;

namespace UI.PanelControllers
{
    public class GameplayPausePanel : APanelController<GameSettingsProperties>
    {
        [Header("Listen On")] 
        [SerializeField] 
        private VoidEventChannelSO m_PauseButtonPressed = default;
        [SerializeField]
        private VoidEventChannelSO m_toggleSound = default;
        
        [Header("ReferencedButtons")] 
        [SerializeField]
        private ToggleButton m_SoundEffectBtn;
        [SerializeField] 
        private EventEmitterButton m_PauseButton;
        [SerializeField] 
        private EventEmitterButton m_SurrenderButton;
        
        [Header("Animations")]
        [SerializeField]
        private ATransitionComponent m_SliderAnimIn;
        [SerializeField]
        private ATransitionComponent m_SliderAnimOut;

        private bool bIsSliderOutVisable = false;
        
        
        protected override void AddListeners()
        {
            m_PauseButtonPressed.OnEventRaised += PauseButtonPressed;
            m_toggleSound.OnEventRaised += ToggleSoundButtonPressed;
        }

        protected override void RemoveListeners()
        {
            m_PauseButtonPressed.OnEventRaised += PauseButtonPressed;
            m_toggleSound.OnEventRaised -= ToggleSoundButtonPressed;
        }

        private void ToggleSoundButtonPressed()
        {
            Properties.PlayerSettingsData.SoundOn = !Properties.PlayerSettingsData.SoundOn;
            m_SoundEffectBtn.OnToggle(Properties.PlayerSettingsData.SoundOn);
        }

        private void PauseButtonPressed()
        {
            Action OnFinishedAnimation = () =>
            {
                m_PauseButton.interactable = true;
                m_SurrenderButton.interactable = true;
                m_SoundEffectBtn.Button.interactable = true;
            };
            DisableInteractivity();
            if (bIsSliderOutVisable)
            {
                m_SliderAnimOut.Animate(m_SliderAnimOut.transform,OnFinishedAnimation);
            }
            else
            {
                m_SliderAnimIn.Animate(m_SliderAnimIn.transform,OnFinishedAnimation);
            }
            bIsSliderOutVisable = !bIsSliderOutVisable;
        }
        
        private void DisableInteractivity()
        {
            m_PauseButton.interactable = false;
            m_SurrenderButton.interactable = false;
            m_SoundEffectBtn.Button.interactable = false;
        }
        

        protected override void OnPropertiesSet()
        {
            m_SoundEffectBtn.OnToggle(Properties.PlayerSettingsData.SoundOn);
        }

        protected override void WhileHiding()
        {
            
        }

        protected override void HierarchyFixOnShow()
        {
            
        }
        
    }
}