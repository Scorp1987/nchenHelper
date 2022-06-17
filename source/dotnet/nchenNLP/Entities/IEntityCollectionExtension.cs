using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nchen.NLP.Entities
{
    public static class IEntityCollectionExtension
    {
        public static Dictionary<string, IEntity> GetEntities(this IDictionary<string, IEntity> entities, IEnumerable<string> entityNames)
        {
            var query = from entity in entities
                        where entityNames.Contains(entity.Key, StringComparer.InvariantCultureIgnoreCase)
                        select entity;
            return query.ToDictionary(e => e.Key, e => e.Value);
        }

        public static Dictionary<string, object> GetEntityValues(this IDictionary<string, IEntity> entities, string utterance, Dictionary<string, EntityAttributes> attributes)
        {
            var entityNames = attributes.Keys;
            var interestedEntities = entities.GetEntities(entityNames);

            var toReturn = new Dictionary<string, object>();
            foreach (var entity in interestedEntities)
            {
                var attribute = attributes[entity.Key];
                if (!attribute.IsOutput) continue;
                var entityName = entity.Key;
                var entityValue = entity.Value.GetValue(entities, utterance);
                toReturn.Add(entityName, entityValue);
            }
            return toReturn;
        }
    }
}
