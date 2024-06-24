using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor
{
    [CustomEditor(typeof(SaveLoadSystem))]
    public class SaveLoadSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SaveLoadSystem saveLoadSystem = (SaveLoadSystem)target;
            string playerSettingsName = saveLoadSystem.PlayerSettingData.Name;

            DrawDefaultInspector();

            // if (GUILayout.Button("Save Player Settings"))
            // {
            //     saveLoadSystem.SavePlayerSettings();
            // }
            //
            // if (GUILayout.Button("Load Player Settings"))
            // {
            //     saveLoadSystem.LoadPlayerSettings(playerSettingsName);
            // }
            //
            // if (GUILayout.Button("Delete Player Settings"))
            // {
            //     saveLoadSystem.DeleteFile(playerSettingsName);
            // }
        }
    }
}