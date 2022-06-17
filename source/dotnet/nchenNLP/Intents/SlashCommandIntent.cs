using nchen.NLP.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nchen.NLP.Intents
{
    public class SlashCommandIntent : AIntent, IIntent
    {

        public IntentType Type => IntentType.SlashCommandIntent;
        public string[] SlashCommands { get; set; }
        public Dictionary<string, EntityAttributes> EntityAttributes { get; set; }

        public bool CheckUtterance(string utterance, IDictionary<string, IEntity> entities, out Dictionary<string, object> entityValues)
        {
            utterance = utterance.Trim();
            var commandCount = SlashCommands.Count(c =>
            {
                var command = (utterance.Length > c.Length) ? $"{c} " : c;
                return utterance.StartsWith(command, StringComparison.InvariantCultureIgnoreCase);
            });
            if(commandCount < 1) { entityValues = null; return false; }

            if (EntityAttributes == null) return ReturnUtterance(true, out entityValues);
            var entityNames = this.EntityAttributes.Keys;
            var interestedEntities = entities.GetEntities(entityNames);
            entityValues = new Dictionary<string, object>();
            foreach(var entity in interestedEntities)
            {
                var attribute = EntityAttributes[entity.Key];
                if (!attribute.IsOutput) continue;
                var entityName = entity.Key;
                var entityValue = entity.Value.GetValue(entities, utterance);
                if (attribute.Required && entityValue == null) return ReturnUtterance(false, out entityValues);
                if(entityValue != null) entityValues.Add(entityName, entityValue);
            }
            if (entityValues.Count < 1) entityValues = null;
            return true;
        }

        public bool ReturnUtterance(bool result, out Dictionary<string, object> entityValues)
        {
            entityValues = null;
            return result;
        }
    }
}
