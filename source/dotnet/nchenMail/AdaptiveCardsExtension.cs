using AdaptiveCards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AdaptiveCards.Types
{
    public static class AdaptiveCardsExtension
    {
        private static string ToHtmlColor(this Color color) => color switch
        {
            Color.Good => "Green",
            Color.Attention => "Red",
            Color.Warning => "Orange",
            Color.Light => "Grey",
            Color.Dark => "Black",
            Color.Accent => "Blue",
            _ => null,
        };
        private static string ToHtmlFontSize(this FontSize size) => size switch
        {
            FontSize.Small => "10pt",
            FontSize.Medium => "15pt",
            FontSize.Large => "18pt",
            FontSize.ExtraLarge => "22pt",
            _ => "12pt",
        };
        private static string ToHtmlFontWeight(this FontWeight weight) => weight switch
        {
            FontWeight.Bolder => "bolder",
            FontWeight.Lighter => "lighter",
            _ => null,
        };


        private static string GetXml(this string str) => 
            str
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("&", "&#38;")
                .Replace("'", "&#39;")
                .Replace("\"", "&#34;")
                .Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
        private static string GetXml(this TextRun textRun)
        {
            var text = textRun.Text.GetXml();
            if (textRun.Weight == FontWeight.Bolder) text = $"<b>{text}</b>";
            if (textRun.Italic) text = $"<i>{text}</i>";
            if (textRun.Underline) text = $"<u>{text}</u>";
            if (textRun.Strikethrough) text = $"<del>{text}</del>";

            var color = textRun.Color?.ToHtmlColor();
            var size = textRun.Size?.ToHtmlFontSize();
            //var weight = textRun.Weight?.ToHtmlFontWeight();

            string styleText = "";
            if (color != null) styleText += $"color:{color};";
            if (size != null) styleText += $"font-size:{size};";
            //if (weight != null) styleText += $"font-weight:{weight};";
            if (!string.IsNullOrEmpty(styleText)) styleText = $" style=\"{styleText}\"";

            if (textRun is Link lnk)
                text = $"<a href=\"{lnk.URL.GetXml()}\">{text}</a>";
            
            if (!string.IsNullOrEmpty(styleText))
                text = $"<span{styleText}>{text}</span>";

            return text;
        }
        private static string GetXml(this IEnumerable<TextRun> textRuns)
        {
            var toReturn = "";
            foreach (var item in textRuns)
                toReturn += item.GetXml();
            return toReturn;
        }


        private static bool IsPercentWidth(this TableColumnDefinition column) => column.Width.Type == TableColumnWidthType.Absolute || column.Width.Type == TableColumnWidthType.Max;
        private static int GetTotalWidth(this IEnumerable<TableColumnDefinition> columns) =>
            (from column in columns
             where column.IsPercentWidth()
             select column.Width.Value).Sum();


        private static XmlNode Render(this TextBlock textBlock, XmlDocument doc)
        {
            var color = textBlock.Color?.ToHtmlColor();
            var size = textBlock.Size?.ToHtmlFontSize();
            var weight = textBlock.Weight?.ToHtmlFontWeight();
            var align = textBlock.HorizontalAlignment?.ToString();

            string styleText = "";
            if (color != null) styleText += $"color:{color};";
            if (size != null) styleText += $"font-size:{size};";
            if (weight != null) styleText += $"font-weight:{weight};";

            var div = doc.CreateElement("div");
            div.InnerXml = textBlock.Text.GetXml();
            if(align != null) div.Attributes.Append(doc.CreateAttribute("align")).Value = align;
            if (!string.IsNullOrEmpty(styleText)) div.Attributes.Append(doc.CreateAttribute("style")).Value = styleText;
            return div;
        }
        private static XmlNode Render(this RichTextBlock richTextBlock, XmlDocument doc)
        {
            var div = doc.CreateElement("div");
            string xml = richTextBlock.Inlines.GetXml();
            div.InnerXml = xml;
            return div;
        }
        private static XmlNode Render(this FactSet factSet, XmlDocument doc)
        {
            if (factSet.Facts.Length < 1) return null;

            var toReturn = doc.CreateElement("table");
            toReturn.Attributes.Append(doc.CreateAttribute("class")).Value = "factset";
            foreach (var fact in factSet.Facts)
            {
                var rowElement = toReturn.AppendChild(doc.CreateElement("tr"));
                rowElement.Attributes.Append(doc.CreateAttribute("class")).Value = "fact";

                var titleElement = rowElement.AppendChild(doc.CreateElement("td"));
                titleElement.Attributes.Append(doc.CreateAttribute("class")).Value = "factTitle";
                titleElement.InnerXml = fact.Title.GetXml();

                var seperatorElement = rowElement.AppendChild(doc.CreateElement("td"));
                seperatorElement.Attributes.Append(doc.CreateAttribute("class")).Value = "factSeperator";
                seperatorElement.InnerText = ":";

                var valueElement = rowElement.AppendChild(doc.CreateElement("td"));
                valueElement.Attributes.Append(doc.CreateAttribute("class")).Value = "factValue";
                valueElement.InnerXml = fact.Value.GetXml();
            }
            return toReturn;
        }
        private static XmlNode Render(this Container container, XmlDocument doc)
        {
            var div = doc.CreateElement("div");
            foreach (var item in container.Items)
            {
                var element = item.Render(doc);
                if (element != null)
                    div.AppendChild(element);
            }
            return div;
        }
        private static XmlNode Render(this Table table, XmlDocument doc)
        {
            if (table.Columns.Length < 1 || table.Rows.Length < 1) return null;

            var toReturn = doc.CreateElement("table");
            toReturn.Attributes.Append(doc.CreateAttribute("class")).Value = "table";

            bool firstRow = table.FirstRowAsHeader;

            var totalWidth = table.Columns.GetTotalWidth();
            foreach (var row in table.Rows)
            {
                var rowElement = toReturn.AppendChild(doc.CreateElement("tr"));
                for (int colIndex = 0; colIndex < table.Columns.Length; colIndex++)
                {
                    XmlElement colElement;
                    if (firstRow)
                    {
                        colElement = doc.CreateElement("th");
                        colElement.Attributes.Append(doc.CreateAttribute("class")).Value = "tableHeader";
                        var column = table.Columns[colIndex];
                        if (column.IsPercentWidth())
                        {
                            var width = column.Width.Value;
                            var widthPercent = 100f * width / totalWidth;
                            colElement.Attributes.Append(doc.CreateAttribute("style")).Value = $"width:{widthPercent:0.00}%";
                        }
                    }
                    else
                    {
                        colElement = doc.CreateElement("td");
                        colElement.Attributes.Append(doc.CreateAttribute("class")).Value = "tableData";
                    }


                    var cell = row.Cells[colIndex];
                    foreach (var item in cell.Items)
                        colElement.AppendChild(item.Render(doc));

                    if (cell.VerticalContentAlignment.HasValue)
                        colElement.Attributes.Append(doc.CreateAttribute("style")).Value = $"vertical-align:{cell.VerticalContentAlignment.Value}";

                    rowElement.AppendChild(colElement);
                }

                if (firstRow)
                    firstRow = false;
            }

            return toReturn;
        }
        private static XmlNode Render(this IElement element, XmlDocument doc)
        {
            if (element is TextBlock tb) return tb.Render(doc);
            else if (element is RichTextBlock rtb) return rtb.Render(doc);
            else if (element is FactSet fs) return fs.Render(doc);
            else if (element is Container ct) return ct.Render(doc);
            else if (element is Table tbl) return tbl.Render(doc);
            else return null;
        }


        public static void Populate(this AdaptiveCard adaptiveCard, XmlNode parentNode)
        {
            foreach (var item in adaptiveCard.Body)
            {
                var element = item.Render(parentNode.OwnerDocument);
                if (element != null) parentNode.AppendChild(element);
            }
        }
        public static XmlNode GetHtmlMailStyle(this XmlDocument doc)
        {
            var style = doc.CreateElement("style");
            style.InnerText = "\n.table{border-collapse: collapse;}" +
                "\n.tableHeader,.tableData{border: 1px solid black; padding: 3px;}" +
                "\n.factTitle,.factSeperator{vertical-align: top; font-weight: bolder; padding: 0px;}" +
                "\n.factValue{padding: 0px 0px 0px 10px;}";
            return style;
        }
    }
}