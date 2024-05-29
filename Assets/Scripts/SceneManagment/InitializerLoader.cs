using System;
using System.Collections;
using System.Collections.Generic;
using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class InitializerLoader : MonoBehaviour
{
    [SerializeField] private GameSceneSO _managersScene = default;
    [SerializeField] private GameSceneSO _menuToLoad = default;

    [Header("BroadCasting On")] [SerializeField]
    private AssetReference _menuLoadChannel = default;
    
    private void Start()
    {
        _managersScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
    }

    private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
    {
        _menuLoadChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += LoadLevelSelect;
    }

    private void LoadLevelSelect(AsyncOperationHandle<LoadEventChannelSO> obj)
    {
      obj.Result.RaiseEvent(_menuToLoad,true);
      SceneManager.UnloadSceneAsync(0);
    }
    
}
