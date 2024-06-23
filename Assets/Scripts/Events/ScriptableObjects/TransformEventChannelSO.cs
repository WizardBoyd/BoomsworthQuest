using BaseClasses;
using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Events/Transform Event Channel")]
    public class TransformEventChannelSO : ScriptableObject
    {
        public UnityAction<Transform> OnEventRaised;

        public void RaiseEvent(Transform value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}