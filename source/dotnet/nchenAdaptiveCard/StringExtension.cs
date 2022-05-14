using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtension
    {
        public static string WrapTeamsMessageJson(this string adaptiveCardPayloadTemplate) =>
            "{\"type\":\"message\",\"attachments\":[{\"contentType\":\"application/vnd.microsoft.card.adaptive\",\"content\":" + adaptiveCardPayloadTemplate + "}]}";

        public static string WrapIbotMessageJson(this string adaptiveCardPayloadTemplate, string conversationId) =>
            "{\"conversation\":{\"id\":\"" + conversationId + "\"},\"type\":\"card\",\"card\":" + adaptiveCardPayloadTemplate + "}";

        public static string FixAdditionalCommaBug(this string payload) =>
            Regex.Replace(payload, @"},(\s*)]", "}$1]");
    }
}
