using AdaptiveCards.Templating;
using nchen.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace nchen
{
    public static class TaskHelper
    {
        public static ConsoleLogger Logger { get; set; }
        public static StreamWriter DataLogger { get; set; }

        public static string ExpandAdaptiveTemplate(string json, object data)
        {
            var template = new AdaptiveCardTemplate(json);
            var payload = template.Expand(data);
            payload = payload.FixAdditionalCommaBug();
            return payload;
        }
        public static T ExpandAdaptiveTemplate<T>(string json, object data)
        {
            var payload = ExpandAdaptiveTemplate(json, data);
            return JsonConvert.DeserializeObject<T>(payload);
        }


        public static async Task RunAsync(this ITask task, Dictionary<string, object> data)
        {
            if (task is ATasks)
                await task.ExecuteAsync(data);
            else
                await Logger?.RunTaskAsync(task.ToString(), () => task.ExecuteAsync(data), result => result);
        }
    }
}
