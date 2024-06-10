using System;
using System.Collections.Generic;
using Levels.ScriptableObjects;
using Mkey;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Levels
{
    [RequireComponent(typeof(RectTransform), typeof(SceneCurve))]
    public class LevelZone : MonoBehaviour
    {
        [SerializeField] private LevelButtonPool _buttonPool;

        [SerializeField] private int count;

        private List<LevelButtonView> Levelbuttons;

        [SerializeField] private bool snapButtonsToCurve = true;

        private List<Vector3> pos;
        private SceneCurve sceneCurve;

        public Vector2 ZoneSize
        {
            get { return GetComponent<RectTransform>().sizeDelta; }
        }

        private void OnValidate()
        {
            if (count < 0) count = 0;
            if (snapButtonsToCurve)
            {
                SetButtonsPositionOnCurve();
            }
        }

        public void Create()
        {
            LevelButtonView[] existingButtons = GetComponentsInChildren<LevelButtonView>();
            bool rebuildInOldPositions = false;
            Vector3[] existingPositions = new Vector3[existingButtons.Length];

            if (existingButtons.Length != 0 && count == existingButtons.Length)
            {
                rebuildInOldPositions = true;
                for (int i = 0; i < existingButtons.Length; i++)
                {
                    if (existingButtons[i])
                    {
                        existingPositions[i] = existingButtons[i].transform.position;
                    }
                    else
                    {
                        rebuildInOldPositions = false;
                        break;
                    }
                }
            }
            
            foreach (LevelButtonView button in existingButtons)
            {
                _buttonPool.Return(button);
            }
            Levelbuttons = new List<LevelButtonView>();

            for (int i = 0; i < count; i++)
            {
                if(rebuildInOldPositions) CreateButton(existingPositions[i]);
                else CreateButton();
            }
            SetButtonsPositionOnCurve();
        }

        private void CreateButton(Vector3 position)
        {
            if (Levelbuttons == null)
                Levelbuttons = new List<LevelButtonView>();
            if (!_buttonPool)
            {
                Debug.LogError("Set Button Pool");
                return;
            }
            LevelButtonView button = _buttonPool.Request();
            button.transform.localScale = transform.lossyScale;
            button.transform.SetParent(transform);
            button.transform.position = position;
            Levelbuttons.Add(button);
        }

        private void CreateButton()
        {
            if (Levelbuttons == null)
                Levelbuttons = new List<LevelButtonView>();
            if (!_buttonPool)
            {
                Debug.LogError("Set Button Pool");
                return;
            }
            LevelButtonView button = _buttonPool.Request();
            button.transform.localScale = transform.lossyScale;
            button.transform.position = transform.position;
            button.transform.SetParent(transform);
            Levelbuttons.Add(button);
        }

        private void SetButtonsPositionOnCurve()
        {
            if (snapButtonsToCurve)
            {
                if (!sceneCurve)
                    sceneCurve = GetComponent<SceneCurve>();
                if(!sceneCurve || sceneCurve.Curve == null) return;
                if (Levelbuttons == null) return;

                pos = sceneCurve.Curve.GetPositions(Levelbuttons.Count);
                if (Levelbuttons.Count > 0)
                {
                    for (int i = 0; i < Levelbuttons.Count; i++)
                    {
                        if (Levelbuttons[i])
                            Levelbuttons[i].transform.position = transform.TransformPoint(pos[i]);
                    }
                }
            }
        }
    }
    
    
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(LevelZone))]
    public class LevelZoneEditor : Editor
    {
        private LevelZone zone;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            zone = target as LevelZone;
            if (zone.gameObject.activeInHierarchy)
            {
                if (GUILayout.Button("Rebuild LevelButtons"))
                {
                    Undo.RecordObject(zone, "Rebuild LevelButtons");
                    EditorUtility.SetDirty(zone);
                    zone.Create();
                }
            }
        }
    }
    #endif
}