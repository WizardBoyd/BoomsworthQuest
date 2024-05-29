using BaseClasses;
using Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Events/Toogle Interaction UI Event Channel")]
    public class InteractionUIEventChannelSO : DescriptionBaseSO
    {
        public UnityAction<bool, InteractionType> OnEventRaised;

        public void RaiseEvent(bool state, InteractionType interactionType)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(state, interactionType);
        }
    }
}