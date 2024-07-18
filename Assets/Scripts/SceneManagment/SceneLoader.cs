using System;
using System.Collections;
using DependencyInjection;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Input;
using Misc.Singelton;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SceneManagment
{
    /// <summary>
    /// This class manages the scene loading and unloading.
    /// </summary>
    public class SceneLoader : MonoBehaviorSingleton<SceneLoader>
    { 
            //The Persistent Gameplay Manager Scene
            [SerializeField] private GameSceneSO m_gameplayScene = default;
            
        [Header("Listening To")]
        //Channel to listen to for loading gameplay levels
        [SerializeField] private LoadLevelEventChannelSO m_LoadLevel = default;
        //Channel to listen to for loading the main level selection screen or other menu based scenes
        [SerializeField] private LoadSceneEventChannelSO m_LoadMenu = default;
#if UNITY_EDITOR
        //Channel to listen to if in editor play mode we don't start with the bootstrapper scene (initialization scene)
        [SerializeField] private LoadSceneEventChannelSO m_coldStartupLocation = default;
#endif
        
        [Header("Broadcasting On")]
        //Channel to broadcast on when we want to toggle the loading screen on or off
        [SerializeField] private BoolEventChannelSO m_toggleLoadingScreen = default;
        //Channel to broadcast on when the scene that is currently loading is finally ready
        [SerializeField] private VoidEventChannelSO m_SceneReadyChannel = default;
        //Channel to broadcast on when the scene needs to transition using a fading.
        [SerializeField] private FadeEventChannelSO m_FadeRequestChannel = default;
        
        //Op Handle for the current loading scene
        private AsyncOperationHandle<SceneInstance> m_loadingOperationHandle;
        //Op Handle for the persistent gameplay manager scene which should only be non-null during level gameplay
        private AsyncOperationHandle<SceneInstance> m_gameplayManagerLoadingOpHandle;

        //The current Scene to load wrapped in GameSceneSO
        private GameSceneSO m_sceneToLoad = null;
        //The currently loaded scene wrapped in GameSceneSO
        private GameSceneSO m_currentlyLoadedScene = null;
        //Should the loading screen be showed during transition
        private bool m_showLoadingScreen;

        //reference to the scene instance for the persistent gameplay manager scene
        private SceneInstance m_gameplayManagerSceneInstance = new SceneInstance();
        //the duration of fade-in and fade-out the scene transition emits
        private float m_fadeDuration = .5f;
        //is the Scene Loader currently loading a scene helps prevent a new load request while already handling one.
        private bool m_isLoading = false;

        //This is for when loading out of game back into menu level we want to wait before loading back into menu
        private AsyncOperation m_currentlyLoadedPhysicsScene = null;
        
        [Inject]
        private TouchInputReader m_touchInputReader;

        private void OnEnable()
        {
            m_LoadLevel.OnLoadingRequested += LoadLevel;
            m_LoadMenu.OnLoadingRequested += LoadMenu;
#if UNITY_EDITOR
            m_coldStartupLocation.OnLoadingRequested += ColdStartUp;
#endif
        }

        private void OnDisable()
        {
            m_LoadLevel.OnLoadingRequested -= LoadLevel;
            m_LoadMenu.OnLoadingRequested -= LoadMenu;
#if UNITY_EDITOR
            m_coldStartupLocation.OnLoadingRequested -= ColdStartUp;
#endif
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Special loading function only used in the editor, when developer presses play in a level scene without passing initialization
        /// </summary>
        /// <param name="currentlyOpenedScene">Currently Opened Level</param>
        /// <param name="showLoadingScreen">show loading screen before going into level</param>
        /// <param name="fadeScreen">fade into level screen</param>
        private void ColdStartUp(GameSceneSO currentlyOpenedScene, bool showLoadingScreen, bool fadeScreen)
        {
                m_currentlyLoadedScene = currentlyOpenedScene;
                if (m_currentlyLoadedScene.SceneType == GameSceneSO.GameSceneType.Level)
                {
                        //Gameplay managers is loaded synchronously
                        m_gameplayManagerLoadingOpHandle =
                                m_gameplayScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                        //Blocking Call to wait
                        m_gameplayManagerLoadingOpHandle.WaitForCompletion();
                        m_gameplayManagerSceneInstance = m_gameplayManagerLoadingOpHandle.Result;
                        //Scene is ready
                        StartCoroutine(ColdStartUp());
                }
        }

        private IEnumerator ColdStartUp()
        {
                yield return new WaitForSecondsRealtime(1);
                //Inject Dependencies
                Injector.Instance.PerformInjection();
                m_SceneReadyChannel.RaiseEvent();
        }
#endif
            
            /// <summary>
            /// loads the gameplay level 
            /// </summary>
            /// <param name="levelToLoad"> the level wanting to be opened</param>
            /// <param name="showLoadingScreen">show loading screen before going into level</param>
            /// <param name="fadeScreen">fade into level screen</param>
            private void LoadLevel(GameSceneSO levelToLoad, bool showLoadingScreen, bool fadeScreen)
            {
                    //Prevent a double loading request
                    if(m_isLoading)
                            return;

                    m_sceneToLoad = levelToLoad;
                    m_showLoadingScreen = showLoadingScreen;
                    m_isLoading = true;
                    
                    //In case we are going from the the level selection screen we need to load the gameplay manager scene first
                    if (!m_gameplayManagerSceneInstance.Scene.isLoaded)
                    {
                            m_gameplayManagerLoadingOpHandle =
                                    m_gameplayScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                            m_gameplayManagerLoadingOpHandle.Completed += OnGameplayPersistentManagerLoaded;
                    }
                    else
                    {
                            StartCoroutine(UnloadPreviousScene());
                    }
            }
            
            private void LoadMenu(GameSceneSO menuToLoad, bool showLoadingScreen, bool fadeScreen)
            {
                    //Prevent a double loading request
                    if(m_isLoading)
                            return;
                    
                    m_sceneToLoad = menuToLoad;
                    m_showLoadingScreen = showLoadingScreen;
                    m_isLoading = true;
                    
                    //In case we are coming from a game level to a menu type scene we need to get rid of the gameplay managers scene
                    if (m_gameplayManagerSceneInstance.Scene.isLoaded)
                    {
                            Addressables.UnloadSceneAsync(m_gameplayManagerLoadingOpHandle, true);
                    }

                    StartCoroutine(UnloadPreviousScene());
            }

            private void OnGameplayPersistentManagerLoaded(AsyncOperationHandle<SceneInstance> obj)
            {
                    m_gameplayManagerSceneInstance = obj.Result;
                    StartCoroutine(UnloadPreviousScene());
            }



           

            private IEnumerator UnloadPreviousScene()
            {
                    //TODO disable all input on mobile screen
                    m_FadeRequestChannel.FadeOut(m_fadeDuration);

                    yield return new WaitForSeconds(m_fadeDuration);
                    m_touchInputReader.SetIsAppCurrentlyInteractable(false);
                    if (m_showLoadingScreen)
                    {
                            m_toggleLoadingScreen.RaiseEvent(true);
                    }
                    
                    //would be null if the game was started in Initialisation 
                    if (m_currentlyLoadedScene != null)
                    {
                            //TODO for saving retain a list of pertinent objects then on the monobehavior of destory just save
                            if (m_currentlyLoadedScene.SceneReference.OperationHandle.IsValid())
                            {
                                    //unload the scene through its assetReference i.e through the addressable system
                                    m_currentlyLoadedScene.SceneReference.UnLoadScene();
                            }
#if UNITY_EDITOR
                            else
                            {
                                    //Only used when, after a cold start, the player moves to a new scene
                                    //but since the Op handle has not been used 
                                    //the scene needs to be unloaded using regular scene manager instead of an addressable
                                    SceneManager.UnloadSceneAsync(
                                            m_currentlyLoadedScene.SceneReference.editorAsset.name);
                            }            
#endif
                    }

                    if (m_currentlyLoadedPhysicsScene != null)
                    {
                            yield return m_currentlyLoadedPhysicsScene;
                            m_currentlyLoadedPhysicsScene = null;
                    }

                    LoadNewScene();
            }

            public void UnloadPhysicsScene(Scene physicsScene)
            {
                    if (physicsScene.isLoaded)
                    {
                            m_currentlyLoadedPhysicsScene = SceneManager.UnloadSceneAsync(physicsScene);
                    }
            }
            

            private void LoadNewScene()
            {
                    m_loadingOperationHandle =
                            m_sceneToLoad.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
                    m_loadingOperationHandle.Completed += OnNewSceneLoaded;
                    SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
            }

            private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
            {
                    SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
                    //Inject Dependencies
                    //Injection of the dependencies here ensures that the dependencies
                    //should be available before any start method is called according to the documentation
                    Injector.Instance.PerformInjection();
            }

            private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
            {
                    m_currentlyLoadedScene = m_sceneToLoad;
                    Scene scene = obj.Result.Scene;
                    SceneManager.SetActiveScene(scene);
                    m_touchInputReader.SetIsAppCurrentlyInteractable(true);
                    //We are no longer loading at this point
                    m_isLoading = false;
                    
                    //Turn off the loading screen if it was on
                    if(m_showLoadingScreen)
                            m_toggleLoadingScreen.RaiseEvent(false);
                    
                    //Fade into the level
                    m_FadeRequestChannel.FadeIn(m_fadeDuration);
                    
                    //The scene is now ready to play emit event
                    m_SceneReadyChannel.RaiseEvent();
            }
    }
}