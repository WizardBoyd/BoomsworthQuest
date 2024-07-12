using UnityEditor;
using UnityEngine;

namespace Levels.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        [MenuItem("Window/BoomsWorth/Level Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<LevelEditorWindow>();
            window.titleContent = new GUIContent("BoomsWorth Level Editor");
            window.Show();
        }

        private void CreateGUI()
        {
            
        }
    }
}