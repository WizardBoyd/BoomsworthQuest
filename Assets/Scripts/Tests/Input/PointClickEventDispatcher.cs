using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tests.Input
{
    public class PointClickEventDispatcher : MonoBehaviour, IPointerClickHandler
    {
        [Header("Broadcasting On")] 
        [SerializeField]
        [Tooltip("The Pointer clicked channel can be used for just any void event channel")]
        private VoidEventChannelSO PointerClickedChannel = default;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (PointerClickedChannel != null)
            {
                PointerClickedChannel.RaiseEvent();
            }
        }
    }
}