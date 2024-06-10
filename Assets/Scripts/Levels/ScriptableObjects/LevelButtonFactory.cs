using Factory;
using UnityEngine;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level Button Factory", menuName = "Factory/Level Button Factor", order = 0)]
    public class LevelButtonFactory : FactorySO<LevelButtonView>
    {
        [SerializeField] 
        private LevelButtonView Prefab;
        
        public override LevelButtonView Create()
        {
            return Instantiate(Prefab);
        }
    }
}