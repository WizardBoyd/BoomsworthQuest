using UnityEngine;
using WizardOptimizations.Runtime.Factory;
using WizardOptimizations.Runtime.Pool;

namespace Shop.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Shop Template Pool", menuName = "Pool/Shop Template")]
    public class ShopTemplateObjectPool : ComponentPoolSO<ShopTemplateObject>
    {
        [SerializeField]
        private ShopTemplateObjectFactory m_factory;
        
        public override IFactory<ShopTemplateObject> Factory
        {
            get => m_factory;
        }

        public override void Return(ShopTemplateObject member)
        {
            member.BuyButton.onClick.RemoveAllListeners();
            base.Return(member);
        }
    }
}