using UnityEngine;

namespace UI.Panel
{
    public class PanelProperties : IScreenProperties
    {
        [Tooltip(
            "Pannels go to different para-layers depending on their priority. you can set up para-layers in the panel Layer")]
        [SerializeField]
        private ScreenPriority m_priority;

        public ScreenPriority Priority
        {
            get => m_priority;
            set => m_priority = value;
        }
        
        
        public bool HideOnForegroundLost { get; set; } = false;
        public bool IsPopup { get; set; } = false;
        public bool SuppressPrefabProperties { get; set; } = false;
    }
}