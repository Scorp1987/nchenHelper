using nchen.Messaging.Channels;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace nchen.Messaging
{
    public static class MessagingHelper
    {
        public static StreamWriter Logger { get; set; }

        private static string _filePathLogger;


        public static void OpenLogger(string filePath)
        {
            _filePathLogger = filePath;
            try
            {
                var stream = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                Logger = new StreamWriter(stream) { AutoFlush = true };
            }
            catch { Logger = null; }
        }
        public static void CloseLogger()
        {
            try
            {
                Logger?.Flush();
                bool deleteDatLog = false;
                if (Logger?.BaseStream.Length == 0) deleteDatLog = true;
                Logger?.Dispose();
                if (deleteDatLog) File.Delete(_filePathLogger);
            }
            catch { }
        }


        private static string GetString(this object obj)
        {
            if (obj == null) return null;
            if (obj is string str) return str;
            if (obj.GetType().IsValueType) return obj.ToString();
            return JsonConvert.SerializeObject(obj);
        }
        public static async Task WriteDataAsync(this StreamWriter writer, IChannel channel, string activityType, bool result, object activityObject = null, object returnObject = null)
        {
            try
            {
                if (writer != null && writer.BaseStream.Length < 1)
                    await writer?.WriteLineAsync("ChannelType,ChannelName,ActivityType,ActivityObject,Result,ReturnObject");
                var activityObjectStr = activityObject.GetString();
                var returnObjectStr = returnObject.GetString();
                await writer?.WriteFieldsAsync(",", new string[]
                {
                    channel.Type.ToString(),
                    channel.Name,
                    activityType,
                    activityObjectStr,
                    result ? "Success" : "Fail",
                    returnObjectStr
                });
            }
            finally { }
        }
    }
}
