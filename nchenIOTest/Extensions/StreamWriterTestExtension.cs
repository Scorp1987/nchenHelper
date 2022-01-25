namespace System.IO.Extensions
{
    static class StreamWriterTestExtension
    {
        public static string ReadStreamAndDispose(this StreamWriter writer)
        {
            writer.Flush();
            var stream = writer.BaseStream;
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
