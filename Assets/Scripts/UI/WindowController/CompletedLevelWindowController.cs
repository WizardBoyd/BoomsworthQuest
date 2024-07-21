using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using TMPro;
using UI.Properties;
using UnityEngine;
using UnityEngine.UI;
using WizardUI.Window;

namespace UI.WindowController
{
    public class CompletedLevelWindowController : AWindowController<LevelCompleteWindowProperties>
    {
        
        [SerializeField]
        private TMP_Text m_LifeCountText;
        
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
            m_LifeCountText.text = Properties.CurrentLifeData.CurrentLifeCount.ToString();
            if(Properties.CurrentLifeData.CurrentLifeCount == 0)
                m_nextLevelButton.interactable = false;
            else
                m_nextLevelButton.interactable = true;
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