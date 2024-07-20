using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using WizardAddressables.Runtime.AssetManagement;

public class InitializerLoader : MonoBehaviour
{
    [SerializeField] private GameSceneSO _PersistentManagerScene = default;
    [SerializeField] private GameSceneSO _MainGameSceneToLoad = default;

    [Header("Broadcasting On")]
    [SerializeField] private AssetReferenceT<LoadSceneEventChannelSO> m_menuLoadChannel = null;
    
    private AsyncOperationHandle<LoadSceneEventChannelSO> m_loadLevelEventChannelHandle;
    
    private void Start()
    {
        //Start Loading the Persistent Manager Scene
        _PersistentManagerScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += On_PersistentManagerSceneLoaded;
    }

    private void On_PersistentManagerSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
      
        if (!AssetManager.Instance.TryGetOrLoadObjectAsync<LoadSceneEventChannelSO>(m_menuLoadChannel,
                out m_loadLevelEventChannelHandle))
        {
            m_loadLevelEventChannelHandle.Completed += LoadMainGameScene;
        }
        else
        {
            LoadMainGameScene(m_loadLevelEventChannelHandle);
        }
    }
    
    private void LoadMainGameScene(AsyncOperationHandle<LoadSceneEventChannelSO> eventChannel)
    {
        eventChannel.Result.RaiseEvent(_MainGameSceneToLoad, true);
        SceneManager.UnloadSceneAsync(0);
    }
    
    




}
