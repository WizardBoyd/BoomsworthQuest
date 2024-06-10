using System;
using Misc.Editor;
using UnityEditor;

namespace SaveSystem.Editor
{
    [CustomEditor(typeof(SaveSystem))]
    public class SaveSystemEditor : UnityEditor.Editor
    {
        private SaveSystem _saveSystem;

        private void OnEnable()
        {
            _saveSystem = target as SaveSystem;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("SaveSystem", EditorUIUtil.guiTitleStyle);
            EditorGUILayout.Space();
           
            serializedObject.Update();
            
            //Prefabs Box
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_audioData"), true);
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_levelProgressData"), true);
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            if (_saveSystem.AudioData == null)
            {
                EditorGUILayout.HelpBox("Set Audio Data Scriptable object", MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }

        }
    }
}