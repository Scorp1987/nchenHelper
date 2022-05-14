namespace System.Attributes
{
    public class StringDataColumnInfoAttribute : DataColumnInfoAttribute
    {
        public int MaxLength { get; set; } = -1;

        public override string GetDbDataType()
        {
            var dbDataType = string.IsNullOrEmpty(DbDataType) ? "VARCHAR" : DbDataType;
            var maxSizeStr = MaxLength == -1 ? "MAX" : $"{MaxLength}";
            return $"{dbDataType}({maxSizeStr})";
        }
    }
}
