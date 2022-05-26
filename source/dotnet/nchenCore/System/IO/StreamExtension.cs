using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    public static class StreamExtension
    {
        /// <summary>
        /// Get TextFieldParser with delimiter ','
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>TextFieldParser with delimiter ','</returns>
        public static TextFieldParser GetCsvTextFieldParser(this Stream stream) => new TextFieldParser(stream) { Delimiters = new string[] { "," } };
    }
}
