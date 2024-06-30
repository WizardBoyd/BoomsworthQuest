using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Zone", menuName = "BoomsWorth/Level/New Zone", order = 0)]
    public class ZoneSo : ScriptableObject
    {
        [SerializeField] 
        private List<LevelSO> m_levelsInZone = new List<LevelSO>();
        
        public IEnumerable<LevelSO> GetLevelsInZone() => m_levelsInZone;
        
    }
}