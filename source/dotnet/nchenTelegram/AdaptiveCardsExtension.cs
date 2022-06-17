using AdaptiveCards.Enums;
using AdaptiveCards.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;

namespace AdaptiveCards.Types
{
    public static class AdaptiveCardsExtension
    {
        private static int GetWidth(this string input)
        {
            input = input.Replace("\r", "");
            var lines = input.Split('\n');
            return lines.Select(line => line.Length).Max();
        }
        private static int GetWidth(this TextBlock textBlock) => textBlock.Text.GetWidth();
        private static int GetWidth(this IEnumerable<TextRun> textRuns) => textRuns.GetText().GetWidth();
        private static int GetWidth(this TableCell tableCell) =>
            tableCell.Items.Select(item =>
            {
                if (item is TextBlock tb) return tb.GetWidth();
                else if (item is RichTextBlock rtb) return rtb.Inlines.GetWidth();
                else return 0;
            }).Max();
        private static int[] GetWidths(this Table table)
        {
            var widths = new int[table.Columns.Length];
            for(var columnIndex = 0; columnIndex < widths.Length; columnIndex++)
            {
                int width = 0;
                switch (table.Columns[columnIndex].Width.Type)
                {
                    case TableColumnWidthType.Absolute:
                        width = table.Columns[columnIndex].Width.Value;
                        break;
                    case TableColumnWidthType.Auto:
                        width = table.Rows.Select(row => row.Cells[columnIndex].GetWidth()).Max();
                        break;
                    case TableColumnWidthType.Max:
                        var width1 = table.Columns[columnIndex].Width.Value;
                        var width2 = table.Rows.Select(row => row.Cells[columnIndex].GetWidth()).Max();
                        width = Math.Min(width1, width2);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                widths[columnIndex] = width;
            }
            return widths;
        }

        private static int GetHeight(this string input, int width)
        {
            input = input.Replace("\r", "");
            var lines = input.Split('\n');
            return lines.Select(line =>
            {
                var textLength = line.Length;
                return (int)Math.Ceiling((double)textLength / width);
            }).Sum();
        }
        private static int GetHeight(this TextBlock textBlock, int width) => textBlock.Text.GetHeight(width);
        private static int GetHeight(this IEnumerable<TextRun> textRuns, int width) => textRuns.GetText().GetHeight(width);
        private static int GetHeight(this TableCell tableCell, int width) =>
            tableCell.Items.Select(item =>
            {
                if (item is TextBlock tb) return tb.GetHeight(width);
                else if (item is RichTextBlock rtb) return rtb.Inlines.GetHeight(width);
                else return 0;
            }).Sum();
        private static int GetHeight(this TableRow tableRow, int[] widths)
        {
            var toReturn = 0;
            for(var columnIndex = 0; columnIndex < widths.Length; columnIndex++)
            {
                var height = tableRow.Cells[columnIndex].GetHeight(widths[columnIndex]);
                if (height > toReturn) toReturn = height;
            }
            return toReturn;
        }

        private static string GetText(this IEnumerable<TextRun> textRuns)
        {
            var toReturn = "";
            foreach (var item in textRuns)
                toReturn += item.Text;
            return toReturn;
        }
        private static string GetText(this string input, int width, HorizontalAlignment horizontalAlignment)
        {
            input = input.Replace("\r", "");
            var lines = input.Split('\n');

            var toReturn = "";
            foreach (var line in lines)
            {
                var textLength = line.Length;
                var lineCount = (int)Math.Ceiling((double)textLength / width);

                for (int i = 0; i < lineCount - 1; i++)
                {
                    toReturn += $"\n{line.Substring(i * width, width)}";
                }

                var startPos = (lineCount - 1) * width;
                var length = textLength - startPos;
                var lineText = line.Substring(startPos, length);
                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        toReturn += $"\n{lineText.PadRight(width)}";
                        break;
                    case HorizontalAlignment.Right:
                        toReturn += $"\n{lineText.PadLeft(width)}";
                        break;
                    case HorizontalAlignment.Center:
                        var lineLength = lineText.Length;
                        var spaceCount = width - lineLength;
                        var leftSpaceCount = (int)Math.Ceiling(spaceCount / 2f);
                        var rightSpaceCount = (int)Math.Floor(spaceCount / 2f);
                        toReturn += $"\n{new string(' ', leftSpaceCount)}{lineText}{new string(' ', rightSpaceCount)}";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return toReturn[1..];
        }
        private static string GetText(this TextBlock textBlock, int width) =>
            textBlock.Text.GetText(width, textBlock.HorizontalAlignment ?? HorizontalAlignment.Left);
        private static string GetText(this RichTextBlock richTextBlock, int width)
        {
            var text = richTextBlock.Inlines.GetText();
            var toReturn = GetText(text, width, richTextBlock.HorizontalAlignment ?? HorizontalAlignment.Left);
            return toReturn;
        }
        private static string GetText(this TableCell tableCell, int width)
        {
            var toReturn = "";
            foreach (var item in tableCell.Items)
                if (item is TextBlock tb)
                    toReturn += $"\n{tb.GetText(width)}";
                else if (item is RichTextBlock rtb)
                    toReturn += $"\n{rtb.GetText(width)}";
            return toReturn[1..];
        }

        private static string[,] GetLines(this TableRow tableRow, int[] widths, int height)
        {
            var toReturn = new string[height, widths.Length];
            for(var columnIndex = 0; columnIndex < widths.Length; columnIndex++)
            {
                var width = widths[columnIndex];
                var text = tableRow.Cells[columnIndex].GetText(width);
                var lines = text.Split('\n');
                var blankLineCount = height - lines.Length;
                for (int rowIndex = 0; rowIndex < lines.Length; rowIndex++)
                    toReturn[rowIndex, columnIndex] = lines[rowIndex];
                for (int rowIndex = lines.Length; rowIndex < lines.Length + blankLineCount; rowIndex++)
                    toReturn[rowIndex, columnIndex] = new string(' ', width);
            }
            return toReturn;
        }
        private static string Render(this TableRow tableRow, int[] widths)
        {
            var height = tableRow.GetHeight(widths);
            var lines = tableRow.GetLines(widths, height);
            var toReturn = "";
            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                var line = "";
                for (var columnIndex = 0; columnIndex < widths.Length; columnIndex++)
                    line += $"|{lines[rowIndex, columnIndex]}";
                toReturn += $"\n{line[1..]}";
            }
            return toReturn[1..];
        }


        public static string ToMarkDownV2Text(this TextBlock textBlock) =>
            (textBlock.Weight == FontWeight.Bolder) ? $"*{textBlock.Text.FixAllOthers()}*" : textBlock.Text.FixAllOthers();
        public static string ToMarkDownV2Text(this TextRun textRun)
        {
            if (textRun.InLineCode)
                return textRun.Text.ToInlineCode();

            var toReturn = textRun.Text.FixAllOthers();
            
            if (textRun is Link lnk)
                toReturn = $"[{toReturn}]({lnk.URL})";

            if (textRun.Weight == FontWeight.Bolder)
                toReturn = $"*{toReturn}*";
            if (textRun.Italic)
                toReturn = $"_{toReturn}_";
            if (textRun.Underline)
                toReturn = $"__{toReturn}__";
            if (textRun.Strikethrough)
                toReturn = $"~{toReturn}~";
            if (textRun.Spoiler)
                toReturn = $"||{toReturn}||";

            return toReturn;
        }
        public static string ToMarkDownV2Text(this Fact fact, int titleLength, bool seperator)
        {
            var valueStr = fact.Value.ToMarkDownV2Text();
            var valueLines = valueStr.Split('\n');

            var titleStr = fact.Title.GetText();
            string toReturn;
            if (seperator)
            {
                toReturn = $"{titleStr.PadRight(titleLength)}: ".ToInlineCode() + valueLines[0];
                for (var i = 1; i < valueLines.Length; i++)
                    toReturn += "\n" + $"{"".PadRight(titleLength)}  ".ToInlineCode() + valueLines[i];
            }
            else
            {
                toReturn = $"{titleStr.PadRight(titleLength)} ".ToInlineCode() + valueLines[0];
                for (var i = 1; i < valueLines.Length; i++)
                    toReturn += "\n" + $"{"".PadRight(titleLength)} ".ToInlineCode() + valueLines[i];
            }
            return toReturn;
        }
        public static string ToMarkDownV2Text(this Table table)
        {
            if (table.Rows.Length < 1) return "";

            var widths = table.GetWidths();
            var toReturn = table.Rows[0].Render(widths);
            if (table.FirstRowAsHeader)
            {
                var headerLine = "";
                foreach (var width in widths)
                    headerLine += $"|{new string('-', width)}";
                toReturn += $"\n{headerLine[1..]}";
            }
            for (var rowIndex = 1; rowIndex < table.Rows.Length; rowIndex++)
                toReturn += $"\n{table.Rows[rowIndex].Render(widths)}";
            return $"```\n{toReturn.FixInLineCode()}\n```";
        }
        public static string ToMarkDownV2Text(this IEnumerable<IElement> elements)
        {
            var toReturn = "";
            foreach (var item in elements)
            {
                var text = item?.ToMarkDownV2Text();
                if(!string.IsNullOrEmpty(text))
                    toReturn += $"\n{text}";
            }
            return toReturn.Length>0 ? toReturn[1..] : toReturn;
        }
        public static string ToMarkDownV2Text(this IEnumerable<TextRun> textRuns)
        {
            var toReturn = "";
            if (textRuns == null) return null;
            foreach (var item in textRuns)
                toReturn += item.ToMarkDownV2Text();
            return toReturn;
        }
        public static string ToMarkDownV2Text(this FactSet factSet)
        {
            var maxTitleLength = factSet.Facts.Select(fact => fact.Title.GetText().Length).Max();

            var toReturn = "";
            foreach (var fact in factSet.Facts)
                toReturn += $"\n{fact.ToMarkDownV2Text(maxTitleLength, factSet.Seperator)}";
            return toReturn[1..];
        }
        public static string ToMarkDownV2Text(this RichTextBlock richTextBlock) => richTextBlock.Inlines.ToMarkDownV2Text();
        public static string ToMarkDownV2Text(this Container container) => container.Items.ToMarkDownV2Text();
        public static string ToMarkDownV2Text(this IElement element)
        {
            if (element is TextBlock tb) return tb.ToMarkDownV2Text();
            else if (element is RichTextBlock rtb) return rtb.ToMarkDownV2Text();
            else if (element is FactSet fs) return fs.ToMarkDownV2Text();
            else if (element is Container ct) return ct.ToMarkDownV2Text();
            else if (element is Table tbl) return tbl.ToMarkDownV2Text();
            else return null;
        }


        private static IInput[] GetInputs(this IEnumerable<IElement> elements)
        {
            var toReturn = new List<IInput>();
            foreach (var element in elements)
            {
                if (element is IInput input) toReturn.Add(input);
                else if (element is Container container) toReturn.AddRange(container.Items.GetInputs());
                else if (element is Table table)
                    foreach (var row in table.Rows)
                        foreach (var cell in row.Cells)
                            toReturn.AddRange(cell.Items.GetInputs());
            }
            return toReturn.ToArray();
        }


        private static string GetValue(this InputText template, string text)
        {
            if (template.MaxLength.HasValue && text.Length > template.MaxLength)
                throw new InvalidInputException($"'{text}' is more than the allowed {template.MaxLength} character length.");
            if (!string.IsNullOrEmpty(template.Regex))
            {
                var match = Regex.Match(text, template.Regex);
                if (!match.Success)
                    throw new InvalidInputException($"'{text}' is not match to the '{template.Regex}' pattern.");
            }
            return text;
        }
        private static DateTime GetValue(this InputDate template, string text)
        {
            if (!DateTime.TryParse(text, out var toReturn))
                throw new InvalidInputException($"'{text}' is not a valid Date.");
            if (template.Min.HasValue && toReturn < template.Min.Value)
                throw new InvalidInputException($"'{text}' is earlier than '{template.Min}'.");
            if (template.Max.HasValue && toReturn > template.Max.Value)
                throw new InvalidInputException($"'{text}' is later than '{template.Max}'.");
            return toReturn;
        }
        private static DateTime GetValue(this InputTime template, string text)
        {
            if (!DateTime.TryParse(text, out var toReturn))
                throw new InvalidInputException($"'{text}' is not a valid Time.");
            if (template.Min.HasValue && toReturn < template.Min.Value)
                throw new InvalidInputException($"'{text}' is earlier than '{template.Min}'.");
            if (template.Max.HasValue && toReturn > template.Max.Value)
                throw new InvalidInputException($"'{text}' is later than '{template.Max}'.");
            return toReturn;
        }
        private static decimal GetValue(this InputNumber template, string text)
        {
            if (!decimal.TryParse(text, out var output))
                throw new InvalidInputException($"'{text}' is not a valid number.");
            if (template.Min.HasValue && output < template.Min.Value)
                throw new InvalidInputException($"'{text}' is less than '{template.Min}'.");
            if (template.Max.HasValue && output > template.Max.Value)
                throw new InvalidInputException($"'{text}' is greater than '{template.Max}'.");
            return output;
        }
        private static string GetValue(this InputChoiceSet template, string prefix, string inlineData)
        {
            if (!inlineData.StartsWith(prefix))
                throw new InvalidInputException($"'{inlineData}' is not valid.");
            var data = inlineData[prefix.Length..];
            if (!template.Choices.Select(choice => choice.Value).Contains(data))
                throw new InvalidInputException($"'{data}' is not a valid choice.");
            return data;
        }
        private static bool GetValue(this InputToogle template, string prefix, string inlineData)
        {
            if (!inlineData.StartsWith(prefix))
                throw new InvalidInputException($"'{inlineData}' is not valid.");
            var data = inlineData[prefix.Length..];

            if (data == template.ValueOn)
                return true;
            else if (data == template.ValueOff)
                return false;
            else
                throw new InvalidInputException($"'{data}' is not a valid choice.");
        }


        public static string ToMarkDownV2Text(this AdaptiveCard adaptiveCard) => adaptiveCard.Body.ToMarkDownV2Text();
        public static IInput[] GetInputs(this AdaptiveCard adaptiveCard)
        {
            var toReturn = new List<IInput>();
            toReturn.AddRange(adaptiveCard.Body.GetInputs());
            return toReturn.ToArray();
        }
        public static object GetValue(this IInput input, string value, string inlineDataPrefix = null)
        {
            if (input is InputText inputText)
                return inputText.GetValue(value);
            else if (input is InputNumber inputNumber)
                return inputNumber.GetValue(value);
            else if (input is InputDate inputDate)
                return inputDate.GetValue(value);
            else if (input is InputTime inputTime)
                return inputTime.GetValue(value);
            else if (input is InputToogle inputToogle)
                return inputToogle.GetValue(inlineDataPrefix, value);
            else if (input is InputChoiceSet inputChoiceSet)
                return inputChoiceSet.GetValue(inlineDataPrefix, value);
            else
                throw new NotImplementedException($"'{input.Type}' is not implemented in {nameof(GetValue)}.");
        }
    }
}
