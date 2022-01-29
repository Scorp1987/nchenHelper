using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public class ReportPrint : IDisposable
    {
        public static string GetDeviceInfo(PageSettings pageSettings) =>
$@"<DeviceInfo>
<OutputFormat>EMF</OutputFormat>
<PageWidth>{pageSettings.PaperSize.Width * 100.0}in</PageWidth>
<PageHeight>{pageSettings.PaperSize.Height * 100.0}in</PageHeight>
<MarginTop>{pageSettings.Margins.Top * 100.0}in</MarginTop>
<MarginLeft>{pageSettings.Margins.Left * 100.0}in</MarginLeft>
<MarginRight>{pageSettings.Margins.Right * 100.0}in</MarginRight>
<MarginBottom>{pageSettings.Margins.Bottom * 100.0}in</MarginBottom>
</DeviceInfo>";


        private int _currentPageIndex;
        private PrintDocument _document;

        public List<Stream> Streams { get; } = new List<Stream>();

        public Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            var stream = new MemoryStream();
            this.Streams.Add(stream);
            return stream;
        }

        public void Dispose()
        {
            _document?.Dispose();
            _document = null;

            this.Streams.ForEach(s => s.Dispose());
            this.Streams.Clear();
        }

        public void Print(string printerName, PageSettings pageSettings)
        {
            if (this.Streams == null || this.Streams.Count() == 0)
                throw new ArgumentNullException("Error: no stream to print.");

            this.Streams.ForEach(s => s.Position = 0);

            _document = new PrintDocument();
            _document.PrinterSettings.PrinterName = printerName;
            _document.PrinterSettings.FromPage = 0;
            _document.PrinterSettings.ToPage = 0;
            _document.DefaultPageSettings = pageSettings;

            if (!_document.PrinterSettings.IsValid)
                throw new ArgumentException("Error: cannot find the printer.");

            _document.PrintPage += PrintPage;
            _currentPageIndex = 0;
            _document.Print();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            var img = new Metafile(this.Streams[_currentPageIndex]);

            //Adjust rectangular area with printer margins.
            var rect = new Rectangle(
            e.PageBounds.Left - (int)e.PageSettings.HardMarginX,
            e.PageBounds.Top - (int)e.PageSettings.HardMarginY,
            e.PageBounds.Width,
            e.PageBounds.Height);

            //Draw a white background for the report
            e.Graphics.FillRectangle(Brushes.White, rect);

            //Draw the report content
            e.Graphics.DrawImage(img, rect);

            //Prepare for the next page. Make sure we haven't hit the end.
            _currentPageIndex++;
            e.HasMorePages = (_currentPageIndex < this.Streams.Count);
        }
    }
}
