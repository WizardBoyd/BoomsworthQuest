using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    public abstract class BaseSceneChangeAction : ScriptableObject
    {
        public abstract void PerformAction();
    }
}