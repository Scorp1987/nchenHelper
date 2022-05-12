using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SqlQueryTask : AGetDataTask
    {
        public override TaskType Type => TaskType.SqlQuery;
        public string ConnectionString { get; set; }
        public string ConnectionStringFilePath { get; set; }
        public string SqlFilePath { get; set; }
        protected override string FunctionString
        {
            get
            {
                var parameterStr = string.IsNullOrEmpty(ConnectionString) ? "" : $", ConnectionString:'{ConnectionString}'";
                parameterStr += string.IsNullOrEmpty(ConnectionStringFilePath) ? "" : $", ConnectionStringFilePath:'{ConnectionStringFilePath}'";
                parameterStr += $", SqlFilePath:'{SqlFilePath}'";

                return $"SqlQuery({parameterStr.Substring(2)})";
            }
        }


        protected override Task<object> GetDataAsync()
        {
            string connectionString = GetConnectionString();
            var sqlStatement = File.ReadAllText(SqlFilePath);

            var toReturn = new DataTable();
            using (var conn = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sqlStatement, conn) { CommandTimeout = TimeoutMsec / 1000 })
            using (var adapter = new SqlDataAdapter(command))
                adapter.Fill(toReturn);

            if (toReturn.Rows.Count < 1)
                return Task.FromResult<object>(null);
            else
                return Task.FromResult<object>(toReturn);
        }
        protected override string GetResult(object obj)
        {
            if (obj is DataTable dt)
                return $"RowCount:{dt.Rows.Count}";
            else
                return null;
        }
        private string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
                return ConnectionString;
            else if (!string.IsNullOrEmpty(ConnectionStringFilePath))
                return File.ReadAllText(ConnectionStringFilePath);
            else
                throw new InvalidOperationException($"Either {ConnectionString} or {ConnectionStringFilePath} must be present");
        }
    }
}
