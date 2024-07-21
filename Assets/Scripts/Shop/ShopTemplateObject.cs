using Shop.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopTemplateObject : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_quantityText;
        
        [SerializeField]
        private Image m_ItemImage;
        
        [SerializeField]
        private Button m_BuyButton;
        public Button BuyButton => m_BuyButton;
        
        [SerializeField]
        private TMP_Text m_priceText;
        
        public ShopItem ShopItem { get; private set; }

        public void SetShopItem(ShopItem shopItem)
        {
            ShopItem = shopItem;
            m_ItemImage.sprite = shopItem.Icon;
            m_quantityText.text = shopItem.Quantity.ToString();
            m_priceText.text = shopItem.Price.ToString();
            m_BuyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        private void OnBuyButtonClicked()
        {
            if(ShopManager.Instance != null)
            {
                ShopManager.Instance.BuyItem(ShopItem);
            }
        }
    }
}