using System;
using System.Attributes;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace System.IO
{
    public static class IOHelper
    {
        public static List<TObject> ReadCsvFile<TObject>(string filePath)
            where TObject : new()
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var parser = stream.GetCsvTextFieldParser();
            return parser.GetList<TObject>();
        }
        public static List<TObject> ReadCsvFile<TObject, TAttribute>(string filePath)
            where TObject : new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var parser = stream.GetCsvTextFieldParser();
            return parser.GetList<TObject,TAttribute>();
        }
    }
}
