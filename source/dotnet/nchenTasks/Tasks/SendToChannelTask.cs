using nchen.Channels;
using nchen.Enums;
using nchen.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SendToChannelTask : ATask
    {
        private string _channelFilePath;


        public override TaskType Type => TaskType.SendToChannel;
        public IChannel Channel { get; set; }
        public ITemplate Template { get; set; }
        public string ChannelFilePath
        {
            get => _channelFilePath;
            set
            {
                _channelFilePath = value;
                if (value == null) return;

                var json = File.ReadAllText(value);
                Channel = JsonConvert.DeserializeObject<IChannel>(json);
            }
        }
        private static StreamWriter Logger => TaskHelper.DataLogger;


        public override async Task<string> ExecuteAsync(Dictionary<string, object> data)
        {
            data["channelType"] = Channel.Type.ToString();
            try
            {
                var result = await CheckTimeout(() => Channel.SendAsync(Template, data));
                WriteData(Channel, true, result);
                return null;
            }
            catch (Exception ex)
            {
                WriteData(Channel, false, ex);
                throw ex;
            }
        }
        public override string ToString() => $"SendTo{Channel.Type}('{Channel.Name}')";
        private static void WriteData(IChannel channel, bool result, object obj)
        {
            try
            {
                if(Logger != null && Logger.BaseStream.Length < 1)
                    Logger?.WriteLine("Type,Name,Result,Object");
                if (!(obj is string str))
                    str = JsonConvert.SerializeObject(obj);
                Logger?.WriteFields(",", new string[]
                {
                    channel.Type.ToString(),
                    channel.Name,
                    result ? "Success" : "Fail",
                    str
                });
            }
            finally { }
        }
    }
}
