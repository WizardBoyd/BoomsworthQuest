using UnityEngine;

namespace Shop.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Item", order = 0)]
    public class ShopItem : ScriptableObject
    {
        [SerializeField]
        private string itemName;
        public string ItemName => itemName;
        
        [SerializeField]
        private ShopCategory category;
        public ShopCategory Category => category;
        
        [SerializeField]
        private ResourceType itemType;
        public ResourceType ItemType => itemType;
        
        [SerializeField]
        private ResourceType currencyType;
        public ResourceType CurrencyType => currencyType;
        
        [SerializeField]
        private float price;
        public float Price => price;
        
        [SerializeField]
        private Sprite icon;
        public Sprite Icon => icon;
        
        [SerializeField]
        private int quantity;
        public int Quantity => quantity;
        
    }
}