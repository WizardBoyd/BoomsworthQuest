using System;
using System.Collections;
using System.Collections.Generic;
using DependencyInjection;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using SaveSystem;
using Shop.ScriptableObjects;
using Shop.SerializableData;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using WizardAddressables.Runtime.AssetManagement;
using WizardOptimizations.Runtime.Pool;
using WizardOptimizations.Runtime.Singelton;
using WizardSave;
using WizardSave.ObjectSerializers;

namespace Shop
{
    public class ShopManager : MonoBehaviorSingleton<ShopManager>,IDependencyProvider, ISavableData
    {
        
        public ISaveableKeyValueStore SaveContainer { get; set; }
        public string FilePath
        {
            get => "CurrencyData";
        }
        
        private CurrencyData m_currencyData;
        
        public Dictionary<ShopCategory, List<ShopItem>> shopItems { get; private set; } = new Dictionary<ShopCategory, List<ShopItem>>();
        
        private AsyncOperationHandle m_ShopItemsHandle;
        
        [Header("Broadcasting on")]
        [SerializeField]
        private VoidEventChannelSO m_insufficientFundsEvent;
        [SerializeField]
        private IntEventChannelSO m_AddExternalHealthEvent;
        [SerializeField]
        private IntEventChannelSO m_PremiumResourceChangeEvent;


        private IEnumerator Start()
        {
            m_ShopItemsHandle = AssetManager.Instance.LoadAssetsByLabelAsync<ShopItem>("Shop", OnShopItemLoaded);
            yield return m_ShopItemsHandle;
        }

        private void OnShopItemLoaded(object key, AsyncOperationHandle<ShopItem> handle)
        {
            if(shopItems.ContainsKey(handle.Result.Category))
            {
                shopItems[handle.Result.Category].Add(handle.Result);
            }
            else
            {
                shopItems.Add(handle.Result.Category, new List<ShopItem>(){handle.Result});
            }
        }
        
        public List<ShopItem> GetShopItems(ShopCategory category)
        {
            if(shopItems.ContainsKey(category))
            {
                return shopItems[category];
            }
            return null;
        }
        
        [Provide]
        private CurrencyData GetCurrencyData()
        {
            return m_currencyData;
        }

        public void BuyItem(ShopItem shopItem)
        {
           if(shopItem.Category == ShopCategory.PremiumCurrency)
           {
               m_currencyData.InGameCurrency += shopItem.Quantity;
               m_PremiumResourceChangeEvent.RaiseEvent(m_currencyData.InGameCurrency);
               SaveContainer.Save();
           }
           else if(shopItem.Category == ShopCategory.Heart)
           {
               if(m_currencyData.InGameCurrency >= shopItem.Price)
               {
                   m_currencyData.InGameCurrency -= (int)shopItem.Price;
                   //Add the item to the player
                   SaveContainer.Save();
                   m_PremiumResourceChangeEvent.RaiseEvent(m_currencyData.InGameCurrency);
                   m_AddExternalHealthEvent.RaiseEvent(shopItem.Quantity);
               }
               else
               {
                   //Show a message that the player does not have enough currency
                   m_insufficientFundsEvent.RaiseEvent();
               }
           }
        }

        #region Save/Load
        public void NewSave(ObjectSerializerMap objectSerializerMap)
        {
            m_currencyData = new CurrencyData();
            SaveContainer.SetObject(objectSerializerMap,"CurrencyData", m_currencyData);
            SaveContainer.Save();
        }

        public void Save(ObjectSerializerMap objectSerializerMap)
        {
            SaveContainer.SetObject(objectSerializerMap,"CurrencyData", m_currencyData);
            SaveContainer.Save();
        }

        public void Load(ObjectSerializerMap objectSerializerMap)
        {
            SaveContainer.Load();
            if(SaveContainer.TryGetObject<CurrencyData>(objectSerializerMap,"CurrencyData", out var currencyData))
            {
                m_currencyData =  currencyData;
            }
            else
            {
                m_currencyData = new CurrencyData();
            }
        }

        public void DeleteData()
        {
            SaveContainer.DeleteKey("CurrencyData");
            SaveContainer.Save();
        }
        #endregion
    }
}