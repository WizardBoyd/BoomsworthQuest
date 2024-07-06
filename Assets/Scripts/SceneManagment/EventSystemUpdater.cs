using System;
using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SceneManagment
{
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemUpdater : MonoBehaviour
    {
        private EventSystem m_eventSystem;

        [Header("Listening On")] [SerializeField]
        private VoidEventChannelSO On_SceneReadyChannel = default;

        private void Awake()
        {
            m_eventSystem = GetComponent<EventSystem>();
        }

        private void OnEnable()
        {
            On_SceneReadyChannel.OnEventRaised += OnSceneReady;
            
        }

        private void OnDisable()
        {
            On_SceneReadyChannel.OnEventRaised -= OnSceneReady;
        }

        void OnSceneReady()
        {
           if(m_eventSystem != null)
               m_eventSystem.UpdateModules();
        }
    }
}