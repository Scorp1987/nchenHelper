using System.Reflection;

namespace System.IO
{
    public static class StreamReaderExtension
    {
        /// <summary>
        /// Get last cursor position of base stream without buffer.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>return last cursor position of base stream without buffer</returns>
        public static long GetPosition(this StreamReader reader)
        {
            int charPos = (int)reader.GetType().InvokeMember("charPos", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, reader, null);
            int charLen = (int)reader.GetType().InvokeMember("charLen", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, reader, null);

            return (reader.BaseStream.Position - charLen + charPos);
        }
    }
}
