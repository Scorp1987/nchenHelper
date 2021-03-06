using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SqlQueryTask : AGetDataTask, ITask
    {
        public TaskType Type => TaskType.SqlQuery;
        public string ConnectionString { get; set; }
        public string ConnectionStringFilePath { get; set; }
        public string SqlString { get; set; }
        public string SqlFilePath { get; set; }
        public Types.SqlParameter[] Parameters { get; set; }


        public override string GetSummaryResult(object obj)
        {
            if (obj is DataTable dt)
                return $"RowCount:{dt.Rows.Count}";
            else
                return null;
        }
        protected override Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            string connectionString = GetConnectionString();
            var sqlStatement = string.IsNullOrEmpty(SqlFilePath) ? SqlString : File.ReadAllText(SqlFilePath);

            var toReturn = new DataTable();
            using (var conn = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sqlStatement, conn) { CommandTimeout = TimeoutMsec / 1000 })
            {
                if (Parameters != null)
                    foreach (var parameter in Parameters)
                    {
                        var obj = data[parameter.Name];
                        var sqlPara = new SqlParameter(parameter.ParameterName, obj);
                        if (parameter.DataType.HasValue) sqlPara.SqlDbType = parameter.DataType.Value;
                        if (parameter.Size.HasValue) sqlPara.Size = parameter.Size.Value;
                        command.Parameters.Add(sqlPara);
                    }
                using var adapter = new SqlDataAdapter(command);
                adapter.Fill(toReturn);
            }

            return Task.FromResult<object>(toReturn);
        }
        protected override string GetDataFunctionString(Dictionary<string, object> data)
        {
            var parameterStr = string.IsNullOrEmpty(ConnectionString) ? "" : $", ConnectionString:'{ConnectionString}'";
            parameterStr += string.IsNullOrEmpty(ConnectionStringFilePath) ? "" : $", ConnectionStringFilePath:'{ConnectionStringFilePath}'";
            parameterStr += string.IsNullOrEmpty(SqlString) ? "" : $", SqlString:'{SqlString}'";
            parameterStr += string.IsNullOrEmpty(SqlFilePath) ? "" : $", SqlFilePath:'{SqlFilePath}'";

            return $"SqlQuery({parameterStr[2..]})";
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
