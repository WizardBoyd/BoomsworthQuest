using Events.ScriptableObjects;
using TMPro;
using UI.Properties;
using UnityEngine;
using WizardUI.Panel;

namespace UI.PanelControllers
{
    public class PremiumResourcePanelController : APanelController<PremiumCurrencyDisplayProperties>
    {
        [SerializeField]
        private IntEventChannelSO m_premiumResourceEventChannel;
        
        [SerializeField]
        private TMP_Text m_premiumResourceText;
        
        protected override void AddListeners()
        {
            m_premiumResourceEventChannel.OnEventRaised += OnPremiumResourceEventRaised;
        }

        private void OnPremiumResourceEventRaised(int amount)
        {
            m_premiumResourceText.text = amount.ToString();
        }

        protected override void RemoveListeners()
        {
            m_premiumResourceEventChannel.OnEventRaised -= OnPremiumResourceEventRaised;
        }

        protected override void OnPropertiesSet()
        {
            m_premiumResourceText.text = Properties.CurrencyData.InGameCurrency.ToString();
        }

        protected override void WhileHiding()
        {
           
        }

        protected override void HierarchyFixOnShow()
        {
           
        }
    }
}