using UnityEngine;

namespace Misc.Math
{
    public class MathUtil
    {
        /// <summary>
        ///  Return full (360 deg) angle between v and OX axe;
        /// </summary>
        public static float GetFullAngleOX(Vector2 v)
        {
            float a = 0f;
            a = Vector2.Angle(v, Vector2.right);
            if (v.y >= 0)
                return a;
            else
            {
                return 360f - a;
            }
        }

        public static int Lerp(int a, int b, float t)
        {
            t = Mathf.Clamp01(t);
            int d = b - a;
            return a + Mathf.RoundToInt((d * t));
        }

        public static long Lerp(long a, long b, float t)
        {
            t = Mathf.Clamp01(t);
            long d = b - a;
            return a + (long)System.Math.Round(d * t);
        }
        
        public static double Lerp(double a, double b, float t)
        {
            t = Mathf.Clamp01(t);
            double d = b - a;
            return a + (d * (double)t);
        }
        
        public static decimal Lerp(decimal a, decimal b, float t)
        {
            t = Mathf.Clamp01(t);
            decimal d = b - a;
            return a + (d * (decimal)t);
        }
        
    }
}