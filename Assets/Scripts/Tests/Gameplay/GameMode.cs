using System;
using Events.ScriptableObjects;
using Levels.Enums;
using Misc.Singelton;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tests.Gameplay
{
    public class GameMode : MonoBehaviorSingleton<GameMode>
    {
        [Header("Configuration")]
        [SerializeField]
        private GameSceneSO m_LevelSelect = default;
        
        [FormerlySerializedAs("OnTargetHitChannel")]
        [Header("Listening On")] 
        [SerializeField]
        private VoidEventChannelSO m_onTargetHitChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_onLevelSurrenderChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_OnLevelFinishedChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_NaivagateToLevelSelectChannel = default;
        
        [Header("Broadcasting On")]
        [SerializeField]
        private LevelCompletionEventChannelSO m_OnLevelCompleteChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_onLevelFailedChannel = default;
        [SerializeField]
        private LoadSceneEventChannelSO m_onLoadSceneChannel = default;

        private void OnEnable()
        {
            m_onTargetHitChannel.OnEventRaised += OnTargetHit;
            m_onLevelSurrenderChannel.OnEventRaised += OnLevelSurrender;
            m_OnLevelFinishedChannel.OnEventRaised += OnLevelComplete;
            m_NaivagateToLevelSelectChannel.OnEventRaised += NavigateToLevelSelect;
        }

        private void OnDisable()
        {
            m_onTargetHitChannel.OnEventRaised -= OnTargetHit;
            m_onLevelSurrenderChannel.OnEventRaised -= OnLevelSurrender;
            m_OnLevelFinishedChannel.OnEventRaised -= OnLevelComplete;
            m_NaivagateToLevelSelectChannel.OnEventRaised -= NavigateToLevelSelect;
        }
        

        private void OnTargetHit()
        {
            Debug.Log("Target has Been Hit");
        }
        
        private void OnLevelSurrender()
        {
            Debug.Log("Level Surrendered");
            m_onLevelFailedChannel.RaiseEvent();
        }
        
        private void OnLevelComplete()
        {
            Debug.Log("Level Completed");
            m_OnLevelCompleteChannel.RaiseEvent(LevelCompletionStatus.OneStarCompletion);
        }
        
        private void NavigateToLevelSelect()
        {
            m_onLoadSceneChannel.RaiseEvent(m_LevelSelect);
        }
    }
}