using UnityEngine;

namespace UI
{
    /// <summary>
    /// This is the centralized access point for all things UI
    /// All the calls should be directed at this
    /// </summary>
    public class UIFrame : MonoBehaviour
    {
        /// <summary>
        /// Should the UI frame initialize itself on the awake callback
        /// </summary>
        [SerializeField]
        private bool InitializeOnAwake = true;
        
    }
}