using AdaptiveCards.Templating;
using AdaptiveCards.Types;
using nchen.Enums;
using nchen.Templates;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace nchen.Channels
{
    public class TelegramChannel : AChannel
    {
        public override ChannelType Type => ChannelType.Telegram;
        public string Token { get; set; }
        public ChatId ChatID { get; set; }


        public override async Task<string> SendAsync(ITemplate template, object data)
        {
            if (!(template is TelegramTemplate telegramTemplate))
                throw new ArgumentException($"{nameof(template)} must be {nameof(TelegramTemplate)}.", nameof(template));

            var json = System.IO.File.ReadAllText(telegramTemplate.TemplateFilePath);
            var card = TaskHelper.ExpandAdaptiveTemplate<AdaptiveCard>(json, data);
            var text = card.RenderTelegramMessage();

            var client = new TelegramBotClient(Token);
            var message = await client.SendTextMessageAsync(ChatID, text, ParseMode.MarkdownV2);
            return JsonConvert.SerializeObject(message);
        }
    }
}
