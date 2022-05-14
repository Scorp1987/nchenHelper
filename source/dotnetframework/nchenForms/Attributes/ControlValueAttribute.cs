using System.Data;

namespace System.Windows.Forms.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ControlValueAttribute : Attribute
    {
        public ControlValueAttribute(DataColumn column) : this(column.ColumnName) { }

        public ControlValueAttribute(string columnName) => this.ColumnName = columnName;

        public string ColumnName { get; set; }
    }
}
