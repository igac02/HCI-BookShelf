using System;
using System.Globalization;
using System.Windows.Data;

namespace BookShelf.Converters
{
    /// <summary>
    /// Converts a numeric rating (e.g., 4) into a string of star characters (e.g., "★★★★☆").
    /// Assumes a 5-star rating system.
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Provjeravamo da li je vrijednost validan cijeli broj
            if (value is int rating)
            {
                // Osiguravamo da je ocjena u rangu 1-5
                rating = Math.Max(0, Math.Min(5, rating));

                // Kreiramo string sa punim i praznim zvjezdicama
                string fullStars = new string('★', rating); // '★' je karakter pune zvijezde
                string emptyStars = new string('☆', 5 - rating); // '☆' je karakter prazne zvijezde

                return fullStars + emptyStars;
            }

            // Ako vrijednost nije broj, vraćamo prazan string ili neku defaultnu vrijednost
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Konverzija nazad nam nije potrebna
            throw new NotImplementedException();
        }
    }
}