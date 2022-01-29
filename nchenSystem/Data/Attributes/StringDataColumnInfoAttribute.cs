using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Attributes
{
    public class StringDataColumnInfoAttribute : DataColumnInfoAttribute
    {
        public int MaxLength { get; set; } = -1;

        public override string GetDbDataType()
        {
            var maxSizeStr = MaxLength != -1 ? $"{MaxLength}" : "MAX";
            return $"{DbDataType}({maxSizeStr})";
        }
    }
}
