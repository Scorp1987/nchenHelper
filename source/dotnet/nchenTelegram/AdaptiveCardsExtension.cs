using AdaptiveCards.Enums;
using AdaptiveCards.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //var topBlankLineCount = (int)Math.Floor(blankLineCount / 2f);
                //var bottomBlankLineCount = (int)Math.Ceiling(blankLineCount / 2f);
                //for (int rowIndex = 0; rowIndex < topBlankLineCount; rowIndex++)
                //    toReturn[rowIndex, columnIndex] = new string(' ', width);
                //for (int rowIndex = topBlankLineCount; rowIndex < topBlankLineCount + lines.Length; rowIndex++)
                //    toReturn[rowIndex, columnIndex] = lines[rowIndex - topBlankLineCount];
                //for (int rowIndex = topBlankLineCount + lines.Length; rowIndex < topBlankLineCount + lines.Length + bottomBlankLineCount; rowIndex++)
                //    toReturn[rowIndex, columnIndex] = new string(' ', width);
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


        private static string Render(this TextBlock textBlock) =>
            (textBlock.Weight == FontWeight.Bolder) ? $"*{textBlock.Text.FixAllOthers()}*" : textBlock.Text.FixAllOthers();
        private static string Render(this TextRun textRun)
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
        private static string Render(this Fact fact, int titleLength)
        {
            var valueStr = fact.Value.Render();
            var valueLines = valueStr.Split('\n');

            var titleStr = fact.Title.Render();

            var toReturn = $"{titleStr.PadRight(titleLength)}: ".ToInlineCode() + valueLines[0];
            for (var i = 1; i < valueLines.Length; i++)
                toReturn += "\n" + $"{"".PadRight(titleLength)}  ".ToInlineCode() + valueLines[i];
            return toReturn;
        }
        private static string Render(this Table table)
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

        private static string Render(this IEnumerable<IElement> elements)
        {
            var toReturn = "";
            foreach (var item in elements)
                toReturn += $"\n{item?.Render()}";
            return toReturn[1..];
        }
        private static string Render(this IEnumerable<TextRun> textRuns)
        {
            var toReturn = "";
            foreach (var item in textRuns)
                toReturn += item.Render();
            return toReturn;
        }

        private static string Render(this FactSet factSet)
        {
            var maxTitleLength = factSet.Facts.Select(fact => fact.Title.GetText().Length).Max();

            var toReturn = "";
            foreach (var fact in factSet.Facts)
                toReturn += $"\n{fact.Render(maxTitleLength)}";
            return toReturn[1..];
        }
        private static string Render(this RichTextBlock richTextBlock) => richTextBlock.Inlines.Render();
        private static string Render(this Container container) => container.Items.Render();
        private static string Render(this IElement element)
        {
            if (element is TextBlock tb) return tb.Render();
            else if (element is RichTextBlock rtb) return rtb.Render();
            else if (element is FactSet fs) return fs.Render();
            else if (element is Container ct) return ct.Render();
            else if (element is Table tbl) return tbl.Render();
            else throw new NotImplementedException();
        }

        public static string RenderTelegramMessage(this AdaptiveCard adaptiveCard) => adaptiveCard.Body.Render();
    }
}
