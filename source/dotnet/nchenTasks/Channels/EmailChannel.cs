using AdaptiveCards.Types;
using nchen.Enums;
using nchen.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nchen.Channels
{
    public class EmailChannel : AChannel
    {
        public override ChannelType Type => ChannelType.Email;
        public string Smtp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }


        public override async Task<string> SendAsync(ITemplate template, object data)
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

            using (var client = new SmtpClient(Smtp))
            using (var mail = new MailMessage(From, To)
            {
                Subject = emailTemplate.Subject,
                IsBodyHtml = true
            })
            {
                if (!string.IsNullOrEmpty(Cc)) mail.CC.Add(Cc);
                if (!string.IsNullOrEmpty(Bcc)) mail.Bcc.Add(Bcc);
                mail.Body = doc.OuterXml;

                await client.SendMailAsync(mail);
                return mail.Body;
            }
        }
        private static void AppendAdaptiveCard(XmlNode node, string filePath, object data)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var json = File.ReadAllText(filePath);
            var card = TaskHelper.ExpandAdaptiveTemplate<AdaptiveCard>(json, data);
            card?.Populate(node);
        }
    }
}
