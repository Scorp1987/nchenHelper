using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public class APropertyChangingChanged : APropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(string propertyName, T previousValue, T currentValue)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs<T>(propertyName, previousValue, currentValue));

        protected bool SetPropertyValue<T>(string propertyName, ref T originalValue, T newValue)
        {
            if (originalValue?.Equals(newValue) == true) return false;
            if (!OnPropertyChanging(propertyName, originalValue, newValue)) return false;
            var previousValue = originalValue;
            originalValue = newValue;
            OnPropertyChanged(propertyName, previousValue, newValue);
            return true;
        }
        protected bool SetPropertyValue<T>(string propertyName, T originalValue, T newValue, Action<T> setMethod)
        {
            if (originalValue.Equals(newValue)) return false;
            if (!OnPropertyChanging(propertyName, originalValue, newValue)) return false;
            setMethod(newValue);
            OnPropertyChanged(propertyName, originalValue, newValue);
            return true;
        }
    }
}
