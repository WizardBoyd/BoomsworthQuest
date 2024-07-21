using System;
using System.Collections.Generic;
using Shop;
using Shop.ScriptableObjects;
using Shop.SerializableData;
using WizardUI.Window;

namespace UI.Properties
{
    [Serializable]
    public class GameShopWindowProperties : WindowProperties
    {
        public CurrencyData CurrencyData { get; set; }
        public Dictionary<ShopCategory, List<ShopItem>> ShopItems { get; set; }
        
        public GameShopWindowProperties(CurrencyData currencyData, Dictionary<ShopCategory, List<ShopItem>> shopItems)
        {
            CurrencyData = currencyData;
            ShopItems = shopItems;
        }
  
    }
}