using System.Drawing;

namespace System.IO
{
    public static class StreamExtension
    {
        public static BasicImageInfo GetBasicImageInfo(this Stream fileStream)
        {
            using (var tif = Image.FromStream(fileStream, false, false))
                return tif.GetBasicImageInfo();
        }
    }
}
