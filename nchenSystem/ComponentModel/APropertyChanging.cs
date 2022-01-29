using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public class APropertyChanging : INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;

        protected bool OnPropertyChanging<T>(string propertyName, T originalValue, T newValue)
        {
            var handler = this.PropertyChanging;
            if (handler == null) return true;
            var args = new PropertyChangingCancelEventArgs<T>(propertyName, originalValue, newValue);
            handler(this, args);
            return !args.Cancel;
        }
    }
}
