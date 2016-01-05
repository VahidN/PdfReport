using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DemosBrowser.Toolkit.AcrobatReader;

namespace DemosBrowser.Converters
{
    public class ShellThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || !File.Exists(value.ToString()))
                return null;

            return getShellThumbnail(value);
        }

        private static BitmapSource getShellThumbnail(object fileName)
        {
            int pixelWidth = 150;
            int pixelHeight = 120;//todo: sta thread
            try
            {
                using (var st = new ShellThumbnail { DesiredSize = new System.Drawing.Size(pixelWidth, pixelHeight) })
                {
                    using (var pic = st.GetThumbnail(fileName.ToString()))
                    {
                        return Imaging.CreateBitmapSourceFromHBitmap(
                                    pic.GetHbitmap(),
                                    IntPtr.Zero,
                                    Int32Rect.Empty,
                                    BitmapSizeOptions.FromWidthAndHeight(pixelWidth, pixelHeight));
                    }
                }
            }
            catch (Exception ex)
            {
                //todo: log ...
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}