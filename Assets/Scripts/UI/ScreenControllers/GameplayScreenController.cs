using System;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Input;
using Levels.Enums;
using SaveSystem;
using UI.Properties;
using UnityEngine;
using UnityEngine.UI;
using WizardSave;
using WizardUI;

namespace UI.ScreenControllers
{
    public class GameplayScreenController : MonoBehaviour
    {
        public sealed class GameplayScreenIds
        {
#if UNITY_EDITOR
            public const string DevelopmentGameplayPausePanel = "DevelopmentGameplayPausePanel";
#endif
            public const string GameplayPauseSlide = "GameplayPauseSlide";
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
        private LevelCompletionEventChannelSO m_OpenWinWindow = default;
        [SerializeField]
        private VoidEventChannelSO m_CloseCurrentWindow = default;
        [SerializeField]
        private VoidEventChannelSO m_PauseButtonPressed = default;
        [SerializeField]
        private VoidEventChannelSO m_OnNextLevel = default;
        [SerializeField]
        private VoidEventChannelSO m_RetryLevel = default;

        private UIFrame m_uiFrame;
        
        [Inject]
        private TouchInputReader m_touchInputReader;
        
        [Inject]
        private AutoSaveKeyValueStoreWrapper m_autoSaveKeyValueStoreWrapper;
        
        private void Awake()
        {
            m_uiFrame = new UISettings.UIFrameBuilder(m_uiSettings).SetInstanceAndRegister().Build();
            if (m_uiFrame.TryGetComponent<CanvasScaler>(out CanvasScaler scaler))
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = m_screenReferenceSize;
                scaler.matchWidthOrHeight = m_uiScaleFactor;
            }
        }

        private void OnEnable()
        {
            m_CloseCurrentWindow.OnEventRaised += CloseCurrentWindow;
            m_OpenWinWindow.OnEventRaised += OpenLevelCompletionWindow;
            m_OpenLoseWindow.OnEventRaised += OpenLevelFailedWindow;
            m_PauseButtonPressed.OnEventRaised += OnPauseButtonPressed;
            m_OnNextLevel.OnEventRaised += ResetUI;
            m_RetryLevel.OnEventRaised += ResetUI;
        }
        

        private void OnPauseButtonPressed()
        {
            if (!m_uiFrame.IsPanelOpen(GameplayScreenIds.GameplayPauseSlide))
            {
                m_uiFrame.ShowPanel<GameSettingsProperties>
                    (GameplayScreenIds.GameplayPauseSlide, new GameSettingsProperties(m_autoSaveKeyValueStoreWrapper));
            }
            else
            {
                m_uiFrame.HidePanel(GameplayScreenIds.GameplayPauseSlide);
            }
        }

        private void OnDisable()
        {
            m_CloseCurrentWindow.OnEventRaised -= CloseCurrentWindow;
            m_OpenWinWindow.OnEventRaised -= OpenLevelCompletionWindow;
            m_OpenLoseWindow.OnEventRaised -= OpenLevelFailedWindow;
            m_PauseButtonPressed.OnEventRaised -= OnPauseButtonPressed;
            m_OnNextLevel.OnEventRaised -= ResetUI;
            m_RetryLevel.OnEventRaised -= ResetUI;
        }

        private void Start()
        {
            ShowHideCorePanels(true);
        }
        
        private void ShowHideCorePanels(bool show)
        {
            if (show)
            {
                m_uiFrame.ShowPanel(
                    GameplayScreenIds.GameplayPausePanel);
            }
            else
            {
                m_uiFrame.HidePanel(GameplayScreenIds.GameplayPausePanel);
                m_uiFrame.HidePanel(GameplayScreenIds.GameplayPauseSlide);
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
        
        private void OpenLevelCompletionWindow(LevelCompletionStatus levelCompletionStatus)
        {
            ShowHideCorePanels(false);
            m_uiFrame.OpenWindow<LevelCompleteWindowProperties>(GameplayScreenIds.LevelCompleteWindow, new LevelCompleteWindowProperties(levelCompletionStatus));
            m_touchInputReader.SetIsAppCurrentlyInteractable(true);
        }
        
        private void OpenLevelFailedWindow()
        {
            ShowHideCorePanels(false);
            m_uiFrame.OpenWindow(GameplayScreenIds.LevelFailedWindow);
            m_touchInputReader.SetIsAppCurrentlyInteractable(true);
        }
        
        private void ResetUI()
        {
            m_uiFrame.HideAll(false);
            ShowHideCorePanels(true);
        }
        
        private void OnDestroy()
        {
            if(m_uiFrame != null)
                Destroy(m_uiFrame);
        }
        
        
        
    }
}