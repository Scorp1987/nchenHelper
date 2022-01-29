using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Exceptions
{
    class EmptyDataTypeException : Exception
    {
        public EmptyDataTypeException(string propertyName) : base($"DataType is null or empty for {propertyName} property.") { }
    }
}
