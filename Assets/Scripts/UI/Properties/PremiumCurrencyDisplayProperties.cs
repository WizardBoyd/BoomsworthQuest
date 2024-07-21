using System;
using Shop.SerializableData;
using WizardUI;

namespace UI.Properties
{
    [Serializable]
    public class PremiumCurrencyDisplayProperties : IScreenProperties
    {
        public ScreenPriority Priority { get; set; } = ScreenPriority.ForceForeground;
        public bool HideOnForegroundLost { get; set; } = false;
        public bool IsPopup { get; set; } = false;
        public bool SuppressPrefabProperties { get; set; } = false;
        
        public CurrencyData CurrencyData { get; set; }
        
        public PremiumCurrencyDisplayProperties(CurrencyData currencyData)
        {
            CurrencyData = currencyData;
        }
    }
}