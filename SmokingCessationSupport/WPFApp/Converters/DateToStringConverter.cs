﻿using System.Globalization;
using System.Windows.Data;

namespace WPFApp.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
                return date.ToString("dd/MM/yyyy");
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DateTime.TryParse(value as string, out var date))
                return date;
            return null;
        }
    }
}