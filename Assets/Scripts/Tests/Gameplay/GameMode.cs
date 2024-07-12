using System;
using Events.ScriptableObjects;
using Misc.Singelton;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tests.Gameplay
{
    public class GameMode : MonoBehaviorSingleton<GameMode>
    {
        [FormerlySerializedAs("OnTargetHitChannel")]
        [Header("Listening On")] 
        [SerializeField]
        private VoidEventChannelSO m_onTargetHitChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_onLevelSurrenderChannel = default;

        [Header("Broadcasting On")] 
        [SerializeField]
        private VoidEventChannelSO m_onLevelCompleteChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_onLevelFailedChannel = default;

        private void OnEnable()
        {
            m_onTargetHitChannel.OnEventRaised += OnTargetHit;
            m_onLevelSurrenderChannel.OnEventRaised += OnLevelSurrender;
        }

        private void OnDisable()
        {
            m_onTargetHitChannel.OnEventRaised -= OnTargetHit;
            m_onLevelSurrenderChannel.OnEventRaised -= OnLevelSurrender;
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
    }
}