using System;
using System.Globalization;
using System.Windows.Data;

namespace BookShelf.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && parameter is string paramString)
            {
                return intValue == int.Parse(paramString);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return int.Parse((string)parameter);
            }
            return null;
        }
    }
}
