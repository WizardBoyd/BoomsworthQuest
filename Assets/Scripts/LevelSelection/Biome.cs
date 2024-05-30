using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    public class Biome : MonoBehaviour
    {
        [SerializeField]
        public GameObject levelButtonPrefab;
        public int count;
        public List<LevelButton> levelButtons;
        [SerializeField]
        private bool snapButtonsToCurve = true;
        [HideInInspector]
        [SerializeField]
        private bool snapButtonsToCurveOld = false;

        #region temp vars
        private List<Vector3> pos;
        private SceneCurve sceneCurve;
        #endregion temp vars

        public Vector2 BiomeSize
        {
            get { return GetComponent<RectTransform>().sizeDelta; }
        }

        private void OnValidate()
        {
            if (count < 0) count = 0;
            if (snapButtonsToCurve & !snapButtonsToCurveOld)
            {
                SetButtonsPositionOnCurve();
            }
            snapButtonsToCurveOld = snapButtonsToCurve;
        }

        public void Create()
        {
            LevelButton[] existingButtons = GetComponentsInChildren<LevelButton>();

            bool rebuildInOldPositions = false;
            Vector3[] existingPositions = new Vector3[existingButtons.Length];
            if (existingButtons != null && count == existingButtons.Length)
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

            foreach (var b in existingButtons)
            {
                DestroyImmediate(b.gameObject);
            }
            levelButtons = new List<LevelButton>();

            for (int i = 0; i < count; i++)
            {
                if (rebuildInOldPositions) CreateButton(existingPositions[i]);
                else CreateButton();
            }
            SetButtonsPositionOnCurve();
        }

        private void CreateButton()
        {
            if (levelButtons == null) levelButtons = new List<LevelButton>();
            if (!levelButtonPrefab)
            {
                Debug.Log("Set level buttons prefab");
                return;
            }
            GameObject b = Instantiate(levelButtonPrefab);
            b.transform.localScale = transform.lossyScale;
            b.transform.position = transform.position;
            b.transform.SetParent(transform);
            levelButtons.Add(b.GetComponent<LevelButton>());
        }

        private void CreateButton(Vector3 position)
        {
            if (levelButtons == null) levelButtons = new List<LevelButton>();
            if (!levelButtonPrefab)
            {
                Debug.Log("Set level buttons prefab");
                return;
            }
            GameObject b = Instantiate(levelButtonPrefab);
            b.transform.localScale = transform.lossyScale;
            b.transform.SetParent(transform);
            b.transform.position = position;
            levelButtons.Add(b.GetComponent<LevelButton>());
        }

        public void SetButtonsPositionOnCurve()
        {
            if (snapButtonsToCurve)
            {
                if (!sceneCurve) sceneCurve = GetComponent<SceneCurve>();
                if (!sceneCurve || sceneCurve.Curve == null) return;
                if (levelButtons == null) return;

                pos = sceneCurve.Curve.GetPositions(levelButtons.Count);
                if (levelButtons.Count > 0)
                {
                    for (int i = 0; i < levelButtons.Count; i++)
                    {
                        if (levelButtons[i]) levelButtons[i].transform.position = transform.TransformPoint(pos[i]);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Biome))]
    public class BiomeEditor : Editor
    {
        private Biome biome;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            biome = target as Biome;
            if (biome.gameObject.activeInHierarchy)
            {
                if (GUILayout.Button("ReBuild LevelButtons"))
                {
                    Undo.RecordObject(biome, "ReBuild  LevelButtons");
                    EditorUtility.SetDirty(biome);
                    biome.Create();
                }
            }
        }
    }
#endif
}