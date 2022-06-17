using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace nchen.NLP.Entities
{
    public class RegexEntity : AEntity, IEntity
    {
        public EntityType Type => EntityType.RegexEntity;
        public string Regex { get; set; }


        internal string GetPatternString() => Regex;
        internal string[] GetItems(string value)
        {
            var pattern = GetPatternString();
            var matches = System.Text.RegularExpressions.Regex.Matches(value, pattern, RegexOptions.IgnoreCase);
            var query = matches.Select(m => m.Value);
            return (query.Count() < 1) ? null : query.ToArray();
        }
        public object GetValue(IDictionary<string, IEntity> entities, string value) => GetItems(value);
    }
}
