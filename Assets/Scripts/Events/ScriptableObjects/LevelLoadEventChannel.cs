using BaseClasses;
using Levels.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Events/Load Level Event Channel")]
    public class LevelLoadEventChannel : DescriptionBaseSO
    {
        public UnityAction<LevelSceneSO, bool, bool> OnLoadingRequested;

        public void RaiseEvent(LevelSceneSO LevelToLoad, bool showLoadingScreen = false, bool fadeScreen = false)
        {
            if (OnLoadingRequested != null)
            {
                OnLoadingRequested.Invoke(LevelToLoad, showLoadingScreen, fadeScreen);
            }
            else
            {
                Debug.LogWarning("A Scene loading was requested, but nobody picked it up. " +
                                 "Check why there is no SceneLoader already present, " +
                                 "and make sure it's listening on this Load Event channel.");
            }
        }
    }
}