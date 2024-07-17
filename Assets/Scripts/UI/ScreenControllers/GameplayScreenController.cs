using System;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Input;
using SaveSystem;
using UI.Properties;
using UnityEngine;
using UnityEngine.UI;
using WizardUI;

namespace UI.ScreenControllers
{
    public class GameplayScreenController : MonoBehaviour
    {
        public sealed class GameplayScreenIds
        {
            public const string GameplayPausePanel = "GameplayPausePanel";
            public const string LevelFailedWindow = "LevelFailedWindow";
            public const string LevelCompleteWindow = "LevelCompleteWindow";
        }
        
        [Header("Configuration")] 
        [SerializeField]
        private Vector2Int m_screenReferenceSize;
        [SerializeField][Range(0f,1f)] private float m_uiScaleFactor = 1.0f;
        
        [Header("Settings")] 
        [SerializeField] private UISettings m_uiSettings = default;

        [Header("Listening On")] 
        [SerializeField]
        private VoidEventChannelSO m_OpenLoseWindow = default;
        [SerializeField]
        private VoidEventChannelSO m_OpenWinWindow = default;
        [SerializeField]
        private VoidEventChannelSO m_CloseCurrentWindow = default;
        [SerializeField]
        private VoidEventChannelSO m_PauseButtonPressed = default;

        private UIFrame m_uiFrame;
        [Inject]
        private TouchInputReader m_touchInputReader;
        
        private void Awake()
        {
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
            m_CloseCurrentWindow.OnEventRaised += CloseCurrentWindow;
            m_OpenWinWindow.OnEventRaised += OpenLevelCompletionWindow;
            m_OpenLoseWindow.OnEventRaised += OpenLevelFailedWindow;
        }

        private void OnDisable()
        {
            m_CloseCurrentWindow.OnEventRaised -= CloseCurrentWindow;
            m_OpenWinWindow.OnEventRaised -= OpenLevelCompletionWindow;
            m_OpenLoseWindow.OnEventRaised -= OpenLevelFailedWindow;
        }
        

        private void ShowHideCorePanels(bool show)
        {
            if (show)
            {
                // m_uiFrame.ShowPanel<GameSettingsProperties>(
                //     GameplayScreenIds.GameplayPausePanel,new GameSettingsProperties(SaveLoadSystem.Instance.PlayerSettingData));
            }
            else
            {
                m_uiFrame.HidePanel(GameplayScreenIds.GameplayPausePanel);
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
        
        private void OpenLevelCompletionWindow()
        {
            m_uiFrame.OpenWindow(GameplayScreenIds.LevelCompleteWindow);
            m_touchInputReader.SetIsAppCurrentlyInteractable(true);
        }
        
        private void OpenLevelFailedWindow()
        {
            m_uiFrame.OpenWindow(GameplayScreenIds.LevelFailedWindow);
            m_touchInputReader.SetIsAppCurrentlyInteractable(true);
        }
        
    }
}