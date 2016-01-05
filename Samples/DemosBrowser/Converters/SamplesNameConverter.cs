using System;
using System.Globalization;
using System.Windows.Data;

namespace DemosBrowser.Converters
{
    public class SamplesNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return string.Empty;
            var parts = value.ToString().Split('.');
            return parts[2].Replace("PdfReport", string.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
