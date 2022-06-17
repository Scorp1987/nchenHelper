using nchen.Messaging.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Messaging.Channels
{
    public abstract class AChannel
    {
        public string Name { get; set; }
        public static StreamWriter Logger => MessagingHelper.Logger;


        protected async Task WriteDataAsync(IChannel channel, string activityType, bool result, object activityObject = null, object returnObject = null)
        {
            if (Logger == null) return;
            await Logger.WriteDataAsync(channel, activityType, result, activityObject, returnObject);
        }
        public virtual Task<Dictionary<string, object>> AskInputsAsync(ITemplate template, object data, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
