using System;
using Events.ScriptableObjects;
using SaveSystem;
using UI.Properties;
using UnityEngine;
using WizardUI;

namespace UI.ScreenControllers
{
    public class LevelSelectionScreenController : MonoBehaviour
    {
        public sealed class LevelSelectionScreenIds
        {
            public const string HeartPanel = "HeartPanel";
            public const string PremiumResourcePanel = "PremiumResourcePanel";
            public const string HUDNavigationPanel = "HUDNavigationPanel";
            public const string HUDPlayPanel = "HudPlayPanel";
            public const string GameSettingWindow = "GameSettingWindow";
            public const string LanguageSettingWindow = "LanguageSettingWindow";
        }
        
        [Header("Settings")] 
        [SerializeField] private UISettings m_uiSettings = default;

        [Header("Listening On")] 
        [SerializeField]
        private VoidEventChannelSO m_CloseCurrentWindow = default;
        [SerializeField]
        private VoidEventChannelSO m_OpenSettingsWindow = default;
        [SerializeField] 
        private VoidEventChannelSO m_OpenLanguageWindow = default;

        private UIFrame m_uiFrame;

        private void Awake()
        {
            m_uiFrame = new UISettings.UIFrameBuilder(m_uiSettings).SetInstanceAndRegister().Build();
            ShowHideCorePanels(true);
        }

        private void OnEnable()
        {
           m_OpenSettingsWindow.OnEventRaised += OpenSettingWindow;
           m_CloseCurrentWindow.OnEventRaised += CloseCurrentWindow;
           m_OpenLanguageWindow.OnEventRaised += OpenLanguageWindow;
        }
        
        private void OnDisable()
        {
            m_OpenSettingsWindow.OnEventRaised -= OpenSettingWindow;
            m_CloseCurrentWindow.OnEventRaised -= CloseCurrentWindow;
            m_OpenLanguageWindow.OnEventRaised -= OpenLanguageWindow;
        }

        private void ShowHideCorePanels(bool show)
        {
            if (show)
            {
                m_uiFrame.ShowPanel(LevelSelectionScreenIds.HeartPanel);
                m_uiFrame.ShowPanel(LevelSelectionScreenIds.PremiumResourcePanel);
                m_uiFrame.ShowPanel(LevelSelectionScreenIds.HUDNavigationPanel);
                m_uiFrame.ShowPanel(LevelSelectionScreenIds.HUDPlayPanel);
            }
            else
            {
                m_uiFrame.HidePanel(LevelSelectionScreenIds.HeartPanel);
                m_uiFrame.HidePanel(LevelSelectionScreenIds.PremiumResourcePanel);
                m_uiFrame.HidePanel(LevelSelectionScreenIds.HUDNavigationPanel);
                m_uiFrame.HidePanel(LevelSelectionScreenIds.HUDPlayPanel);
            }
        }
        
        private void CloseCurrentWindow() => m_uiFrame.CloseCurrentWindow();

        private void OpenSettingWindow() => m_uiFrame.OpenWindow<GameSettingsProperties>(LevelSelectionScreenIds.GameSettingWindow, new GameSettingsProperties(SaveLoadSystem.Instance.PlayerSettingData));
        
        private void OpenLanguageWindow() => m_uiFrame.OpenWindow(LevelSelectionScreenIds.LanguageSettingWindow);
        
    }
}