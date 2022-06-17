using AdaptiveCards;
using AdaptiveCards.Enums;
using AdaptiveCards.Types;
using nchen.Messaging.Exceptions;
using nchen.Messaging.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace nchen.Messaging.Channels
{
    public class TelegramChannel : AChannel, IChannel
    {
        public const int MAX_TEXT_LENGTH = 4096;

        public ChannelType Type => ChannelType.Telegram;
        public string Token { get; set; }
        public ChatId ChatID { get; set; }
        public bool IsWaitForInput { get; private set; } = false;


        private Update Update { get; set; }
        private bool IsUpdateReceived { get; set; }
        private bool IsCancelWaitForInput { get; set; } = false;


        public void OnUpdateReceived(Update update)
        {
            Update = update;
            IsUpdateReceived = true;
        }
        public void CancelWaitForInput() => IsCancelWaitForInput = true;


        public async Task<object> SendAsync(ITemplate template, object data)
        {
            var card = GetAdaptiveCard(template, data);
            return await SendAsync(card);
        }
        public override async Task<Dictionary<string, object>> AskInputsAsync(ITemplate template, object data, CancellationToken cancellationToken)
        {
            var card = GetAdaptiveCard(template, data);
            await SendAsync(card);

            var telegramTemplate = (TelegramTemplate)template;
            var client = new TelegramBotClient(Token);
            var inputs = card.GetInputs();
            var toReturn = new Dictionary<string, object>();
            foreach (var input in inputs)
            {
                await SendInputMessageAsync(client, input, telegramTemplate.InlineDataPrefix);
                IsWaitForInput = true;
                while (true)
                {
                    if (IsCancelWaitForInput) { IsWaitForInput = false; throw new OperationCanceledException(); }
                    if (cancellationToken.IsCancellationRequested) { IsWaitForInput = false; return null; }

                    if (!IsUpdateReceived)
                    {
                        await Task.Delay(100);
                        continue;
                    }

                    var update = Update;
                    IsUpdateReceived = false;
                    await EditMessageIfNeededAsync(client, update);
                    if (!TryGetInputValue(update, input, out var valueString, out var errMessage))
                    {
                        await SendValidationFailMessageAsync(client, input, errMessage);
                        await SendInputMessageAsync(client, input, telegramTemplate.InlineDataPrefix);
                        continue;
                    }
                    if (!TryGetValue(input, valueString, telegramTemplate.InlineDataPrefix, out var value, out errMessage))
                    {
                        await SendValidationFailMessageAsync(client, input, errMessage);
                        await SendInputMessageAsync(client, input, telegramTemplate.InlineDataPrefix);
                        continue;
                    }
                    toReturn.Add(input.ID, value);
                    IsWaitForInput = false;
                    break;
                }
            }
            return toReturn;
        }


        private bool TryGetInputValue(Update update, IInput input, out string value, out string errorMessage)
        {
            switch (input.Type)
            {
                case ElementType.InputText:
                case ElementType.InputNumber:
                case ElementType.InputDate:
                case ElementType.InputTime:
                    value = update.Message.Text;
                    errorMessage = "Text message is expected.";
                    return (update.Type == UpdateType.Message && !string.IsNullOrEmpty(update.Message.Text));
                case ElementType.InputToggle:
                case ElementType.InputChoiceSet:
                    value = update.CallbackQuery.Data;
                    errorMessage = "Button click is expected.";
                    return (update.Type == UpdateType.CallbackQuery);
                default:
                    throw new NotImplementedException($"'{input.Type}' is not implemented in {nameof(TryGetInputValue)}");
            }
        }
        private bool TryGetValue(IInput input, string valueString, string inlineDataPrefix, out object value, out string errorMessage)
        {
            try
            {
                value = input.GetValue(valueString, inlineDataPrefix);
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                value = null;
                errorMessage = ex.Message;
                return false;
            }
        }
        private AdaptiveCard GetAdaptiveCard(ITemplate template, object data)
        {
            if (!(template is TelegramTemplate telegramTemplate))
                throw new ArgumentException($"{nameof(template)} must be {nameof(TelegramTemplate)}.", nameof(template));

            var json = System.IO.File.ReadAllText(telegramTemplate.TemplateFilePath);
            return AdaptiveCardHelper.ExpandAdaptiveTemplate<AdaptiveCard>(json, data);
        }


        private async Task<Message> EditMessageIfNeededAsync(TelegramBotClient client, Update update)
        {
            if (update.Type != UpdateType.CallbackQuery) return null;

            var data = update.CallbackQuery.Data;
            var buttons = from b in update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard
                          from sb in b
                          select sb;
            foreach (var button in buttons)
            {
                if (button.CallbackData != data) continue;
                var message = update.CallbackQuery.Message;
                return await EditMessageTextAsync(client, message.MessageId, $"{message.Text}\n{button.Text}".FixAllOthers(), null);
            }
            return null;
        }
        private async Task<Message> EditMessageTextAsync(TelegramBotClient client, int messageId, string text, InlineKeyboardMarkup replyMarkup = null)
        {
            try
            {
                var message = await client.EditMessageTextAsync(ChatID, messageId, text, ParseMode.MarkdownV2, replyMarkup: replyMarkup);
                await WriteDataAsync(this, "EditMessage", true, text, message);
                return message;
            }
            catch (Exception ex)
            {
                await WriteDataAsync(this, "EditMessage", false, text, ex);
                throw new SendMessageFailException(this, ex);
            }
        }
        private async Task<Message> SendTextMessageAsync(TelegramBotClient client, string text, IReplyMarkup replyMarkup = null)
        {
            try
            {
                var message = await client.SendTextMessageAsync(ChatID, text, ParseMode.MarkdownV2, replyMarkup: replyMarkup);
                await WriteDataAsync(this, "SendMessage", true, text, message);
                return message;
            }
            catch (Exception ex)
            {
                await WriteDataAsync(this, "SendMessage", false, text, ex);
                throw new SendMessageFailException(this, ex);
            }
        }
        private async Task<Message> SendInputMessageAsync(TelegramBotClient client, IInput input, string inlineDataPrefix)
        {
            switch (input.Type)
            {
                case ElementType.InputText:
                case ElementType.InputNumber:
                case ElementType.InputDate:
                case ElementType.InputTime:
                    return await SendTextMessageAsync(client, input.Title.ToMarkDownV2Text());
                case ElementType.InputToggle:
                    var toggle = (InputToogle)input;
                    var inlineButtons = GetButtons(toggle, inlineDataPrefix);
                    return await SendTextMessageAsync(client, input.Title.ToMarkDownV2Text(), new InlineKeyboardMarkup(inlineButtons));
                case ElementType.InputChoiceSet:
                    var choiceSet = (InputChoiceSet)input;
                    inlineButtons = GetButtons(choiceSet, inlineDataPrefix);
                    return await SendTextMessageAsync(client, choiceSet.Title.ToMarkDownV2Text(), new InlineKeyboardMarkup(inlineButtons));
                default:
                    throw new NotImplementedException($"'{input.Type}' is not implemented yet in {nameof(SendInputMessageAsync)}");
            }
        }


        private List<InlineKeyboardButton[]> GetButtons(InputToogle toggle, string inlineDataPrefix)
        {
            return new List<InlineKeyboardButton[]> { new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(toggle.ValueOn){ CallbackData = $"{inlineDataPrefix}{toggle.ValueOn}" },
                new InlineKeyboardButton(toggle.ValueOff){ CallbackData =  $"{inlineDataPrefix}{toggle.ValueOff}" }
            }};
        }
        private List<InlineKeyboardButton[]> GetButtons(InputChoiceSet choiceSet, string inlineDataPrefix)
        {
            var index = 0;
            var buttons = new List<InlineKeyboardButton>();
            var inlineButtons = new List<InlineKeyboardButton[]>();
            foreach (var choice in choiceSet.Choices)
            {
                buttons.Add(new InlineKeyboardButton(choice.Title) { CallbackData = $"{inlineDataPrefix}{choice.Value}" });
                index++;
                if (index >= choiceSet.MaxWidth)
                {
                    index = 0;
                    inlineButtons.Add(buttons.ToArray());
                    buttons.Clear();
                }
            }
            if (index != 0) inlineButtons.Add(buttons.ToArray());
            return inlineButtons;
        }
        private async Task<Message> SendValidationFailMessageAsync(TelegramBotClient client, IInput input, string errMessage)
        {
            var inputErrMsg = input.ErrorMessage.ToMarkDownV2Text();
            errMessage = string.IsNullOrEmpty(inputErrMsg) ? errMessage.FixAllOthers() : inputErrMsg;
            return await SendTextMessageAsync(client, errMessage);
        }
        private async Task<object> SendAsync(AdaptiveCard card)
        {
            var text = card.ToMarkDownV2Text();
            if (text.Length < 1) return null;

            // If the text is longer than telegram allowed length
            if (text.Length > MAX_TEXT_LENGTH)
            {
                text = text[0..^6];
                if (text[^1] == '\\' && text[^2..^0] != @"\\") text = text[0..^1];
                text += @"\.\.\.";
            }
            var client = new TelegramBotClient(Token);
            return await SendTextMessageAsync(client, text);
        }
    }
}
