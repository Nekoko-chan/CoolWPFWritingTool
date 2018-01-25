using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TestElements.global
{
    public class TagsVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length <2) throw new ArgumentException("Not enough parameters");

            var strings = (ObservableCollection<string>) values[0];

            return values.Skip(1).Any(value => strings.Any(elem => string.Equals(elem , (string) value, StringComparison.CurrentCultureIgnoreCase))) ? Visibility.Visible: Visibility.Collapsed;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
