using System;
using System.Collections.Generic;
using UnityEngine;

namespace Misc.Curves
{
    [Serializable]
    public class PolyLine : CommonCurve
    {
        public PolyLine(List<Vector3> handlesPositions) : base(handlesPositions)
        {
            CacheDistances();
        }

        public override void Display(Transform transform)
        {
            if(HandlesCount < 2)
                return;
            for (int i = 0; i < HandlesCount - 1; i++)
            {
                Gizmos.DrawLine(transform.TransformPoint(handlesPositions[i]), transform.TransformPoint(handlesPositions[i + 1]));
            }
        }

        private void CacheDistances()
        {
            int count = HandlesCount;
            distances = new float[count];

            float accumulateDistance = 0;
            for (int i = 0; i < count; i++)
            {
                Vector3 t1 = handlesPositions[(i) % count];
                Vector3 t2 = handlesPositions[(i + 1) % count];

                distances[i] = accumulateDistance;
                accumulateDistance += (t1 - t2).magnitude;
            }
        }
    }
}