using nchen.NLP.Entities;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.NLP.JsonConverters
{
    public class IEntityJsonConverter : TypedAbstractJsonConverter<IEntity, EntityType>
    {
        protected override string TypePropertyName => nameof(IEntity.Type);

        protected override IEntity GetObject(EntityType type) => type switch
        {
            EntityType.ListEntity => new ListEntity(),
            EntityType.RegexEntity => new RegexEntity(),
            EntityType.PatternEntity => new PatternEntity(),
            _ => throw new NotImplementedException($"'{type}' is not implemented to convert to Json")
        };
    }
}
