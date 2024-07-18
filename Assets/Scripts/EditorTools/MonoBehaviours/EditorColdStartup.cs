using System;
using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace EditorTools.MonoBehaviours
{
    /// <summary>
    /// Allows a "cold start" in the editor, when pressing play and not passing from the Initialisation Scene
    /// </summary>
    public class EditorColdStartup : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private GameSceneSO m_thisSceneSO = default;
        [SerializeField] private GameSceneSO m_PersistenManagerSO = default;
        [SerializeField] private AssetReference m_notifyColdStartupChannel = default;
        [SerializeField] private VoidEventChannelSO m_OnSceneReadyChannel = default;

        private bool IsColdStart = false;

        private void Awake()
        {
            if (!SceneManager.GetSceneByName(m_PersistenManagerSO.SceneReference.editorAsset.name).isLoaded)
            {
                IsColdStart = true;
            }
        }

        private void Start()
        {
            if (IsColdStart)
            {
                m_PersistenManagerSO.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed +=
                    LoadEventChannel;
            }
        }

        private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
        {
            m_notifyColdStartupChannel.LoadAssetAsync<LoadSceneEventChannelSO>().Completed += OnNotifyChannelLoaded;
        }

        private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadSceneEventChannelSO> obj)
        {
            if (m_thisSceneSO != null)
            {
                obj.Result.RaiseEvent(m_thisSceneSO);
            }
            else
            {
                //Raise a fake scene ready event so the player is spawned
                m_OnSceneReadyChannel.RaiseEvent();
                //when this happens, the player won't be able to move between scenes because the sceneloader has no concept of which scene we are in.
            }
        }
#endif
    }
}