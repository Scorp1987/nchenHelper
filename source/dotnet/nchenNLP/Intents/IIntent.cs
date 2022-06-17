using nchen.NLP.Entities;
using nchen.NLP.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.NLP.Intents
{
    [JsonConverter(typeof(IIntentJsonConverter))]
    public interface IIntent
    {
        public IntentType Type { get; }

        public bool CheckUtterance(string utterance, IDictionary<string, IEntity> entities, out Dictionary<string, object> entityValues);
    }
}
