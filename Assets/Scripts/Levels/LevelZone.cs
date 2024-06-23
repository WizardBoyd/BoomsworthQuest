using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagment;
using AssetManagment.ConcreteReferences;
using Factory;
using Levels.ScriptableObjects;
using Mkey;
using Pool;
using SaveSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Levels
{
    [RequireComponent(typeof(RectTransform), typeof(SceneCurve))]
    public class LevelZone : MonoBehaviour
    {
        [SerializeField] 
        private ComponentPoolSO<LevelButton> m_buttonPool;
        [SerializeField] 
        private ZoneSo m_Zone;

        private List<LevelSO> m_levels;

        private List<LevelButton> m_buttons;
        
        private SceneCurve m_sceneCurve;

        private void Awake()
        {
            m_sceneCurve = GetComponent<SceneCurve>();
            AssetReferences();
            m_levels = new List<LevelSO>(m_Zone.GetLevelsInZone());
            m_buttonPool.Prewarm(m_levels.Count);
        }
        

        private void AssetReferences()
        {
            Assert.IsNotNull(m_buttonPool, "The Button Pool Is not referenced");
            Assert.IsNotNull(m_Zone, "Zone has not been assigned in editor");
            Assert.IsNotNull(m_sceneCurve, "Scene Curve has not been assigned");
        }

        public void SpawnLevelButtons()
        {
            m_buttons = new List<LevelButton>(m_buttonPool.Request(m_levels.Count));
            List<Vector3> positions = m_sceneCurve.Curve.GetPositions(m_buttons.Count);
            m_levels.Sort();
            for (int i = 0; i < m_buttons.Count; i++)
            {
                m_buttons[i].AssignedLevel = m_levels[i];
                m_buttons[i].transform.localScale = transform.lossyScale;
                m_buttons[i].transform.SetParent(transform);
                
                m_buttons[i].transform.position = transform.TransformPoint(positions[i]);
            }
        }


        public Vector2 ZoneSize
        {
            get { return GetComponent<RectTransform>().sizeDelta; }
        }
    }
    
    
    
    // #if UNITY_EDITOR
    // [CustomEditor(typeof(LevelZone))]
    // public class LevelZoneEditor : Editor
    // {
    //     private LevelZone zone;
    //
    //     public override void OnInspectorGUI()
    //     {
    //         DrawDefaultInspector();
    //         zone = target as LevelZone;
    //         if (zone.gameObject.activeInHierarchy)
    //         {
    //             if (GUILayout.Button("Rebuild LevelButtons"))
    //             {
    //                 Undo.RecordObject(zone, "Rebuild LevelButtons");
    //                 EditorUtility.SetDirty(zone);
    //                 zone.Create();
    //             }
    //         }
    //     }
    // }
    // #endif
}