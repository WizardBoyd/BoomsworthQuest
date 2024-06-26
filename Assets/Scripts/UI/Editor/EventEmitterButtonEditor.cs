using UnityEditor;
using UnityEditor.UI;

namespace UI.Editor
{
    [CustomEditor(typeof(EventEmitterButton), true)]
    public class EventEmitterButtonEditor : SelectableEditor
    {
        private SerializedProperty m_eventChanelProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_eventChanelProperty = serializedObject.FindProperty("m_OnClickEvent");
        }

        public override void OnInspectorGUI()
        {
           base.OnInspectorGUI();
           EditorGUILayout.Space();
           
           serializedObject.Update();
           EditorGUILayout.PropertyField(m_eventChanelProperty);
           serializedObject.ApplyModifiedProperties();
        }
    }
}