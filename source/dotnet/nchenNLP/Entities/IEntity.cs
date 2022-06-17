using nchen.NLP.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.NLP.Entities
{
    [JsonConverter(typeof(IEntityJsonConverter))]
    public interface IEntity
    {
        public EntityType Type { get; }
        public bool IsOutput { get; set; }
        public bool Required { get; set; }

        public object GetValue(IDictionary<string, IEntity> entities, string value);
    }
}
