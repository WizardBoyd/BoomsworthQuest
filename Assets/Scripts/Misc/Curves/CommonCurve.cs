using System;
using System.Collections.Generic;
using UnityEngine;

namespace Misc.Curves
{
    /// <summary>
    /// Common curve class
    /// </summary>
    [Serializable]
    public class CommonCurve
    {
        [Tooltip("Local positions of curve handles")]
        public List<Vector3> handlesPositions;

        [SerializeField]
        [HideInInspector]
        public List<Vector3> controlPositions; // cached intermediate positons (catjmull rom)

        [SerializeField]
        [HideInInspector]
        public float[] distances;

        #region properties

        /// <summary>
        /// Return the curve path length
        /// </summary>
        public float Length
        {
            get
            {
                if (distances == null || distances.Length == 0)
                    return 0f;
                return distances[^1];
            }
        }

        /// <summary>
        /// Return how many handles are in the curve
        /// </summary>
        public int HandlesCount => handlesPositions?.Count ?? 0;

        #endregion

        public CommonCurve(List<Vector3> handlesPositions)
        {
            this.handlesPositions = handlesPositions;
        }

        public virtual void Display(Transform transform)
        {
            Debug.Log("Base Display");
        }

        public virtual void RemovePoint(int selectedIndex)
        {
            Debug.Log("Remove point: " + selectedIndex);
            handlesPositions.RemoveAt(selectedIndex);
        }

        public virtual void AddPoint(int selectedIndex)
        {
            Debug.Log("Add point: " + selectedIndex);
            Vector3 p0 = handlesPositions[selectedIndex];
            Vector3 p1 = handlesPositions[selectedIndex + 1];
            Vector3 pn = (p1 + p0) / 2.0f;
            
            handlesPositions.Insert(selectedIndex + 1, pn);
        }

        public virtual void AddPoint(Vector3 position)
        {
            handlesPositions.Add(position);
        }

        public virtual void ChangePoint(int selectedIndex, Vector3 position)
        {
            handlesPositions[selectedIndex] = position;
        }

        public virtual Vector3 GetPosition(float dist)
        {
            return Vector3.zero;
        }

        public virtual List<Vector3> GetPositions(int count)
        {
            return new List<Vector3>();
        }

        protected int ClampListPos(int pos, int count)
        {
            if (pos < 0)
                pos = count - 1;
            if (pos > count)
                pos = 1;
            else if (pos > count - 1)
                pos = 0;

            return pos;
        }

        public Vector3 GetHandlePosition(int index)
        {
            return handlesPositions[index];
        }
        
        
    }
}