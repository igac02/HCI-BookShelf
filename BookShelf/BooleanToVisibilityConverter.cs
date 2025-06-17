using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookShelf.Converters
{
    /// <summary>
    /// Converts a boolean value to a Visibility value.
    /// true -> Visibility.Visible
    /// false -> Visibility.Collapsed
    /// This is used in XAML to show or hide UI elements based on a condition.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ConvertBack is not needed for this application.
            throw new NotImplementedException();
        }
    }
}
