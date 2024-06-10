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
    
    private void Start()
    {
        //Start Loading the Persistent Manager Scene
        _PersistentManagerScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += On_PersistentManagerSceneLoaded;
    }

    private async void On_PersistentManagerSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        SceneManager.UnloadSceneAsync(0);
    }




}
