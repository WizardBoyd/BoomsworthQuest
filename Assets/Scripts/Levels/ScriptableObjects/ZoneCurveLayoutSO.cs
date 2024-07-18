using Misc.Curves;
using Mkey;
using UnityEngine;

namespace Levels.ScriptableObjects
{
    public class ZoneCurveLayoutSO : ScriptableObject
    {
        [SerializeField]
        public Sprite ZoneImage;

        [SerializeField] 
        public CRCurve Curve;

        public Vector2 referenceResolution
        {
            get => ZoneImage != null ? new Vector2(ZoneImage.rect.width, ZoneImage.rect.height) : Vector2.zero;
        }
    }
}