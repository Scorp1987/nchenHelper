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
