using Microsoft.VisualBasic.FileIO;
using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class ReadDelimitedFileTask : AGetDataTask
    {
        public override TaskType Type => TaskType.ReadDelimitedFile;
        public string Delimiter { get; set; }
        public string FilePath { get; set; }
        public bool Header { get; set; } = true;

        protected override string FunctionString => $"ReadDelimitedFile('{Delimiter}', '{FilePath}', {Header})";

        protected override Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            using var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var parser = new TextFieldParser(stream) { Delimiters = new string[] { Delimiter } };
            
            if (parser.EndOfData) return null;

            int columnCount = 0;
            DataTable toReturn = new DataTable();
            
            // First row of delimited file as Column Headers
            if(Header)
            {
                var headers = parser.ReadFields();
                columnCount = headers.Length;
                foreach (var header in headers)
                    toReturn.Columns.Add(header);
            }

            // Fill the subsequent rows to datatable
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                // Add new Column if necessary
                for (var i = fields.Length; i > columnCount; i--)
                    toReturn.Columns.Add();
                // Add new row
                var row = toReturn.NewRow();
                for (var i = 0; i < fields.Length; i++)
                    row[i] = fields[i];
                toReturn.Rows.Add(row);
            }
            return Task.FromResult<object>(toReturn);
        }
    }
}
