using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace DemosBrowser.Converters
{
    public class RunDirectionConverter : IValueConverter
    {
        static readonly Regex MatchArabicHebrew = new Regex(@"[\u0600-\u06FF,\u0590-\u05FF]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static bool IsRtl(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;
            return MatchArabicHebrew.IsMatch(data);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return FlowDirection.LeftToRight;

            return IsRtl(value.ToString()) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
