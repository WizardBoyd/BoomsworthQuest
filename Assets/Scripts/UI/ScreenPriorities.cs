using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public enum ScreenPriority
    {
        None = 0,
        Prioritary = 1,
        Tutorial = 2,
        Blocker = 3,
        //Window Only Priorities
        ForceForeground = 4,
        Enqueue = 5,
    }

    [Serializable]
    public class ScreenPriorityLayerListEntry
    {
        [Tooltip("The screen priority type for a given target para-layer (Exclude Window Only Properties)")]
        [SerializeField] 
        private ScreenPriority m_priority;

        [Tooltip("The Gameobject that should house all the panels tagged with this priority")] 
        [SerializeField]
        private Transform m_targetParent;

        public Transform TargetParent
        {
            get => m_targetParent;
            set => m_targetParent = value;
        }

        public ScreenPriority Priority
        {
            get => m_priority;
            set => m_priority = value;
        }

        public ScreenPriorityLayerListEntry(ScreenPriority priority, Transform parent)
        {
            this.m_priority = priority;
            this.m_targetParent = parent;
        }
    }
    
    [Serializable]
    public class ScreenPriorityLayerList
    {
        [Tooltip(
            "A lookup of GameObjects to store panels depending on their Priority. Render priority is set by the hierarchy order of these GameObjects")]
        [SerializeField]
        private List<ScreenPriorityLayerListEntry> m_paraLayers = null;

        private Dictionary<ScreenPriority, Transform> m_lookup;

        public Dictionary<ScreenPriority, Transform> paraLayerLookup
        {
            get
            {
                if (m_lookup == null || m_lookup.Count == 0)
                    CacheLookUp();
                return m_lookup;
            }
        }

        private void CacheLookUp()
        {
            m_lookup = new Dictionary<ScreenPriority, Transform>();
            for (int i = 0; i < m_paraLayers.Count; i++)
            {
                m_lookup.Add(m_paraLayers[i].Priority, m_paraLayers[i].TargetParent);
            }
        }

        public ScreenPriorityLayerList(List<ScreenPriorityLayerListEntry> entries) => this.m_paraLayers = entries;
    }
}