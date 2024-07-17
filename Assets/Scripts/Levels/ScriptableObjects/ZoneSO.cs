using System.Collections.Generic;
using Mkey;
using UnityEngine;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Zone", menuName = "BoomsWorth/Level/New Zone", order = 0)]
    public class ZoneSO : ScriptableObject
    {
        [SerializeField]
        private ZoneRuntime m_zoneRuntime;
        public ZoneRuntime ZonePrefab => m_zoneRuntime;
        
        [SerializeField] 
        public List<LevelSceneSO> m_levelsInZone = new List<LevelSceneSO>();

        
        
        public IEnumerable<LevelSceneSO> GetLevelsInZone() => m_levelsInZone;
        
    }
}