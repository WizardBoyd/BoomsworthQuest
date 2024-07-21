using System.Collections;
using System.Collections.Generic;
using Events.ScriptableObjects;
using Shop;
using Shop.ScriptableObjects;
using TMPro;
using UI.Properties;
using UnityEngine;
using UnityEngine.UI;
using WizardOptimizations.Runtime.Pool;
using WizardUI.Window;

namespace UI.WindowController
{
    public class GameShopWindowController : AWindowController<GameShopWindowProperties>
    {
        [SerializeField]
        private ComponentPoolSO<ShopTemplateObject> m_shopTemplateObjectPool;
        
        [SerializeField]
        private IntEventChannelSO m_premiumResourceEventChannel;
        [SerializeField]
        private TMP_Text m_premiumResourceText;
        
        [SerializeField]
        private VoidEventChannelSO m_insufficientFundsEvent;
        [SerializeField]
        private TMP_Text m_insufficientFundsText;
        
        private bool alreadyDisplayedinsufficientFundsText = false;
        
        [Header("Tab Buttons")]
        [SerializeField]
        private Button CashTabButton;
        [SerializeField]
        private Button HeartTabButton;
        [SerializeField]
        private Sprite ActiveTabSprite;
        [SerializeField]
        private Sprite InactiveTabSprite;
        
        [Header("Root Tab Objects")]
        [SerializeField]
        private RectTransform CashTabRoot;
        [SerializeField]
        private RectTransform HeartTabRoot;
        
        private List<ShopTemplateObject> m_shopTemplateObjects = new List<ShopTemplateObject>();
        
        protected override void AddListeners()
        {
            CashTabButton.onClick.AddListener(OnCashTabClicked);
            HeartTabButton.onClick.AddListener(OnHeartTabClicked);
            m_insufficientFundsEvent.OnEventRaised += OnInsufficientFunds;
            m_premiumResourceEventChannel.OnEventRaised += OnPremiumResourceEventChanged;
        }

        private void OnInsufficientFunds()
        {
            if(m_insufficientFundsText != null && !alreadyDisplayedinsufficientFundsText)
            {
                StartCoroutine(DisplayInsufficientFundsText());
                alreadyDisplayedinsufficientFundsText = true;
            }
        }
        
        private IEnumerator DisplayInsufficientFundsText()
        {
            m_insufficientFundsText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            m_insufficientFundsText.gameObject.SetActive(false);
            alreadyDisplayedinsufficientFundsText = false;
        }


        protected override void RemoveListeners()
        {
            CashTabButton.onClick.RemoveListener(OnCashTabClicked);
            HeartTabButton.onClick.RemoveListener(OnHeartTabClicked);
            m_insufficientFundsEvent.OnEventRaised -= OnInsufficientFunds;
            m_premiumResourceEventChannel.OnEventRaised -= OnPremiumResourceEventChanged;
        }

        private void OnPremiumResourceEventChanged(int amount)
        {
            m_premiumResourceText.text = amount.ToString();
        }


        protected override void OnPropertiesSet()
        {
            m_premiumResourceText.text = Properties.CurrencyData.InGameCurrency.ToString();
            SwitchTabs(ShopCategory.PremiumCurrency, HeartTabRoot.gameObject, CashTabRoot.gameObject);
        }

        protected override void WhileHiding()
        {
            foreach (ShopTemplateObject templateObject in m_shopTemplateObjects)
            {
                m_shopTemplateObjectPool.Return(templateObject);
            }
            m_insufficientFundsText.gameObject.SetActive(false);
        }
        
        private void OnCashTabClicked()
        {
            //Hide the other tab
            SwitchTabs(ShopCategory.PremiumCurrency, HeartTabRoot.gameObject, CashTabRoot.gameObject);
            //Create the objects but for the Cash Tab
        }
        
        private void OnHeartTabClicked()
        {
            //Hide the other tab
            SwitchTabs(ShopCategory.Heart, CashTabRoot.gameObject, HeartTabRoot.gameObject);
            //Create the objects but for the Heart Tab
        }

        private void SwitchTabs(ShopCategory category, GameObject lastTab, GameObject nextTab)
        {
            
            if(category == ShopCategory.PremiumCurrency)
            {
                CashTabButton.image.sprite = ActiveTabSprite;
                HeartTabButton.image.sprite = InactiveTabSprite;
            }
            else
            {
                CashTabButton.image.sprite = InactiveTabSprite;
                HeartTabButton.image.sprite = ActiveTabSprite;
            }
            
            
            lastTab.SetActive(false);
            //Recycle the objects
            foreach (var shopTemplateObject in m_shopTemplateObjects)
            {
                m_shopTemplateObjectPool.Return(shopTemplateObject);
            }
            m_shopTemplateObjects.Clear();
            nextTab.SetActive(true);
            foreach (ShopItem item in Properties.ShopItems[category])
            {
                ShopTemplateObject shopTemplateObject = m_shopTemplateObjectPool.Request();
                shopTemplateObject.SetShopItem(item);
                shopTemplateObject.transform.SetParent(nextTab.transform);
                m_shopTemplateObjects.Add(shopTemplateObject);
            }
        }
    }
}