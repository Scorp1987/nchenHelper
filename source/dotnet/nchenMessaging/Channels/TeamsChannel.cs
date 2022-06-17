using AdaptiveCards;
using AdaptiveCards.Templating;
using nchen.Messaging.Exceptions;
using nchen.Messaging.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Messaging.Channels
{
    public class TeamsChannel : AChannel, IChannel
    {
        public ChannelType Type => ChannelType.Teams;
        public string BotUrl { get; set; }
        public string ConversationID { get; set; }
        public string IncomingWebhookUrl { get; set; }

        public async Task<object> SendAsync(ITemplate template, object data)
        {
            if (!(string.IsNullOrEmpty(IncomingWebhookUrl) ^ string.IsNullOrEmpty(ConversationID)))
                throw new InvalidOperationException($"Can only have either {nameof(IncomingWebhookUrl)} or {nameof(ConversationID)}.");

            if (!(template is TeamsTemplate teamsTemplate))
                throw new ArgumentException($"{nameof(template)} must be {nameof(TeamsTemplate)}.", nameof(template));

            var json = File.ReadAllText(teamsTemplate.TemplateFilePath);
            var payload = AdaptiveCardHelper.ExpandAdaptiveTemplate(json, data);
            payload = payload.WrapTeamsMessageJson();
            if (!string.IsNullOrEmpty(ConversationID))
                payload = payload.WrapIbotMessageJson(ConversationID);

            var url = string.IsNullOrEmpty(IncomingWebhookUrl) ? BotUrl : IncomingWebhookUrl;

            try
            {
                using var client = new HttpClient();
                using var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var contentStr = await response.Content.ReadAsStringAsync();
                await WriteDataAsync(this, "SendMessage", true, payload, contentStr);
                if (!string.IsNullOrEmpty(ConversationID) && response.StatusCode != HttpStatusCode.Created)
                    throw new Exception($"Couldn't send message to {ConversationID}", new HttpRequestException(contentStr));
                else if (!string.IsNullOrEmpty(IncomingWebhookUrl) && response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Couldn't send message to {IncomingWebhookUrl}", new HttpRequestException(contentStr));
                return contentStr;
            }
            catch(Exception ex)
            {
                await WriteDataAsync(this, "SendMessage", false, payload, ex);
                throw new SendMessageFailException(this, ex);
            }
        }
    }
}
