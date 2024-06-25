using System;

namespace UI
{
    public interface IScreenProperties
    {
        ScreenPriority Priority { get; set; }
        bool HideOnForegroundLost { get; set; }
        bool IsPopup { get; set; }
        bool SuppressPrefabProperties { get; set; }
    }

    public class ScreenPropertiesBuilder<T> where T : IScreenProperties, new()
    {
        private ScreenPriority Priority = ScreenPriority.None;
        private bool HideOnForegroundLost = true;
        private bool IsPopup = false;
        private bool SuppressPrefabProperties = false;

        public ScreenPropertiesBuilder<T> WithScreenPriority(ScreenPriority priority)
        {
            this.Priority = priority;
            return this;
        }
        public ScreenPropertiesBuilder<T> SetHideOnForegroundLost(bool hideOnForegroundLost = true)
        {
            this.HideOnForegroundLost = hideOnForegroundLost;
            return this;
        }

        public ScreenPropertiesBuilder<T> SetIsPopUp(bool isPopUp = false)
        {
            this.IsPopup = isPopUp;
            return this;
        }
        
        public ScreenPropertiesBuilder<T> SetPrefabSuppression(ScreenPriority priority)
        {
            this.Priority = priority;
            return this;
        }

        public T Build()
        {
            T screenProperty = new T
            {
                Priority = this.Priority,
                SuppressPrefabProperties = this.SuppressPrefabProperties,
                HideOnForegroundLost = this.HideOnForegroundLost,
                IsPopup = this.IsPopup
            };
            return screenProperty;
        }
    }
    
}