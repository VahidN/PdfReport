using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace PdfThumbnailComparer.Lib
{
    public static class ImageExt
    {
        public static byte[] ResizeImage(this Image image, int thumbWidth, int thumbHeight)
        {
            var srcWidth = image.Width;
            var srcHeight = image.Height;
            using (var bmp = new Bitmap(thumbWidth, thumbHeight, PixelFormat.Format32bppArgb))
            {
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.CompositingQuality = CompositingQuality.HighQuality;
                    gr.InterpolationMode = InterpolationMode.High;

                    var rectDestination = new Rectangle(0, 0, thumbWidth, thumbHeight);
                    gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);

                    using (var memStream = new MemoryStream())
                    {
                        bmp.Save(memStream, ImageFormat.Png);
                        return memStream.ToArray();
                    }
                }
            }
        }
    }
}