using System;
using System.Collections.Generic;
using Misc.Editor;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditorInternal;
using UnityEngine;

namespace Levels.Editor
{
    [CustomEditor(typeof(MapMaker))]
    public class MapMakerEditor : UnityEditor.Editor
    {
        private ReorderableList list;
        private MapMaker mapMaker;
        private List<SerializedObject> zonesSO;
        private List<SerializedObject> prefList;
        private static bool innerChange = false;

        private MapType OldMapType = MapType.Horizontal;

        private void OnEnable()
        {
            mapMaker = target as MapMaker;
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("zones"), true, true, true,
                true);

            list.onRemoveCallback += RemoveCallback;
            list.drawElementCallback += OnDrawCallback;
            list.onAddCallback += OnAddCallBack;
            list.onSelectCallback += OnSelectCallback;
            list.drawHeaderCallback += DrawHeaderCallBack;
            list.onChangedCallback += OnChangeCallback;
            list.onAddDropdownCallback += OnAddDropDownCallBack;

            zonesSO = new List<SerializedObject>();
            mapMaker.AddComponents(true, true, mapMaker.mapType);
        }

        private void OnDisable()
        {
            if (list != null)
            {
                list.onRemoveCallback -= RemoveCallback;
                list.drawElementCallback -= OnDrawCallback;
                list.onAddCallback -= OnAddCallBack;
                list.onSelectCallback -= OnSelectCallback;
                list.drawHeaderCallback -= DrawHeaderCallBack;
                list.onChangedCallback -= OnChangeCallback;
                list.onAddDropdownCallback -= OnAddDropDownCallBack;
            }
        }

        public override void OnInspectorGUI()
        {
            // help field
            EditorGUILayout.LabelField(GetType().ToString(), EditorUIUtil.guiTitleStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Simple click on \"+\" button to add new background or \"-\" button to remove. Drag the elements to rearrange backgrounds in Level Screens List ", EditorUIUtil.guiMessageStyle);
            EditorGUILayout.Space();
            
            serializedObject.Update();
            //prefabs box
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LevelPrefabs"), true);
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
            if (!mapMaker) mapMaker = (MapMaker)target;
            if (mapMaker.HasEmptyPrefabs())
            {
                EditorGUILayout.HelpBox("Set zones prefabs list.", MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            
            if (mapMaker.enabled)
            {
                mapMaker.CleanLostZones();
                
            }

            zonesSO = new List<SerializedObject>();
            if (mapMaker.zones != null)
            {
                for (int i = 0; i < mapMaker.zones.Count; i++)
                {
                    if(mapMaker.zones[i])
                        zonesSO.Add(new SerializedObject(mapMaker.zones[i]));
                }
            }

            for (int i = 0; i < zonesSO.Count; i++)
            {
                zonesSO[i].Update();
            }
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            var sProp = serializedObject.FindProperty("mapType");
            var guiContent = new GUIContent();
            guiContent.text = sProp.displayName;
            EditorGUILayout.PropertyField(sProp, guiContent);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
            if (mapMaker.mapType != OldMapType)
            {
                serializedObject.ApplyModifiedProperties();
                Debug.Log("maptype changed : " + mapMaker.mapType);
                OldMapType = mapMaker.mapType;
                mapMaker.AddComponents(true, true, mapMaker.mapType);
            }
            
            list.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
            
            if(zonesSO.Count == 0)
                return;
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Remove all screens from map.");
            if (GUILayout.Button("Clear Map"))
            {
                if (EditorUtility.DisplayDialog("Warning!", "The map will be cleared.", "Yes", "No"))
                {
                    mapMaker.Clean();
                }
            }
            EditorGUILayout.EndVertical();

            // show clean button 
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Clear and build map.");
            if (GUILayout.Button("Build Map"))
            {
                mapMaker.Build();
            }
            EditorGUILayout.EndVertical();
        }

        private void OnDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (zonesSO != null && zonesSO.Count > 0 && index < zonesSO.Count)
            {
                EditorGUI.DropShadowLabel(new Rect(
                    rect.x,
                    rect.y,
                    300,
                    EditorGUIUtility.singleLineHeight),
                    zonesSO[index].targetObject.name,
                    EditorUIUtil.guiReordStyle);
            }
        }

        private void OnSelectCallback(ReorderableList reorderableList)
        {
            LevelZone zone = mapMaker.zones[list.index];
            if(zone)
                EditorGUIUtility.PingObject(zone.gameObject);
        }

        private void DrawHeaderCallBack(Rect rect)
        {
            EditorGUI.LabelField(rect, "Level Screens");
        }

        private void OnAddDropDownCallBack(Rect buttonrect, ReorderableList reorderableList)
        {
            GenericMenu menu = new GenericMenu();
            prefList = new List<SerializedObject>();
            if (mapMaker.LevelPrefabs != null && mapMaker.LevelPrefabs.Count > 0)
            {
                for (int i = 0; i < mapMaker.LevelPrefabs.Count; i++)
                {
                    prefList.Add(new SerializedObject(mapMaker.LevelPrefabs[i]));
                }
            }

            for (int i = 0; i < prefList.Count; i++)
            {
                UnityEngine.Object obj = prefList[i].targetObject;
                menu.AddItem(new GUIContent(i.ToString() + ". " + obj.name), false, ClickHandler, obj);
            }
            menu.ShowAsContext();
        }

        private void ClickHandler(object obj)
        {
            if(mapMaker.mapType == MapType.Vertical)
                mapMaker.AddZoneAtTop((LevelZone)obj);
            else
                mapMaker.AddZone((LevelZone)obj);

            zonesSO = new List<SerializedObject>();
            if (mapMaker.zones != null)
            {
                for (int i = 0; i < mapMaker.zones.Count; i++)
                {
                    zonesSO.Add( new SerializedObject(mapMaker.zones[i]));
                }
                mapMaker.ReArrangeZones();
            }
        }

        private void RemoveCallback(ReorderableList reorderableList)
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure?", "Yes", "No"))
            {
                mapMaker.RemoveZone(list.index);
            }

            zonesSO = new List<SerializedObject>();
            for (int i = 0; i < mapMaker.zones.Count; i++)
            {
                zonesSO.Add(new SerializedObject(mapMaker.zones[i]));
            }
        }

        private void OnAddCallBack(ReorderableList reorderableList)
        {
            Debug.Log("OnAddCallBack");
        }


        private void OnChangeCallback(ReorderableList reorderableList)
        {
            innerChange = true;
            mapMaker.ReArrangeZones();
        }
    }
}