using nchen.NLP.Intents;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.NLP.JsonConverters
{
    public class IIntentJsonConverter : TypedAbstractJsonConverter<IIntent, IntentType>
    {
        protected override IIntent GetObject(IntentType type) => type switch
        {
            IntentType.SlashCommandIntent => new SlashCommandIntent(),
            _ => throw new NotImplementedException($"'{type}' is not implemented to convert to Json")
        };
    }
}
