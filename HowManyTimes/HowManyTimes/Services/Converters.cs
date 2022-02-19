﻿using System;
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
                        //return "#D4AF37";
                        return "Red";
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

    /// <summary>
    /// Converter for pinned counter background
    /// </summary>
    public class PinnedCounterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && value != null)
            {
                bool s = (bool)value;
                switch (s)
                {
                    case true:
                        //return "#D4AF37";
                        return "#7ed1b5";
                    default:
                        return "White";
                }
            }

            return "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter for returning oposite value of boolean
    /// </summary>
    public class BoolValueXORConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && value != null)
            {
                bool s = !(bool)value;
                return s;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter for returning disabled color
    /// </summary>
    public class DisabledColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && value != null)
            {
                bool s = (bool)value;
                switch (s)
                {
                    case true:
                        return "Black";
                    default:
                        return "Silver";
                }
            }

            return "Black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
