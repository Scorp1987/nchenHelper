using AdaptiveCards.Templating;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveCards
{
    public static class AdaptiveCardHelper
    {
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
    }
}
