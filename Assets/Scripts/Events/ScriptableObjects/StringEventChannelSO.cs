using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New String Event Channel", menuName = "Events/String Event Channel", order = 0)]
    public class StringEventChannelSO : ScriptableObject
    {
        public event UnityAction<string> OnEventRaised;

        public void RaiseEvent(string value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}