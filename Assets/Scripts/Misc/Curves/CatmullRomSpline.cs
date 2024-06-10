using System;
using System.Collections.Generic;
using UnityEngine;

namespace Misc.Curves
{
    [Serializable]
    public class CatmullRomSpline : CommonCurve
    {
        public CatmullRomSpline(List<Vector3> handlesPositions) : base(handlesPositions)
        {
            CreateControlPositions();
            CacheDistances();
        }

        
        /// <summary>
        /// Return List of position on curve with equal distances inbetween
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public override List<Vector3> GetPositions(int count)
        {
            List<Vector3> pos = new List<Vector3>();
            float delta = (Length) / (count - 1);
            for (int i = 0; i < count - 1; i++)
            {
                pos.Add(GetPosition(i * delta));
            }
            pos.Add(GetPosition(Length));
            return pos;
        }

        /// <summary>
        /// Return position on spline at distance
        /// </summary>
        /// <param name="dist"></param>
        /// <returns></returns>
        public override Vector3 GetPosition(float dist)
        {
            int point = 0;
            int numPoints = distances.Length;
            float pathLength = Length;

            dist = (dist == pathLength) ? pathLength : Mathf.Repeat(dist, pathLength);

            while (point < distances.Length - 1 && distances[point] <= dist)
            {
                ++point;// Debug.Log("point: "+point + " ; dis: " + dist);
            }

            int p1n = ((point - 1) + numPoints) % numPoints;
            int p2n = point;

            float t = Mathf.InverseLerp(distances[p1n], distances[p2n], dist);

            int p0n = ((point - 2) + numPoints) % numPoints;
            int p3n = (point + 1) % numPoints;
            p2n = p2n % numPoints;

            Vector3 P0 = controlPositions[p0n];
            Vector3 P1 = controlPositions[p1n];
            Vector3 P2 = controlPositions[p2n];
            Vector3 P3 = controlPositions[p3n];
            return GetCatmullRomPosition(t, P0, P1, P2, P3);
        }

        public override void RemovePoint(int selectedIndex)
        {
            if(HandlesCount < 4)
                return;
            Debug.Log("Remove point: " + selectedIndex);
            handlesPositions.RemoveAt(selectedIndex);
            CreateControlPositions();
            CacheDistances();
        }

        public override void ChangePoint(int selectedIndex, Vector3 position)
        {
            base.ChangePoint(selectedIndex, position);
            CreateControlPositions();
            CacheDistances();
        }

        public override void AddPoint(int selectedIndex)
        {
            base.AddPoint(selectedIndex);
            CreateControlPositions();
            CacheDistances();
        }

        public override void AddPoint(Vector3 position)
        {
            base.AddPoint(position);
            CreateControlPositions();
            CacheDistances();
        }

        public override void Display(Transform transform)
        {
            if (HandlesCount < 2) return;
            for (int i = 0; i < controlPositions.Count; i++)
            {
                if ((i == 0 || i == controlPositions.Count - 2 || i == controlPositions.Count - 1))
                {
                    continue;
                }
                Display(transform, i);
            }
        }

        private void Display(Transform transform, int pos)
        {
            Vector3 p0 = controlPositions[ClampListPos(pos - 1, controlPositions.Count)];
            Vector3 p1 = controlPositions[pos];
            Vector3 p2 = controlPositions[ClampListPos(pos + 1, controlPositions.Count)];
            Vector3 p3 = controlPositions[ClampListPos(pos + 2, controlPositions.Count)];

            Vector3 lastPos = p1;

            float resolution = 0.2f;

            int loops = Mathf.FloorToInt(1f / resolution);

            for (int i = 1; i <= loops; i++)
            {
                float t = i * resolution;
                Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                Gizmos.DrawLine(transform.TransformPoint(lastPos), transform.TransformPoint(newPos));
                lastPos = newPos;
            }
        }

        /// <summary>
        /// Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        /// http://www.iquilezles.org/www/articles/minispline/minispline.htm
        /// </summary>
        private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }

        /// <summary>
        /// Create positions list for curve calculating
        /// </summary>
        private void CreateControlPositions()
        {
            // hidden duplicate 0 point
            controlPositions = new List<Vector3> { handlesPositions[0]};
            handlesPositions.ForEach((p) => { controlPositions.Add(p); });
            controlPositions.Add(handlesPositions[HandlesCount - 1]); // hidden duplicate last point
        }

        private void CacheDistances()
        {
            int count = controlPositions.Count;
            distances = new float[count];

            float accumulateDistance = 0;
            for (int i = 0; i < count; ++i)
            {
                var t1 = controlPositions[(i) % count];
                var t2 = controlPositions[(i + 1) % count];
                
                distances[i] = accumulateDistance;
                accumulateDistance += (t1 - t2).magnitude;
                
            }
        }
    }
}