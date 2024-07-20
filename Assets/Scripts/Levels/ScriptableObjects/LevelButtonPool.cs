using UnityEngine;
using WizardOptimizations.Runtime.Factory;
using WizardOptimizations.Runtime.Pool;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level Button Pool", menuName = "Pool/Level Button Pool", order = 0)]
    public class LevelButtonPool : ComponentPoolSO<LevelButton>
    {
        [SerializeField] private LevelButtonFactory _factory;
        public override IFactory<LevelButton> Factory
        {
            get => _factory;
        }
    }
}