using nchen.Messaging.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nchen.Messaging.Templates
{
    public static class TemplateCollectionExtensions
    {
        public static ITemplate GetTemplate(this IEnumerable<ITemplate> templates, ChannelType type)
        {
            var query = templates.Where(t => t.Type == type);
            var count = query.Count();
            if (count > 1) throw new InvalidOperationException($"There are {count} {type} template found. Only one is allowed");
            if (count < 1) throw new InvalidOperationException($"Couldn't find {type} template.");
            return query.First();
        }

        public static bool TryGetTemplate(this IEnumerable<ITemplate> templates, ChannelType type, out ITemplate template)
        {
            try { template = templates.GetTemplate(type); return true; }
            catch { template = null; return false; }
        }
    }
}
