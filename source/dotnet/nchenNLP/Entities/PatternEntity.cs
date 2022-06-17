using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace nchen.NLP.Entities
{
    public class PatternEntity : AEntity, IEntity
    {
        public EntityType Type => EntityType.PatternEntity;
        public string[] Patterns { get; set; }


        private IEntity GetEntity(IDictionary<string, IEntity> entities, string entityName)
        {
            if (!entities.TryGetValue(entityName, out var entity)) throw new InvalidOperationException($"'{entityName}' is not defined");
            return entity;
        }
        private string GetPatternString(IEntity entity)
        {
            if (entity is ListEntity listEntity) return listEntity.GetPatternString();
            else if (entity is RegexEntity regexEntity) return regexEntity.GetPatternString();
            else throw new NotImplementedException($"'{entity.Type}' is not implemented in '{nameof(PatternEntity)}.{nameof(GetPatternString)}'.");
        }
        internal string GetPatternString(string pattern, IDictionary<string, IEntity> entities)
        {
            var matches = Regex.Matches(pattern, @"{(\w*)}");
            var toReturn = pattern;
            foreach (Match match in matches)
            {
                var entityName = match.Groups[1].Value;
                var entity = GetEntity(entities, entityName);

                var toReplace = $"(?<{match.Groups[1].Value}>{GetPatternString(entity)})";
                toReturn = toReturn.Replace(match.Value, toReplace);
            }
            return toReturn;
        }
        public Dictionary<string, object>[] GetItems(IDictionary<string, IEntity> entities, string value)
        {
            var toReturn = new List<Dictionary<string, object>>();
            foreach (var pattern in Patterns)
            {
                var regexStr = GetPatternString(pattern, entities);
                var matches = Regex.Matches(value, regexStr, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var items = new Dictionary<string, object>();
                    for (var i = 1; i < match.Groups.Count; i++)
                    {
                        var entityName = match.Groups[i].Name;
                        var entityValue = match.Groups[i].Value;
                        var entity = GetEntity(entities, entityName);
                        var item = entity.GetValue(entities, entityValue);
                        items.Add(entityName, item);
                    }
                    toReturn.Add(items);
                }
            }
            return (toReturn.Count < 1) ? null : toReturn.ToArray();
        }
        public object GetValue(IDictionary<string, IEntity> entities, string value) => GetItems(entities, value);
    }
}
