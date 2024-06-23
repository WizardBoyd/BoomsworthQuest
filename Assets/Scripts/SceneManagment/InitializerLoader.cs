using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using TaskManagment.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using TaskScheduler = TaskManagment.TaskScheduler;

public class InitializerLoader : MonoBehaviour
{
    [SerializeField] private GameSceneSO _PersistentManagerScene = default;
    [SerializeField] private GameSceneSO _MainGameSceneToLoad = default;

    [Header("Broadcasting On")]
    [SerializeField] private AssetReferenceT<LoadEventChannelSO> m_menuLoadChannel = null;
    
    private void Start()
    {
        //Start Loading the Persistent Manager Scene
        _PersistentManagerScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += On_PersistentManagerSceneLoaded;
    }

    private void On_PersistentManagerSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        m_menuLoadChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += LoadMainGameScene;
    }

    private void LoadMainGameScene(AsyncOperationHandle<LoadEventChannelSO> obj)
    {
        obj.Result.RaiseEvent(_MainGameSceneToLoad, true);
        SceneManager.UnloadSceneAsync(0);
    }
    
    




}
