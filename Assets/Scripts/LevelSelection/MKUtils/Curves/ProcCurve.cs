using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System;
using System.Runtime.InteropServices;
/*
08102019 - start
*/
namespace Mkey
{
    public class ProcCurve
    {
        /// <summary>
        /// Create arc  between 2 points with h;  if(h>0) left position
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pointsCount"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Vector3[] SimpleCurve2D(Vector3 start, Vector3 end, int pointsCount, float h)
        {
            Vector3[] res = new Vector3[pointsCount];
            res[0] = start;
            res[pointsCount - 1] = end;

            Vector3 dir = end - start;
            Vector3 delTadir = dir / (pointsCount - 1);
            if (dir.x != 0)
            {
                float xN = -(dir.y / dir.x) * h;
                Vector3 normalDir = new Vector3(xN, h, 0);
                normalDir = normalDir.normalized * Mathf.Abs(h);
                for (int i = 1; i < pointsCount - 1; i++)
                {
                    res[i] = start + delTadir * i + normalDir;
                }
            }
            else
            {
                for (int i = 1; i < pointsCount - 1; i++)
                {
                    res[i] = start + delTadir * i;
                }
            }
            return res;
        }

        /// <summary>
        /// Retun parabola points between 2 points and h;  if(h>0) left position y = h-a*x*x else y = a*x*x - h 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pointsNumber"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static List<Vector2> ArcCurve2D(Vector2 start, Vector2 end, int sidePointsNumber, float h)
        {
            List<Vector2> res = new List<Vector2>(sidePointsNumber * 2 + 1);
            List<Vector2> rRes = new List<Vector2>(sidePointsNumber+1); // right side
            sidePointsNumber = (sidePointsNumber <= 0) ? 1 : sidePointsNumber;
            
            Vector2 dPos = end - start;
            Vector2 dPosD2 = dPos / 2; ;
            float dPosM = dPos.magnitude;
            float dPosMD2 = dPosM/2;

            Vector2 dirN = Perpendicular(dPos, false).normalized;
            float a = (dPosMD2!=0) ? - h / (dPosMD2*dPosMD2) : 0;
            Vector2 dPosI = dPosD2 / sidePointsNumber;
            float dPosMI = dPosMD2 / sidePointsNumber;

            for (int i = 0; i < sidePointsNumber; i++)
            {
                float x = i * dPosMI;
                float y = a * x*x + h;
                rRes.Add(start + dPosD2 + dPosI * i + dirN * y);
            }
            rRes.Add(end);

            res.Add(start);
            for (int i = sidePointsNumber -1; i > 0; i--)
            {
                float di = i * dPosMI;
                float y = a * di * di + h;
                res.Add(start + dPosD2 - dPosI * i + dirN * y);
            }
            res.AddRange(rRes);
          //  Debug.Log("a = " + a);
          //  Debug.Log("n = " + dirN);

          //  Debug.Log(res.Count);
            return res;
        }

        /// <summary>
        /// return points array along circle from start point
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="center">rotate center</param>
        /// <param name="angle">degree</param>
        /// <param name="step"></param>
        /// <param name="pointsCount"></param>
        /// <returns></returns>
        public static Vector3[] CirclePointsAroundZ(Vector3 startPoint, Vector3 center, float degAngle, int pointsCount)
        {
            Vector3[] cList = new Vector3[pointsCount];
            cList[0] = startPoint;
            float radAngle = degAngle* Mathf.Deg2Rad;
           // float startAngle = Mathf.Deg2Rad * Vector2.Angle((startPoint - center), Vector2.right);
            float startAngle = Mathf.Deg2Rad *  MathMk.GetFullAngleOX((startPoint - center));
            float cRadius = Vector3.Distance(startPoint, center);
            float dAngle = radAngle / (pointsCount);

            for (int i = 1; i < pointsCount; i++)
            {
                float newAngle = startAngle + i * dAngle;
                Vector3 pos = center + cRadius * new Vector3(Mathf.Cos(newAngle), Mathf.Sin(newAngle), 0);
                cList[i] = pos;
            }
            return cList;
        }

