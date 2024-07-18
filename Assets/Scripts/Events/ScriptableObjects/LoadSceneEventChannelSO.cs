using Levels.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{
    public abstract class LoadSceneEventChannelSO<T> : ScriptableObject
    where T : GameSceneSO
    {
        public UnityAction<T, bool, bool> OnLoadingRequested;
        
        public void RaiseEvent(T locationToLoad, bool showLoadingScreen = false, bool fadeScreen = false)
        {
            if (OnLoadingRequested != null)
            {
                OnLoadingRequested.Invoke(locationToLoad, showLoadingScreen, fadeScreen);
            }
            else
            {
                Debug.LogWarning("A Scene loading was requested, but nobody picked it up. " +
                                 "Check why there is no SceneLoader already present, " +
                                 "and make sure it's listening on this Load Event channel.");
            }
        }
    }
    
    [CreateAssetMenu(menuName = "Events/Load Scene Event Channel")]
    public class LoadSceneEventChannelSO : LoadSceneEventChannelSO<GameSceneSO>{}
    
    [CreateAssetMenu(menuName = "Events/Load Level Event Channel")]
    public class LoadLevelEventChannelSO : LoadSceneEventChannelSO<LevelSceneSO>{}
    
}