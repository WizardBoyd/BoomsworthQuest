using UnityEditor;
using UnityEngine;

namespace Mkey
{
    [CustomEditor(typeof(SceneCurve))]
    public class SceneCurveEditor : Editor
    {
        private SceneCurve curve;
        private Transform handleTransform;
        private Quaternion handleRotation;
        private const float directionScale = 0.5f;
        private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };
        private const float handleSize = 0.04f;
        private const float pickSize = 0.06f;
        private int selectedIndex = -2;

        private void OnSceneGUI()
        {
            curve = target as SceneCurve;
            handleTransform = curve.transform;
            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

            ShowControlPoints();
            Handles.color = Color.gray;
            ShowPivot();
        }

        void OnEnable()
        {
            curve = target as SceneCurve;
            if (curve && !curve.Created) curve.SetInitial();
        }

        private Vector3 ShowControlPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(curve.GetHandlePosition(index));
            float size = HandleUtility.GetHandleSize(point);
            if (index == 0)
            {
                size *= 2f;
            }
            if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
            {
                selectedIndex = index;
                Repaint();
            }
            if (selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Move Point");
                    EditorUtility.SetDirty(curve);
                    curve.ChangePoint(index, handleTransform.InverseTransformPoint(point));
                }
            }
            return point;
        }

        private void ShowControlPoints()
        {
            if(!curve)
            curve = target as SceneCurve;

            for (int i = 0; i < curve.HandlesCount; i++)
            {
                ShowControlPoint(i);
            }
        }

        private Vector3 ShowPivot()
        {
            Vector3 point = curve.transform.position;
            float size = HandleUtility.GetHandleSize(point);
            Handles.color = Color.red;

            if (Handles.Button(point, handleRotation, 4f * handleSize, 4f * pickSize, Handles.RectangleHandleCap))
            {
                selectedIndex = -1;
                Repaint();
            }
            return point;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if(!curve)
            curve = target as SceneCurve;
            if (!curve)
            {

                return;
            }
            DrawPointsInspector();

            if (selectedIndex >= 0 && selectedIndex < curve.HandlesCount - 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Press to add or remove curve control point.");

                if (GUILayout.Button("Add Point"))
                {
                    Undo.RecordObject(curve, "Add Point");
                    EditorUtility.SetDirty(curve);
                    curve.AddPoint(selectedIndex);
                }

                if (curve.HandlesCount > 3)
                {
                    if (GUILayout.Button("Remove Point"))
                    {
                        Undo.RecordObject(curve, "Remove Point");
                        EditorUtility.SetDirty(curve);
                        curve.RemovePoint(selectedIndex);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            if (selectedIndex >= 0 && selectedIndex < curve.HandlesCount)
            {
                DrawSelectedPointInspector();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Select curve control point for curve edit.");
                EditorGUILayout.EndHorizontal();
            }

            if (!curve.Created)
            {
                if (GUILayout.Button("Create curve"))
                {
                    curve.SetInitial();
                }
            }
            else
            {
                if (GUILayout.Button("Rebuild curve"))
                {
                    curve.SetInitial();
                }
            }
        }

        private void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point");
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", curve.GetHandlePosition(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                curve.ChangePoint(selectedIndex, point);
                EditorUtility.SetDirty(curve);
            }

            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(curve);
            }
        }

        private void DrawPointsInspector()
        {
            GUILayout.Label("Handles ");

            for (int i = 0; i < curve.HandlesCount; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 point = EditorGUILayout.Vector3Field("Position " + i, curve.GetHandlePosition(i));
             
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Move Point");
                    curve.ChangePoint(i, point);
                    EditorUtility.SetDirty(curve);
                }
            }

            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(curve);
            }
        }

    }
}