using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace System.Net.Mail
{
    public class HtmlMailMessage : MailMessage
    {
        private readonly XmlDocument doc;
        private readonly XmlElement style;
        private readonly XmlElement body;

        public HtmlMailMessage(string from, string to, string subject, string cc = null)
        {
            base.From = new MailAddress(from);
            base.To.Add(to);
            if (!string.IsNullOrWhiteSpace(cc)) base.CC.Add(cc);
            base.Subject = subject;
            base.IsBodyHtml = true;

            this.doc = new XmlDocument();
            this.style = doc.CreateElement("style");
            this.style.InnerText = "\ntable{border-collapse: collapse}";
            this.style.InnerText = "\nth,td{border: 1px solid black}";
            this.body = doc.CreateElement("body");
            var html = doc.AppendChild(this.doc.CreateElement("html"));
            html.AppendChild(this.style);
            html.AppendChild(this.body);
            UpdateBody();
        }

        public void AddText(
            string text,
            string name,
            string align = null,
            string width = null,
            string color = null,
            string fontSize = null,
            string fontWeight = null)
        {
            if (width != null || color != null || fontSize != null || fontWeight != null)
            {
                style.InnerText += $"\n.{name}" + "{";
                if (width != null) style.InnerText += "width:" + width + ";";
                if (color != null) style.InnerText += "color:" + color + ";";
                if (fontSize != null) style.InnerText += "font-size:" + fontSize + ";";
                if (fontWeight != null) style.InnerText += "font-weight:" + fontWeight + ";";
                style.InnerText += "}";
            }
            var div = this.body.AppendChild(doc.CreateElement("div"));
            div.InnerXml = text.Replace("\n", "<br />");
            div.Attributes.Append(doc.CreateAttribute("class")).Value = name;
            if (align != null) div.Attributes.Append(doc.CreateAttribute("align")).Value = align;

            UpdateBody();
        }


        public void AddList(IEnumerable list)
        {
            var ulObj = this.body.AppendChild(doc.CreateElement("ul"));
            ulObj.Attributes.Append(doc.CreateAttribute("style")).Value = "padding: 0; Margin: 0 0 0 40px;";
            foreach (var item in list)
            {
                var liObj = ulObj.AppendChild(doc.CreateElement("li"));
                liObj.Attributes.Append(doc.CreateAttribute("style")).Value = "Margin: 0 0 0 0;";
                liObj.InnerText = item.ToString();
            }

            UpdateBody();
        }

        public void AddTable(
            DataTable table,
            string name,
            string width = null,
            string cellPadding = null,
            bool showTableHeader = true,
            IEnumerable<string> columnNames = null)
        {
            if (width != null) this.style.InnerText += "\n." + name + "{width:" + width + ";}";
            var tblObj = this.body.AppendChild(doc.CreateElement("table"));
            tblObj.Attributes.Append(doc.CreateAttribute("class")).Value = name;
            if (cellPadding != null) tblObj.Attributes.Append(doc.CreateAttribute("cellpadding")).Value = cellPadding;
            var tr = tblObj.AppendChild(doc.CreateElement("tr"));
            if (showTableHeader)
                foreach (DataColumn col in table.Columns)
                {
                    if (columnNames?.Contains(col.ColumnName) == false) continue;
                    tr.AppendChild(doc.CreateElement("th")).InnerText = col.ColumnName;
                }
            foreach (DataRow row in table.Rows)
            {
                tr = tblObj.AppendChild(doc.CreateElement("tr"));
                foreach (DataColumn col in table.Columns)
                {
                    if (columnNames?.Contains(col.ColumnName) == false) continue;
                    var td = tr.AppendChild(doc.CreateElement("td"));
                    object value = row[col];
                    if (value is DateTime dt)
                    {
                        if (dt.TimeOfDay.TotalSeconds == 0) td.InnerText = dt.ToString("dd-MMM-yyyy");
                        else td.InnerText = dt.ToString("dd-MMM-yyyy HH:mm:ss");
                    }
                    else if (value is int || value is decimal || value is Single || value is double)
                    {
                        td.InnerText = value.ToString();
                        td.Attributes.Append(doc.CreateAttribute("align")).Value = "right";
                    }
                    else
                    {
                        td.InnerText = value.ToString();
                    }
                }
            }

            UpdateBody();
        }

        public void AddTable<TObject>(
            IEnumerable<TObject> list,
            string name,
            string width = null,
            string cellPadding = null,
            bool showTableHeader = true)
            where TObject : class
        {
            var properties = typeof(TObject).GetProperties();
            AddTable(list, name, properties, width, cellPadding, showTableHeader);
        }

        public void AddTable<TObject, TAttribute>(
            IEnumerable<TObject> list,
            string name,
            string width = null,
            string cellPadding = null,
            bool showTableHeader = true)
            where TObject : class
            where TAttribute : Attribute
        {
            var properties = from property in typeof(TObject).GetProperties()
                             let attributes = property.GetCustomAttributes<TAttribute>()
                             where attributes.Count() > 0
                             select property;
            AddTable(list, name, properties, width, cellPadding, showTableHeader);
        }


        private void AddTable<TObject>(
            IEnumerable<TObject> list,
            string name,
            IEnumerable<PropertyInfo> properties,
            string width = null,
            string cellPadding = null,
            bool showTableHeader = true)
            where TObject : class
        {
            if (width != null) this.style.InnerText += "\n." + name + "{width:" + width + "}";
            var tblObj = this.body.AppendChild(doc.CreateElement("table"));
            tblObj.Attributes.Append(doc.CreateAttribute("class")).Value = name;
            if (cellPadding != null) tblObj.Attributes.Append(doc.CreateAttribute("cellpadding")).Value = cellPadding;
            var tr = tblObj.AppendChild(doc.CreateElement("tr"));
            if (showTableHeader)
                foreach (var property in properties)
                    tr.AppendChild(doc.CreateElement("th")).InnerText = property.Name;
            foreach (var item in list)
            {
                foreach (var property in properties)
                {
                    var td = tr.AppendChild(doc.CreateElement("td"));
                    object value = property.GetValue(item);
                    if (value is DateTime dt)
                    {
                        if (dt.TimeOfDay.TotalSeconds == 0) td.InnerText = dt.ToString("dd-MMM-yyyy");
                        else td.InnerText = dt.ToString("dd-MMM-yyyy HH:mm:ss");
                    }
                    else if (value is int || value is decimal || value is Single || value is double)
                    {
                        td.InnerText = value.ToString();
                        td.Attributes.Append(doc.CreateAttribute("align")).Value = "right";
                    }
                    else
                    {
                        td.InnerText = value.ToString();
                    }
                }
            }

            UpdateBody();
        }

        public void AddImage(
            string fileName,
            string name,
            string width)
        {
            Attachment oAttachment = new Attachment(fileName);
            string contentID = oAttachment.ContentId;
            base.Attachments.Add(oAttachment);

            if (width != null) this.style.InnerText += "\n." + name + "{width:" + width + "}";
            var img = this.body.AppendChild(doc.CreateElement("imag"));
            img.Attributes.Append(doc.CreateAttribute("class")).Value = name;
            img.Attributes.Append(doc.CreateAttribute("src")).Value = "cid:" + contentID;

            UpdateBody();
        }

        private void UpdateBody() => base.Body = doc.OuterXml;
    }
}
