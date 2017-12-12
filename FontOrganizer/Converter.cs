using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FontOrganizer
{
    public class FontFamilySourceConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fam = value as FontFamily;
            return fam == null ? string.Empty : fam.Source;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class DirectoryDisplayConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fam = value as string;

            if (fam == null) return string.Empty;

            if (File.Exists(Path.Combine(fam, "info.txt")))
            {
                return File.ReadAllText(Path.Combine(fam, "info.txt")).Trim();
            }

            return Path.GetFileName(fam);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class FontElementePathConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fam = value as FontElement;
            return fam == null ? Enumerable.Empty<string>() : fam.Pathes;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class IsLargerZero : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fam = value as int?;
            return fam != null && fam.Value > 0;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class FontFamilyDisplayConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fam = value as FontFamily;
            return fam == null ? string.Empty : fam.FamilyNames.FirstOrDefault().Value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class FontFamilyConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fam = value as FontElement;
            return fam == null ? new FontFamily("Arial") : fam.Family;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