        /// <summary>
        /// Return two points on circle, for two tangents from outer point 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="circleRadius"></param>
        /// <param name="outerPoint"></param>
        /// <returns></returns>
        public static Vector2 [] GetTangenteToCircleFromOuterPoint(Vector3 center, float circleRadius, Vector3 outerPoint)
        {
            float r0 = circleRadius;
            float r02 = r0 * r0;
            float d = Vector2.Distance(new Vector2(center.x, center.y), new Vector2(outerPoint.x, outerPoint.y));
            float d2 = d * d;
            Vector2[] result = new Vector2[2];

            if (d < r0)
            {
                Debug.Log("No tangents, point inside circle");
                result[0] = center;
                result[1] = center;
                return result;
            }

            Vector2 P0P1 = new Vector2(outerPoint.x, outerPoint.y) - new Vector2(center.x, center.y);
            float r12 = d2 - r02;

            float a = (r02 - r12 + d2)/(2f*d);
            Vector2 P0 = new Vector2(center.x, center.y);
            Vector2 P0P2 = P0P1 / d * a;

            float h = Mathf.Sqrt(r02 - a * a);
            Vector2 P2P3 = Perpendicular(P0P1, true).normalized * h;
            Vector2 P31 =P0 + P0P2 + P2P3; // first point on circle
            Vector2 P32 =P0 + P0P2 - P2P3; // second point on circle
            result[0] = P31;
            result[1] = P32;

            return result;
        }

        /// <summary>
        /// return perpendicular to vector V the some length as V , if (left) from left side Vector V, or right
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Perpendicular(Vector2 v, bool left)
        {
            return (left) ? new Vector2(-v.y, v.x) : new Vector2(v.y, -v.x);
        }

        /// <summary>
        /// Return array of points on arc inbetween two degree angles 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="circleRadius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="pointsCount"></param>
        /// <returns></returns>
        public static void ArcPoints(Vector3 center, float circleRadius,  float startAngle, float endAngle, int pointsCount, out Vector3 [] result)
        {
            float radStart = Mathf.Deg2Rad * startAngle;
            result = new Vector3[pointsCount];
            if (pointsCount == 0 || pointsCount == 1 || startAngle == endAngle)
            {
                result = new Vector3[] { new Vector3(Mathf.Cos(radStart), Mathf.Sin(radStart), 0) * circleRadius };
            }

            float radEnd = Mathf.Deg2Rad * endAngle;
            float dAngle = (radEnd - radStart) / (pointsCount - 1);
            for (int i = 0; i < pointsCount; i++)
            {
                float currAngle = radStart + i * dAngle;
                result[i] = center + new Vector3(Mathf.Cos(currAngle), Mathf.Sin(currAngle), 0) * circleRadius;
            }
        }

        /// <summary>
        /// Return array of points on arc inbetween two degree angles 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="circleRadius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="pointsCount"></param>
        /// <returns></returns>
        public static Vector3[] ArcPoints(Vector3 center, float circleRadius, Vector2 startDirection, Vector2 endDirection,  int pointsCount)
        {
            float radStart = Mathf.Deg2Rad * MathMk.GetFullAngleOX(startDirection);
            Debug.Log("startAngle" + MathMk.GetFullAngleOX(startDirection));
            if (pointsCount == 0 || pointsCount == 1 || startDirection == endDirection)
            {
                return new Vector3[] { new Vector3(Mathf.Cos(radStart), Mathf.Sin(radStart), 0) * circleRadius };
            }
            Vector3[] result = new Vector3[pointsCount];
            float radEnd = Mathf.Deg2Rad * MathMk.GetFullAngleOX(endDirection);
            Debug.Log("endAngle" + MathMk.GetFullAngleOX(endDirection));
            float dAngle = (radEnd - radStart) / (pointsCount - 1);

            for (int i = 0; i < pointsCount; i++)
            {
                float currAngle = radStart + i * dAngle;
                result[i] =center + new Vector3(Mathf.Cos(currAngle), Mathf.Sin(currAngle), 0) * circleRadius;
            }
            return result;
        }

        private static T[] FromByteArray<T>(byte[] source) where T : struct
        {
            T[] destination = new T[source.Length / Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                Marshal.Copy(source, 0, pointer, source.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        private static byte[] ToByteArray<T>(T[] source) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                byte[] destination = new byte[source.Length * Marshal.SizeOf(typeof(T))];
                Marshal.Copy(pointer, destination, 0, destination.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }
        //https://www.codeproject.com/Articles/19472/Convert-a-byte-array-to-a-struct-which-contains-ma
    }
}

/* Procedural curves 
  changes
    08.10.2019
    - working release
 */

