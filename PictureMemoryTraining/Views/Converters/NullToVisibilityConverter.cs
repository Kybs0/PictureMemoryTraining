using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PictureMemoryTraining.Views.Converters
{
    class NullToVisibilityConverter : IValueConverter
    {
        public bool IsReverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = value is null ? Visibility.Collapsed : Visibility.Visible;
            return IsReverse
                ? (visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible)
                : visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
