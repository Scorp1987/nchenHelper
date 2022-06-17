using System.Data;

namespace nchen.Tasks.Types
{
    public class SqlParameter
    {
        public string ParameterName { get; set; }

        public SqlDbType? DataType { get; set; }

        public int? Size { get; set; }

        public string Name { get; set; }
    }
}
