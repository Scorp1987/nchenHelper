using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data;

namespace nchen.Types
{
    public class SqlParameter
    {
        public string ParameterName { get; set; }

        public SqlDbType? DataType { get; set; }

        public int? Size { get; set; }

        public string Name { get; set; }
    }
}
