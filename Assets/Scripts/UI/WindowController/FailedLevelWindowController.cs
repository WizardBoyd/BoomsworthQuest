using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using WizardUI.Window;

namespace UI.WindowController
{
    public class FailedLevelWindowController : AWindowController
    {
        [Header("Broadcasting On")]
        [SerializeField]
        private LoadSceneEventChannelSO m_loadmainMenu = default;
        [SerializeField]
        private VoidEventChannelSO m_RetryLevel = default;
        
        [Header("buttons")]
        [SerializeField] 
        private Button m_RetryButton;
        [SerializeField] 
        private Button m_backToMenuButton;
        
        [Header("Configuration")]
        [SerializeField]
        private GameSceneSO m_LevelSelectScene = default;
        
        protected override void AddListeners()
        {
            m_RetryButton.onClick.AddListener(RetryLevel);
            m_backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);
        }

        protected override void RemoveListeners()
        {
            m_RetryButton.onClick.RemoveListener(RetryLevel);
            m_backToMenuButton.onClick.RemoveListener(OnBackToMenuButtonClicked);
        }

        protected override void OnPropertiesSet()
        {
           
        }

        protected override void WhileHiding()
        {
           
        }
        
        private void RetryLevel() => m_RetryLevel.RaiseEvent();
        private void OnBackToMenuButtonClicked() => m_loadmainMenu.RaiseEvent(m_LevelSelectScene, true, true);
    }
}