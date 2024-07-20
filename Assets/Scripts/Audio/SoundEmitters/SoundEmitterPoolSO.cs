using UnityEngine;
using WizardOptimizations.Runtime.Factory;
using WizardOptimizations.Runtime.Pool;

namespace Audio.SoundEmitters
{
    [CreateAssetMenu(fileName = "NewSoundEmitterPool", menuName = "Pool/SoundEmitter Pool")]
    public class SoundEmitterPoolSO : ComponentPoolSO<SoundEmitter>
    {
        [SerializeField] private SoundEmitterFactorySO _factory;

        public override IFactory<SoundEmitter> Factory
        {
            get
            {
                return _factory;
            }
        }
    }
}