using System;
using Events.ScriptableObjects;
using Misc.Singelton;
using UnityEngine;

namespace UI
{
    public class LoadingInterfaceController : MonoBehaviorSingleton<LoadingInterfaceController>
    {
        [SerializeField] private GameObject m_rootLoadingInterface;
        
        [Header("Listening On")]
        [SerializeField] private BoolEventChannelSO m_toggleScreen = default;

        private void OnEnable()
        {
            m_toggleScreen.OnEventRaised += ToggleLoadingScreen;
        }

        private void OnDisable()
        {
            m_toggleScreen.OnEventRaised -= ToggleLoadingScreen;
        }

        private void ToggleLoadingScreen(bool state)
        {
            m_rootLoadingInterface.SetActive(state);
        }
    }
}