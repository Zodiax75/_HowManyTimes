using System;
using Xamarin.Forms;

namespace HowManyTimes.Services
{
    /// <summary>
    /// Converter for favorite icon background
    /// </summary>
    public class FavoriteIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && value != null)
            {
                bool s = (bool)value;
                switch (s)
                {
                    case true:
                        return "#D4AF37";
                    default:
                        return "#6F6F6F";
                }
            }

            return "#6F6F6F";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
