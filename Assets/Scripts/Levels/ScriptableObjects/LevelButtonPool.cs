using Factory;
using UnityEngine;

namespace Levels.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level Button Pool", menuName = "Pool/Level Button Pool", order = 0)]
    public class LevelButtonPool : ComponentPoolSO<LevelButtonView>
    {
        [SerializeField] private LevelButtonFactory _factory;
        public override IFactory<LevelButtonView> Factory
        {
            get => _factory;
        }
    }
}