using Levels.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Events/Level Completion Event Channel")]
    public class LevelCompletionEventChannelSO : ScriptableObject
    {
        public UnityAction<LevelCompletionStatus> OnEventRaised;
        
        public void RaiseEvent(LevelCompletionStatus value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}