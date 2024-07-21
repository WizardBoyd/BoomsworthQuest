using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using TMPro;
using UI.Properties;
using UnityEngine;
using UnityEngine.UI;
using WizardUI.Window;

namespace UI.WindowController
{
    public class FailedLevelWindowController : AWindowController<LevelFailWindowProperties>
    {
        [SerializeField]
        private TMP_Text m_LifeCountText;
        
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
            m_LifeCountText.text = Properties.CurrentLifeData.CurrentLifeCount.ToString();
           if(Properties.CurrentLifeData.CurrentLifeCount == 0)
               m_RetryButton.interactable = false;
           else
               m_RetryButton.interactable = true;
        }

        protected override void WhileHiding()
        {
           
        }
        
        private void RetryLevel() => m_RetryLevel.RaiseEvent();
        private void OnBackToMenuButtonClicked() => m_loadmainMenu.RaiseEvent(m_LevelSelectScene, true, true);
    }
}