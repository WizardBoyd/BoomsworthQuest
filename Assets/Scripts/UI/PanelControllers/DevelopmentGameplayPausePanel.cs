using System;
using Events.ScriptableObjects;
using UI.Properties;
using UI.Widgets;
using UnityEngine;
using WizardSave;
using WizardUI.Panel;
using WizardUI.ScreenTransitions;

namespace UI.PanelControllers
{
    public class DevelopmentGameplayPausePanel : APanelController<GameSettingsProperties>
    {
#if UNITY_EDITOR
        
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
        [SerializeField] 
        private EventEmitterButton m_skipLevelButton;
        
        
        [Header("Animations")]
        [SerializeField]
        private ATransitionComponent m_SliderAnimIn;
        [SerializeField]
        private ATransitionComponent m_SliderAnimOut;

        private bool bIsSliderOutVisable = false;


        protected override void Awake()
        {
            base.Awake();
            Debug.Log("DevelopmentGameplayPausePanel Awake");
        }
        

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("DevelopmentGameplayPausePanel OnDestroy");
        }

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
            bool isSoundOn = !Properties.KeyValueStore.GetBool("SoundOn");
            m_SoundEffectBtn.OnToggle(isSoundOn);
            Properties.KeyValueStore.SetBool("SoundOn", isSoundOn);
        }

        private void PauseButtonPressed()
        {
            Action OnFinishedAnimation = () =>
            {
                m_PauseButton.interactable = true;
                m_SurrenderButton.interactable = true;
                m_SoundEffectBtn.Button.interactable = true;
                m_skipLevelButton.interactable = true;
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
            m_skipLevelButton.interactable = false;
        }
        

        protected override void OnPropertiesSet()
        {
            bool isSoundOn = Properties.KeyValueStore.GetBool("SoundOn");
            m_SoundEffectBtn.OnToggle(isSoundOn);
        }

        protected override void WhileHiding()
        {
            
        }

        protected override void HierarchyFixOnShow()
        {
            
        }
#endif
    }
}