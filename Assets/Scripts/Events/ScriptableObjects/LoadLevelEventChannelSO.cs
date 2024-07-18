using System;
using BaseClasses;
using Levels.ScriptableObjects;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{
    /// <summary>
    /// This class is used for scene-loading events.
    /// Takes a GameSceneSO of the location or menu that needs to be loaded, and a bool to specify if a loading screen needs to display.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Load Level Event Channel")]
    public class LoadLevelEventChannelSO : LoadSceneEventChannelSO<LevelSceneSO>{}
}