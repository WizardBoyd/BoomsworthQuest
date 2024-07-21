using System;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Input;
using SaveSystem;
using UI.Properties;
using UnityEngine;
using UnityEngine.UI;
using WizardSave;
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

        [Header("Configuration")] 
        [SerializeField]
        private Vector2Int m_screenReferenceSize;
        [SerializeField][Range(0f,1f)] private float m_uiScaleFactor = 1.0f;
        
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
        [Inject]
        private TouchInputReader m_touchInputReader;
        
        private AutoSaveKeyValueStoreWrapper m_autoSaveKeyValueStoreWrapper;

        private void Awake()
        {
            m_autoSaveKeyValueStoreWrapper = new AutoSaveKeyValueStoreWrapper(SaveLoadSystem.Instance.GetSaveContainer("ApplicationStatus"));
            m_uiFrame = new UISettings.UIFrameBuilder(m_uiSettings).SetInstanceAndRegister().Build();
            if (m_uiFrame.TryGetComponent<CanvasScaler>(out CanvasScaler scaler))
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = m_screenReferenceSize;
                scaler.matchWidthOrHeight = m_uiScaleFactor;
            }
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

        private void CloseCurrentWindow()
        {
            m_uiFrame.CloseCurrentWindow();
            if (!m_uiFrame.IsUIFrameEmpty)
                m_touchInputReader.SetIsAppCurrentlyInteractable(true);
            else
            {
                m_touchInputReader.SetIsAppCurrentlyInteractable(false);
            }
        }

        private void OpenSettingWindow()
        {
            m_uiFrame.OpenWindow<GameSettingsProperties>(LevelSelectionScreenIds.GameSettingWindow, new GameSettingsProperties(m_autoSaveKeyValueStoreWrapper));
        } 
        
        private void OpenLanguageWindow() => m_uiFrame.OpenWindow(LevelSelectionScreenIds.LanguageSettingWindow);

        private void OnDestroy()
        {
            if(m_uiFrame != null)
                Destroy(m_uiFrame);
        }
    }
}