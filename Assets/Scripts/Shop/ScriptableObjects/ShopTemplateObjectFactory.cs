using UnityEngine;
using WizardOptimizations.Runtime.Factory;

namespace Shop.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Shop Template Object Factory", menuName = "Factory/Shop Template Object Factory", order = 0)]
    public class ShopTemplateObjectFactory : FactorySO<ShopTemplateObject>
    {
        [SerializeField]
        private ShopTemplateObject Prefab;
        
        public override ShopTemplateObject Create()
        {
            return Instantiate(Prefab);
        }
    }
}