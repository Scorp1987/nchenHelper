using AdaptiveCards;
using AdaptiveCards.Types;
using nchen.Messaging.Exceptions;
using nchen.Messaging.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace nchen.Messaging.Channels
{
    public class EmailChannel : AChannel, IChannel
    {
        public ChannelType Type => ChannelType.Email;
        public string Smtp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }


        public async Task<object> SendAsync(ITemplate template, object data)
        {
            if (!(template is EmailTemplate emailTemplate))
                throw new ArgumentException($"{nameof(template)} must be {nameof(EmailTemplate)}.", nameof(template));

            var doc = new XmlDocument();
            var html = doc.AppendChild(doc.CreateElement("html"));
            html.AppendChild(doc.GetHtmlMailStyle());
            var body = html.AppendChild(doc.CreateElement("body"));

            AppendAdaptiveCard(body, emailTemplate.HeaderTemplateFilePath, data);
            AppendAdaptiveCard(body, emailTemplate.BodyTemplateFilePath, data);
            AppendAdaptiveCard(body, emailTemplate.FooterTemplateFilePath, data);

            try
            {
                using var client = new SmtpClient(Smtp);
                using var mail = new MailMessage(From, To)
                {
                    Subject = emailTemplate.Subject,
                    IsBodyHtml = true
                };
                if (!string.IsNullOrEmpty(Cc)) mail.CC.Add(Cc);
                if (!string.IsNullOrEmpty(Bcc)) mail.Bcc.Add(Bcc);
                mail.Body = doc.OuterXml;
                await client.SendMailAsync(mail);
                await WriteDataAsync(this, "SendMessage", true, doc.OuterXml);
                return mail.Body;
            }
            catch(Exception ex)
            {
                await WriteDataAsync(this, "SendMessage", false, doc.OuterXml, ex);
                throw new SendMessageFailException(this, ex);
            }
        }
        private static void AppendAdaptiveCard(XmlNode node, string filePath, object data)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var json = File.ReadAllText(filePath);
            var card = AdaptiveCardHelper.ExpandAdaptiveTemplate<AdaptiveCard>(json, data);
            card?.Populate(node);
        }
    }
}
