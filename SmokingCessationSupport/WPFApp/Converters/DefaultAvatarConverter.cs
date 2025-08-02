﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WPFApp.Converters
{
    class DefaultAvatarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? new BitmapImage(new Uri("pack://application:,,,/Assets/default-avatar.png"))
                : new BitmapImage(new Uri(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
