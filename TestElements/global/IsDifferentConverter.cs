using System;
using System.Globalization;
using System.Windows.Data;

namespace TestElements.global
{
    public class IsDifferentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var textFile1 = (double)values[0];
            var textFile2 = (double)values[1];

            bool convert = !textFile1.Equals(textFile2);
            return convert;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
