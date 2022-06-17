using System;
using System.IO;

namespace System.Drawing
{
    public static class ImageHelper
    {
        public static BasicImageInfo GetBasicImageInfo(string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var image = Image.FromStream(stream, false, false);
            return image.GetBasicImageInfo();
        }

        public static BasicImageInfo GetBasicImageInfo(this Image tif) =>
            new BasicImageInfo
            {
                Size = tif.Size,
                PhysicalDimension = tif.PhysicalDimension,
                HorizontalResolution = tif.HorizontalResolution,
                VerticalResolution = tif.VerticalResolution
            };
    }
}
