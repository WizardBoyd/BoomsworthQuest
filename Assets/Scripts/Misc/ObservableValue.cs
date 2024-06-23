using System;

namespace Misc
{
    public class ObservableValue<T>
    {
        private T m_Value;

        public event EventHandler<ValueChangedEventArgs<T>> OnValueChanged;

        public T Value
        {
            get => m_Value;
            set
            {
                if (!Equals(m_Value, value))
                {
                    T oldValue = m_Value;
                    m_Value = value;
                    onValueChanged(oldValue, m_Value);
                }
            }
        }

        protected virtual void onValueChanged(T oldValue, T newValue)
        {
            OnValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(oldValue, newValue));
        }

        public ObservableValue(T initalValue)
        {
            m_Value = initalValue;
        }
        
        
    }
    
}