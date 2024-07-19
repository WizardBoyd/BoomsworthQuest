using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UI.Properties;
using UnityEngine;
using UnityEngine.UI;
using WizardUI.Window;

namespace UI.WindowController
{
    public class CompletedLevelWindowController : AWindowController<LevelCompleteWindowProperties>
    {
        [Header("Broadcasting On")]
        [SerializeField]
        private LoadSceneEventChannelSO m_loadmainMenu = default;
        [SerializeField]
        private VoidEventChannelSO m_loadNextLevel = default;
        
        [Header("buttons")]
        [SerializeField] 
        private Button m_nextLevelButton;
        [SerializeField] 
        private Button m_backToMenuButton;
        
        [Header("Configuration")]
        [SerializeField]
        private GameSceneSO m_LevelSelectScene = default;
        
        protected override void AddListeners()
        {
            m_backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);
            m_nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        protected override void RemoveListeners()
        {
            m_backToMenuButton.onClick.RemoveListener(OnBackToMenuButtonClicked);
            m_nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
        }

        protected override void OnPropertiesSet()
        {
            
        }

        protected override void WhileHiding()
        {
           
        }
        
        private void OnBackToMenuButtonClicked()
        {
            m_loadmainMenu.RaiseEvent(m_LevelSelectScene, true, true);
        }
        
        private void OnNextLevelClicked()
        {
           m_loadNextLevel.RaiseEvent();
        }
    }
}