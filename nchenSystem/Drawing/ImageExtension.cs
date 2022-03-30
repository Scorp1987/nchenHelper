using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Drawing
{
    public static class ImageExtension
    {
        public static BasicImageInfo GetBasicImageInfo(this Image tif)
        {
            return new BasicImageInfo
            {
                Size = tif.Size,
                PhysicalDimension = tif.PhysicalDimension,
                HorizontalResolution = tif.HorizontalResolution,
                VerticalResolution = tif.VerticalResolution
            };
        }
    }
}
