using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace nchen.NLP.Entities
{
    public class ListEntity : AEntity, IEntity
    {
        public EntityType Type => EntityType.ListEntity;
        public ListEntityItem[] Items { get; set; }


        internal string GetPatternString()
        {
            var listStr = "";
            foreach (var item in Items)
            {
                listStr += $"|{item.Value}";
                if (item.Synonyms == null) continue;
                foreach (var synonym in item.Synonyms)
                    if (string.IsNullOrEmpty(synonym)) continue;
                    else listStr += $"|{synonym}";
            }
            return listStr.Length > 0 ? listStr[1..] : listStr;
        }
        internal ListEntityItem GetItem(string value) =>
            Items.Where(i => i.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase) ||
            i.Synonyms.Where(s => s.Equals(value, StringComparison.InvariantCultureIgnoreCase)).Count() > 0).First();
        internal ListEntityItem[] GetItems(string value)
        {
            var pattern = GetPatternString();
            var matches = Regex.Matches(value, pattern, RegexOptions.IgnoreCase);
            var toReturn = new List<ListEntityItem>();
            foreach (Match match in matches)
            {
                var item = GetItem(match.Value);
                toReturn.Add(item);
            }
            return (toReturn.Count < 1) ? null : toReturn.ToArray();
        }
        public object GetValue(IDictionary<string, IEntity> entities, string value) => GetItems(value);

    }
}
