using System;
using System.Collections.Generic;
using Mkey;
using UnityEngine;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Zone", menuName = "BoomsWorth/Level/New Zone", order = 0)]
    public class ZoneSO : ScriptableObject
    {
        [SerializeField] 
        public int ZoneIndex;
        
        [SerializeField]
        public LevelSceneSO[] Levels;
        
        [SerializeField]
        public ZoneCurveLayoutSO ZoneCurveLayout;
    }
}