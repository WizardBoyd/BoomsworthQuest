using System;
using UnityEngine;

namespace UI.Window
{
    [Serializable]
    public class WindowProperties : IScreenProperties
    {
        [SerializeField] protected bool m_hideOnForegroundLost = true;
        [SerializeField] protected ScreenPriority m_windowQueuePriority = ScreenPriority.ForceForeground;
        [SerializeField] protected bool m_isPopUp = false;

        public ScreenPriority Priority
        {
            get => m_windowQueuePriority;
            set => m_windowQueuePriority = value;
        }
        public bool HideOnForegroundLost
        {
            get => m_hideOnForegroundLost;
            set => m_hideOnForegroundLost = value;
        }
        public bool IsPopup
        {
            get => m_isPopUp;
            set => m_isPopUp = value;
        }
        public bool SuppressPrefabProperties { get; set; }
    }
}