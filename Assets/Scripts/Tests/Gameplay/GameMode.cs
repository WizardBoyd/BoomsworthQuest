using System;
using Events.ScriptableObjects;
using Misc.Singelton;
using UnityEngine;

namespace Tests.Gameplay
{
    public class GameMode : MonoBehaviorSingleton<GameMode>
    {
        [Header("Listening On")] 
        [SerializeField]
        private VoidEventChannelSO OnTargetHitChannel = default;
        

        private void OnEnable()
        {
            OnTargetHitChannel.OnEventRaised += OnTargetHit;
        }
        
        private void OnDisable()
        {
            OnTargetHitChannel.OnEventRaised -= OnTargetHit;
        }
        

        private void OnTargetHit()
        {
            Debug.Log("Target has Been Hit");
        }
    }
}