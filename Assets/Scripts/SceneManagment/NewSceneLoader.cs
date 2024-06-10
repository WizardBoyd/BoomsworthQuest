using System.Threading.Tasks;
using Events.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace SceneManagment
{
    public class NewSceneLoader
    {
        [Header("Data")] 
        [SerializeField] private GameSceneSO _gameplayManagerScene;

        [Header("Broadcasting On")] 
        [SerializeField]
        private VoidEventChannelSO _onSceneReady = default;
        [SerializeField]
        private BoolEventChannelSO _onToggleLoadingScreen = default;

        private AsyncOperationHandle<SceneInstance> _gameplayManagerLoadingOpHandle;
        private AsyncOperationHandle<SceneInstance> _loadingOperationHandle;

        private bool _isLoading = false;
        private GameSceneSO _sceneToLoad;
        private GameSceneSO _currentlyLoadedScene;
        private bool _showLoadingScreen;
        
        private SceneInstance _gameplayManagerSceneInstance = new SceneInstance();
        private float _fadeDuration = .5f;
        

        /// <summary>
        /// This function loads the Game Level (aka: forest level 1)
        /// </summary>
        private void LoadGameLevel(GameSceneSO LevelToLoad, bool showLoadingScreen, bool fadeScreen)
        {
            //Prevent double loading
            if(_isLoading)
                return;
            _sceneToLoad = LevelToLoad;
            _showLoadingScreen = showLoadingScreen;
            _isLoading = true;
            
            //check to see if the gameplay persistent manager scene is loaded
            //if its not begin the loading process
            if (!_gameplayManagerSceneInstance.Scene.isLoaded)
            {
                // _gameplayManagerLoadingOpHandle 
            }
        }
        
        /// <summary>
        /// This functions loads the main menu level, removing any gameplay manager that may be in the scene as well
        /// </summary>
        private void LoadMainMenu(){}
        
        /// <summary>
        /// In the menu loading handles unloading whatever the previous scene was 
        /// </summary>
        private async Task UnloadPreviousScene(){}
        
        /// <summary>
        /// Kicks off the asynchronous of loading a new scene
        /// </summary>
        private void LoadNewScene(){}
        
        /// <summary>
        /// Callback for when the new scene has been loaded in
        /// </summary>
        private void OnNewSceneLoaded(){}
        
        /// <summary>
        /// Callback for when everthing is ready and gameplay can begin in the new scene
        /// </summary>
        private void StartGameplay(){}

        /// <summary>
        /// Callback for when the game should begin to end
        /// </summary>
        private void ExitGame(){}
    }
}