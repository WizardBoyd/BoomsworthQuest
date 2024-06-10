using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Misc.Curves
{
    [ExecuteInEditMode]
    public enum DisplayMode {None, Display, DisplaySelected}

    public class SceneCurve : MonoBehaviour
    {
        [HideInInspector] [SerializeField] private CatmullRomSpline curve;
        [SerializeField] private DisplayMode displayMode = DisplayMode.Display;
        [SerializeField] private Color color = Color.white;

        [HideInInspector] [SerializeField] private bool created = false;

        #region Properties

        public bool Created
        {
            get { return created; }
        }

        /// <summary>
        /// Return handles count
        /// </summary>
        public int HandlesCount
        {
            get
            {
                if (!created)
                    return 0;
                return curve.HandlesCount;
            }
        }

        /// <summary>
        /// Return path length
        /// </summary>
        public float Length
        {
            get
            {
                if (!created)
                    return 0;
                return curve.Length;
            }
        }

        public CatmullRomSpline Curve => curve;
        #endregion

        [Space(8, order = 0)] [Header("Curve Change Event", order = 1)]
        public UnityEvent OnChangeCurveEvent;

        private void Awake()
        {

            if (!Created)
                SetInitial();
        }
        

        private void OnDrawGizmos()
        {
            if (displayMode == DisplayMode.Display)
            {
                Gizmos.color = color;
                if(Created)
                    curve.Display(transform);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (displayMode == DisplayMode.DisplaySelected)
            {
                Gizmos.color = color;
                if(Created)
                    curve.Display(transform);
            }
        }
        
        public void SetInitial()
        {
            Vector3[] points = null;
            RectTransform rT = GetComponent<RectTransform>();
            if (rT)
            {
                Vector2 sd2 = rT.sizeDelta / 2;
                Vector2 sd6 = rT.sizeDelta / 6;
                points = new Vector3[4]
                {
                    new Vector3(0f, -sd2.y, 0f),
                    new Vector3(0f, -sd6.y, 0f),
                    new Vector3(0f, sd6.y, 0f),
                    new Vector3(0f, sd2.y, 0f)
                };
            }
            else
            {
                points = new Vector3[3]
                {
                    Vector3.left,
                    Vector3.down,
                    Vector3.right
                };
            }

            curve = new CatmullRomSpline(new List<Vector3>(points));
            OnChangeCurveEvent?.Invoke();
            created = true;
            Debug.Log("Created SceneCurve: " + created);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        public void RemovePoint(int selectedIndex)
        {
            if(curve == null)
                return;
            curve.RemovePoint(selectedIndex);
            OnChangeCurveEvent?.Invoke();
        }

        public void AddPoint(int selectedIndex)
        {
            if(curve == null)
                return;
            curve.AddPoint(selectedIndex);
            OnChangeCurveEvent?.Invoke();
        }

        public void AddPoint(Vector3 position)
        {
            if(curve == null)
                return;
            curve.AddPoint(position);
            OnChangeCurveEvent?.Invoke();
        }

        public void ChangePoint(int selectedIndex, Vector3 position)
        {
            if(curve == null)
                return;
            curve.ChangePoint(selectedIndex, position);
            OnChangeCurveEvent?.Invoke();
        }

        public Vector3 GetHandlePosition(int index)
        {
            return curve.GetHandlePosition(index);
        }

        // public void MoveAlongPath(GameObject gObject,  float time, float delay, Ease easeMode,
        //     Action onCompleteCallback)
        // {
        //     gObject.transform.DOPath()
        // }
    }
}