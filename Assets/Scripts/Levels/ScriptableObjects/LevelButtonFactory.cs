using UnityEngine;
using WizardOptimizations.Runtime.Factory;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level Button Factory", menuName = "Factory/Level Button Factor", order = 0)]
    public class LevelButtonFactory : FactorySO<LevelButton>
    {
        [SerializeField] 
        private LevelButton Prefab;
        
        public override LevelButton Create()
        {
            return Instantiate(Prefab);
        }
    }
}