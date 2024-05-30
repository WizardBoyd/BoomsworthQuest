using UnityEngine;
namespace Mkey
{
    public class EditorUIUtil
    {
        public static GUIStyle guiTitleStyle
        {
            get
            {
                var guiTitleStyle = new GUIStyle(GUI.skin.label);
                guiTitleStyle.normal.textColor = Color.black;
                guiTitleStyle.fontSize = 16;
                guiTitleStyle.fixedHeight = 30;

                return guiTitleStyle;
            }
        }

        public static GUIStyle guiMessageStyle
        {
            get
            {
                var messageStyle = new GUIStyle(GUI.skin.label);
                messageStyle.wordWrap = true;

                return messageStyle;
            }
        }

        public static GUIStyle guiReordStyle
        {
            get
            {
                var guiTitleStyle = new GUIStyle(GUI.skin.textArea);
                guiTitleStyle.fixedHeight = 20;
                guiTitleStyle.alignment = TextAnchor.MiddleLeft;
                return guiTitleStyle;
            }
        }
    }
}
