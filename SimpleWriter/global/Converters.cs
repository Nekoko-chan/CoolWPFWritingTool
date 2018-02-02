using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ComplexWriter.MessageBoxes;
using ComplexWriter.Properties;
using CustomControls;
using Writer.Data;
using Cursors = System.Windows.Input.Cursors;
using FlowDirection = System.Windows.FlowDirection;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using TextBox = System.Windows.Controls.TextBox;

namespace ComplexWriter.global
{
    public class ImageImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var elem = value as Image;
            return elem?.Source;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

      public class BoolToCornerRadius : IValueConverter
    {
          public object Convert(object value, Type type, object parameter, CultureInfo culture)
          {
              return value != null &&(bool)value ?  new CornerRadius(10,10,10,0) :new CornerRadius(10);

          }
        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

      public class MessageBoxIconConverter : IValueConverter
      {
          public object Convert(object value, Type type, object parameter, CultureInfo culture)
          {
              var elem = (MessageBoxImage?) value ?? MessageBoxImage.None;
              var imageName = string.Empty;
              switch (elem)
              {
                  case MessageBoxImage.Information: imageName = "info.png"; break;
                  case MessageBoxImage.Error: imageName = "errorNuke.png"; break;
                  case MessageBoxImage.Warning: imageName = "Warning.png"; break;
                  case MessageBoxImage.Question: imageName = "question.png"; break;
              }

              if (string.IsNullOrEmpty(imageName))
                  return null;

              try
              {
                  return Utilities.GetImageBrushFromSource(imageName);
              }
              catch
              {
                  return null;
              }
          }
          public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
          {
              throw new NotSupportedException();
          }
      }


      public class SeverityImageBrushConverter : IValueConverter
      {
          public object Convert(object value, Type type, object parameter, CultureInfo culture)
          {
              var elem = (ErrorSeverity?)value;
              var imageName = string.Empty;
              switch (elem)
              {
                  case ErrorSeverity.Information: imageName = "info.png"; break;
                  case ErrorSeverity.Error: imageName = "errorNuke.png"; break;
                  case ErrorSeverity.Warning: imageName = "Warning.png"; break;
              }

              if (string.IsNullOrEmpty(imageName))
                  return null;
              try
              {
                  return Utilities.GetImageBrushFromSource(imageName);
              }
              catch
              {
                  return null;
              }
          }
          public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
          {
              throw new NotSupportedException();
          }
      }

      public class SeverityColorBrushConverter : IValueConverter
      {
          public object Convert(object value, Type type, object parameter, CultureInfo culture)
          {
              var elem = (ErrorSeverity?)value;
              switch (elem)
              {
                  case ErrorSeverity.Information:
                      return Brushes.Black;
                  case ErrorSeverity.Error:
                      return new SolidColorBrush(DiverseUtilities.ColorFromString("#ffd31f1f"));
                  case ErrorSeverity.Warning:
                      return new SolidColorBrush(DiverseUtilities.ColorFromString("#ffff9000"));
              }
              return Brushes.Transparent;
          }
          public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
          {
              throw new NotSupportedException();
          }
      }

    public class ShortenFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var elem = (string)value;
            return string.IsNullOrEmpty(elem) ? "" : Path.GetFileNameWithoutExtension(elem);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class LanguageTextConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var elem = (string)value;
            return elem.StartsWith("de") ? "DEU" : "ENG";
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class IsNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            bool convert = value != null && !value.Equals(DependencyProperty.UnsetValue) && ((IEnumerable<ComplexStyle>)value).Any();
            return convert;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return value is bool ? ((bool)value ? "foo" : string.Empty) : null;
        }
    }

    public class DoubleNullCollapseConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return Math.Abs((double) value - 0d) > 0.001 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            Debug.WriteLine(value);
            return value;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class InvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var vis = (Visibility?)value;
            return vis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var vis = (bool?)value;
            return !vis;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

  


    public class ImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string) == null)
                return null;


            return ImageFromString(value);
        }

        public static BitmapImage ImageFromString(object value)
        {
            var uri = new Uri((string) value, UriKind.RelativeOrAbsolute);

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmp.UriSource = uri;
            bmp.EndInit();

            return bmp;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class MonthMaxConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var month = (double?)value;
            if (month > 0.0)
                return $"(Maximal gewünschte Ausgaben: {month:0.00}€)";

            return string.Empty;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ImageHeightConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = value as BitmapImage;

            if (bmp == null)
                return 10;

            return bmp.PixelHeight;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class IntVisibleConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var visibility = (int) value > 2 ? Visibility.Visible : Visibility.Collapsed;
                return visibility;
            }
            return Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolWidthConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = (bool?)value;

            if (bmp == null)
                return new GridLength(1, GridUnitType.Auto);

            return new GridLength(1, GridUnitType.Star);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = (bool?)value;

            return bmp!= null && bmp.Value ? Brushes.Gray : Brushes.Black;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolWatermarkConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = (bool?)value ?? false;

            return bmp ? "" : "Standardlimit definieren";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StringColorBrushConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = value as string;

            if (bmp == null)
                return Brushes.Black;

            return new SolidColorBrush(DiverseUtilities.ColorFromString(bmp));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

 

    public class ImageWidthConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = value as BitmapImage;

            if (bmp == null)
                return 10;

            return bmp.PixelHeight;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class MultilieHelperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? AlignmentY.Top : AlignmentY.Center;
            }

            return AlignmentY.Top;
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


  


    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            var s = values[0] as string;
            if (s != null && values[1] is bool && values[2] is bool && values[3] is bool)
                // Fall : ein string word übergeben und isfocus
            {
                if ((bool) values[2] && (bool) values[3])
                    return Visibility.Hidden;

                var hasText = s;

                var hasFocus2 = (bool) values[1];
                bool stopReadOnly = !(bool) values[3] && (bool) values[2];

                if (hasText.Length > 0 || hasFocus2 && !stopReadOnly)
                    return Visibility.Hidden;
            }

            if (values[0] is bool && values[1] is bool && values[2] is bool && values[3] is bool)
                // Fall text.isEmpty und isfocused
            {
                if ((bool) values[2] && (bool) values[3])
                    return Visibility.Hidden;

                bool hasFocus = !(bool) values[0];
                var hasFocus2 = (bool) values[1];
                bool stopReadOnly = !(bool) values[3] && (bool) values[2];

                if (hasFocus || hasFocus2 && !stopReadOnly)
                    return Visibility.Hidden;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ColorTabConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var textFile1 = values[0] as TextFile;
            var textFile2 = values[1] as TextFile;

            var ext = (bool) values[2];

            var brush1 = ext ? values[5] : values[3];
            var brush2 = ext ? values[6] : values[4];

            return textFile1 == textFile2 ? brush1 : brush2;

        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

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

    public class EqualStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var textFile1 = (string)values[0];
            var textFile2 = (string)values[1];

            return string.Equals(textFile1, textFile2);
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class MultiDebugConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class QuoteButtonBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var color1 = (Brush) values[0];
            var color2 = (Brush) values[1];
            var cont = (string) values[2];
            var testList = new[]
            {
                (string) values[3],
                (string) values[4],
                (string) values[5],
                (string) values[6]
            };

            return testList.Any(elem => elem.Equals(cont)) ? color2 : color1;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class EqualColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var color1 = (SolidColorBrush)values[0];
            var color2 = (SolidColorBrush)values[1];
            var color3 = (SolidColorBrush)values[2];

            if (color3 == null||color2==null)
                return Brushes.Transparent;

            var convert = Color.Equals(color1.Color, color2.Color) ? Brushes.DarkOrchid : Color.Equals(color1.Color, color3.Color) ? Brushes.OrangeRed:Brushes.Transparent;
            return convert;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class LeadingZeroConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var lead = (bool)values[0];
            var total = (Int16)values[1];
            const int one = 1;

            return $"{(!lead ? one.ToString(CultureInfo.InvariantCulture) : one.CalculateGoodLead(total))} / {total}";
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class FormatStringWithEqualLengthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var one = (int)values[0];
            var total = (int)values[1];

            return one.CalculateGoodLead(total);
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    /// <summary>
    /// Foreground, ActualWidth indicator, ActualHeight indicator, ActualWidth track
    /// </summary>
    public class ProgressMask : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var geo = new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new Rect(0, 0, (double)values[3], (double)values[2])),
                Brush = (Brush)values[0]
            };

            var brush = new DrawingBrush
            {
                Drawing = geo,
                Viewbox = new Rect(0, 0, (double)values[1], (double)values[2]),
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewport = new Rect(0, 0, (double)values[1], (double)values[2]),
                ViewportUnits = BrushMappingMode.Absolute
            };


            return brush;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    /// <summary>
    /// Foreground, ActualWidth indicator, ActualHeight indicator, ActualWidth track
    /// </summary>
    public class IconBackMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var val1 =values[0] as SolidColorBrush;
            var val2 = (bool) values[1];

            if(!val2 || val1 == null)
                return Brushes.Transparent;

            return val1;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    /// <summary>
    /// Foreground, ActualWidth indicator, ActualHeight indicator, ActualWidth track
    /// </summary>
    public class OpacityForFalseConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool && values[1] is double)
                return (bool)values[0] ? 1 : (double)values[1];
            return 1;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    /// <summary>
    /// Foreground, ActualWidth indicator, ActualHeight indicator, ActualWidth track
    /// </summary>
    public class OpacityForTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool && values[1] is double)
                return (bool)values[0] ? (double)values[1] : 1;
            return 1;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }



    /// <summary>
    /// Breite & Höhe
    /// </summary>
    public class IndicatorMask : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var softEdge = new LinearGradientBrush {EndPoint = new Point(1, 0.5), StartPoint = new Point(0, 0.5)};
            softEdge.GradientStops.Add(new GradientStop(Colors.Black, 0));
            softEdge.GradientStops.Add(new GradientStop(Colors.Transparent, 0.8));
            if ((double)values[0] > 10)
            {
                var geo = new GeometryDrawing
                {
                    Geometry = new RectangleGeometry(new Rect(0, 0, (double) values[0] - 19, (double) values[1])),
                    Brush = Brushes.Black
                };

                var geo2 = new GeometryDrawing
                {
                    Geometry = new RectangleGeometry(new Rect((double) values[0] - 20, 0, 20, (double) values[1])),
                    Brush = softEdge
                };

                var group = new DrawingGroup();
                group.Children.Add(geo);
                group.Children.Add(geo2);

                var br = new DrawingBrush
                {
                    Drawing = @group,
                    Viewport = new Rect(0, 0, (double) values[0], (double) values[1]),
                    ViewportUnits = BrushMappingMode.Absolute
                };

                return br;


            }
            return softEdge;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class VisibilityFromTextbox : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var box = value as TextBox;
            if (box != null)
            {
                return !string.IsNullOrEmpty(box.Text.Trim());
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class NullCollapseConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class OpacityBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool) value ? 0.3 : 1;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class DayBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                return (DateTime) value != DateTime.MinValue;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class DecisionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (values.Length >= 3)
            {
                if (values[0] is bool)
                    return (bool)values[0] ? values[1] : values[2];
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            CultureInfo culture)
        {
            return null;
        }
    }
    public class TrueCollapsedConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;

            return Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class TrueHideConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Visibility.Hidden : Visibility.Visible;

            return Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    

    public class FalseHideConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Visibility.Visible : Visibility.Hidden;

            return Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class MarginConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool) value ? new Thickness(0, 310, 0, 16) : new Thickness(0, 32, 0, 16);

            return new Thickness(0, 32, 0, 16);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FalseHideConverterHideStylePopup : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return Visibility.Hidden;

            var main = parameter as MainWindow;
            if (main != null)
            {
                    main.IsMinimized = !(bool) value; //.ShowStylePopup = main.LastShowPopupValue;
                
            }
           
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FalseCollapsedConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Visibility.Hidden : Visibility.Visible;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class BoolToVisibilityCollapseConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class BoolToVisibilityCollapseConverterRevert : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class TestConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine(value);
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

  public class WidthConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            var l = o as ListView;
            var g = l?.View as GridView;
            if (g == null)
                return 1;
            double total = 0;
            for (int i = 0; i < g.Columns.Count - 1; i++)
            {
                total += g.Columns[i].Width;
            }
            return (l.ActualWidth - total);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class HideButtonOnUnknownConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Write("Macht er aber");
            if (value is Int32)
            {
                return ((Int32)value).Equals(0) ? Visibility.Collapsed : Visibility.Visible;
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
    
    public class TextBlockTooltipConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var textBlock = value as TextBlock;
            if (textBlock == null) 
                return null;
            var block = textBlock;

            var sFt = new FormattedText(block.Text,
                CultureInfo.GetCultureInfo(Settings.Default.Language),FlowDirection.LeftToRight,
                new Typeface(block.FontFamily.ToString()),
                block.FontSize,
                Brushes.Black);
            if (sFt.Width > block.Width)
                return block.Text;
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class BorderConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var box = value as TextBox;
            if (box != null)
            {
                var block = box;
                if (string.IsNullOrEmpty(block.Text))
                    return Cursors.Arrow;
                return Cursors.IBeam;
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    

    public class VisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var textBox = value as TextBox;
            if (textBox != null)
            {
                var box = textBox;


                bool isVisible = !box.IsFocused;
                isVisible &= string.IsNullOrEmpty(box.Text);


                return isVisible ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class NegateBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

    }


    public class NegateVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class CollapseNullConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class CollapseNotNullConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value != null ? Visibility.Collapsed : Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class BoolToVisibilityRevertConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class EmptyStringTextKonverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return $"<{Resources.Unset}>";
            var s = value as string;
            if (s != null)
            {
                return string.IsNullOrEmpty(s) ? $"<{Resources.Unset}>" : s;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class CollapseEmptyConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            var s = value as string;
            if (s != null)
            {
                var visibility = string.IsNullOrEmpty(s)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                return visibility;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class CollapseOnOneConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            var s = value as int?;
            if (s != null)
            {
                return  s.Value > 1;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class HideEmptyConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            var s = value as string;
            if (s != null)
            {
                return string.IsNullOrEmpty(s)
                           ? Visibility.Hidden
                           : Visibility.Visible;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class CollapseNonEmptyConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s != null)
            {
                return string.IsNullOrEmpty(s)
                           ? Visibility.Visible
                           : Visibility.Collapsed;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class TagConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s != null)
                return s;

            return value?.ToString();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var item = (ListViewItem)value;
            var listView =
                ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            // Get the index of a ListViewItem
            if (item != null)
            {
                int? index =
                    listView?.ItemContainerGenerator.IndexFromContainer(item);

                if (index % 2 == 0)
                {
                    return Brushes.LightGray;
                }
            }
            return Brushes.White;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class MyMaskHelperConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var border = value as Border;
            if (border != null)
            {
                double width = border.ActualWidth;
                double height = border.ActualHeight;
                if (height > 0 && width > 0)
                {
                    Debug.WriteLine("{0} x {1}", width, height);
                }
                else
                    return Brushes.Black;
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class FontElementDisplayConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var elem = value as FontElement;
            if (elem == null)
                return string.Empty;

            return string.Format("{2} {1} - {0}", elem.Family.FamilyNames.First().Value, elem.Group,Resources.UsedFont);
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
            if (fam == null)
            {
                var fam2 = value as FontElement;
                if (fam2 == null) return value;
                return fam2.Family.FamilyNames.FirstOrDefault().Value;
            }

            return fam.FamilyNames.FirstOrDefault().Value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    public class DefaultExpandedConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int?)value < 5;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    //ToolTipService.IsEnabled="{Binding RelativeSource={RelativeSource Self},Path=Content, Converter=ShowCharacterTooltipMultiConverter}" 
    internal class ShowCharacterTooltipMultiConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var textBox = (TextBox)value;
                return !textBox.IsHitTestVisible;
            }
            return false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class OwnFontFamilyConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((FontElement) value)?.Family;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Utilities.PossibleFonts.FirstOrDefault(elem => elem.Family.Equals(value));
        }
    }

    public class OwnFontFamilyTextConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var fe = value as FontElement;
            if (fe != null)
            {
                return fe.Family.FamilyNames.First().Value;
            }
            var f = value as FontFamily;
            return f == null ? string.Empty : f.FamilyNames.First().Value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ColorStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var style = value as ComplexStyle;
            if (style == null)
                return null;


            var block = new TextBlock
            {
               Foreground = MainWindow.Global.Box.Foreground,
               FontFamily = MainWindow.Global.Box.FontFamily,
               FontSize = MainWindow.Global.Box.FontSize
            };

            block.Inlines.Add(new Run($"{Resources.ApplyStlye}\n     ") { FontFamily = MainWindow.Global.FindResource("defaultFont") as FontFamily, FontSize = (double)MainWindow.Global.FindResource("fontSizeMedium") });


            var run = new Run
            {
                Foreground = style.ForegroundBrush,
                Background = style.BackgroundBrush,
                FontFamily = style.FontFamily,
                FontSize = style.FontSize,
                FontStyle = style.IsItalic ? FontStyles.Italic : FontStyles.Normal,
                FontWeight = style.IsBold ? FontWeights.Bold : FontWeights.Normal,
                Text = Properties.Resources.ExampleText
            };

            block.Inlines.Add(run);

            if (style.IsStrikedOut)
                run.TextDecorations = TextDecorations.Strikethrough;
            if(style.IsUnderlined)
                run.TextDecorations = TextDecorations.Underline;


            var border = new Border
            {
                Child = block,
                Style = MainWindow.Global.FindResource("borderForTooltip") as Style
            };


            return border;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    internal class CurrentPlayerColorMultiConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val1 = (ButtonState)values[0];
                var is1 = values[1] as ImageSource;
                var is2 = values[2] as ImageSource;
                var is3 = values[3] as ImageSource;
                var is4 = values[4] as ImageSource;
                var is5 = values[5] as ImageSource;

                switch (val1)
                {
                    case ButtonState.State1: return new ImageBrush(is1) { Stretch = Stretch.Uniform };
                    case ButtonState.State2: return new ImageBrush(is2) { Stretch = Stretch.Uniform };
                    case ButtonState.State3: return new ImageBrush(is3) { Stretch = Stretch.Uniform };
                    case ButtonState.State4: return new ImageBrush(is4) { Stretch = Stretch.Uniform };
                    default: return new ImageBrush(is5) { Stretch = Stretch.Uniform };
                }

            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    internal class BoolColorMultiConverter: IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val1 = (bool)values[0];
                var is1 = values[1] as Brush;
                var is2 = values[2] as Brush;

                return val1 ? is1 : is2;

            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }


    internal class IsNewestElementMultiConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val1 = values[0] as ErrorMessageItem;
                var col = values[2] as ItemCollection;

                if (val1 == null || col == null)
                    return null;

                var newest = col.OfType<ErrorMessageItem>().OrderBy(elem => elem.OccuranceTime).LastOrDefault();
                if (newest == null)
                    return Brushes.Transparent;

                if (newest != val1)
                    return Brushes.Transparent;

                return MainWindow.Global.FindResource("CurrentItem") as Brush;

            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }


    internal class IfThenElseMultiConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values.Any(elem => elem == null || elem.Equals(DependencyProperty.UnsetValue))) return values[1];
                var val1 = (bool)values[0];
                var br1 = values[1];
                var br2 = values[2];

                return val1 ? br1 : br2;


            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    public class ActivOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null || DependencyProperty.UnsetValue.Equals(value)) return 1d;
            var b = (bool)value;

            return b ? 1d : 0.5d;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    internal class VisibleOnsameStyleOnly : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var style1 = (ComplexStyle)values[0];
                var style2 = (ComplexStyle)values[1];

                bool isEqual = ComplexStyle.Equals(style1, style2);
                return isEqual ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    internal class VisibleOnDifferentStyleOnly : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var style1 = (ComplexStyle)values[0];
                var style2 = (ComplexStyle)values[1];

                bool isEqual = ComplexStyle.Equals(style1, style2);
                return isEqual ? Visibility.Hidden : Visibility.Visible;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }


    internal class IsNewestGroupConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var str1 = values[0] as string;
                var str2 = values[1] as string;

                return string.Equals(str1,str2) ? Visibility.Visible : Visibility.Hidden;

            }
            catch (Exception)
            {
                return Visibility.Hidden;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }


    public class MinMaxFontConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doub = (double?)value;

            return doub != null ? Math.Min(Math.Max(doub.Value, 12), 24) : 0;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    internal class HasItemsConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values.Any(elem => elem == null || elem.Equals(DependencyProperty.UnsetValue))) return null;
                var val1 = (int)values[0];
                var val2 = (ErrorWindow2)values[1];

                return val1 > 0 || val2 != null;
            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }


    internal class DeletionToolTipConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values[0].Equals(DependencyProperty.UnsetValue) || values[1].Equals(DependencyProperty.UnsetValue))
                    return string.Empty;
                var val1 = (bool)values[0];
                var val2 = (string)values[1];

                return string.Format(val1 ? Resources.RevertDelete : Resources.DeleteSpezial, val2);

            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    internal class ItemsControlWidthConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values[0].Equals(DependencyProperty.UnsetValue) || values[1].Equals(DependencyProperty.UnsetValue))
                    return 0d;

                var val1 = (Visibility)values[0];
                var val2 = (double) values[1];


                double convert = Math.Max(1d,val2 - (val1 == Visibility.Visible ? 15:0));
                return convert;
            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }


       

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }


    public class FontWeightConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(FontWeights.Bold);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            return val != null && val.Value ? FontWeights.Bold : FontWeights.Normal;
        }

    }

    public class FontStyleConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(FontStyles.Italic);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            return val != null && val.Value ? FontStyles.Italic : FontStyles.Normal;
        }

    }

    public class FontStretchConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(FontStretches.Condensed);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            return val != null && val.Value ? FontStretches.Condensed : FontStretches.Normal;
        }

    }


    public class UnderlineConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value!= null && value.Equals(TextDecorations.Underline);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            return val != null && val.Value ? TextDecorations.Underline : null;
        }

    }
    public class StrikeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(TextDecorations.Strikethrough);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            return val != null && val.Value ? TextDecorations.Strikethrough : null;
        }

    }

    public class BoldAllowedConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ff = value as FontElement;
            return ff != null && ff.Family.FamilyTypefaces.Any(elem => elem.Weight == FontWeights.Bold);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
    public class ItalicAllowedConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ff = value as FontElement;
            return ff != null && ff.Family.FamilyTypefaces.Any(elem => elem.Style == FontStyles.Italic);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
    public class CondensedAllowedConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ff = value as FontElement;
            return ff != null && ff.Family.FamilyTypefaces.Any(elem => elem.Stretch == FontStretches.Condensed);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }



    public class CharacterTypeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (NameType?)value;


            return new EnumDescriptionTypeConverter(typeof(NameType)).ConvertTo(null, CultureInfo.CurrentCulture, str,
                typeof(string));

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }


    internal class ShowStylesMultiConverter : IMultiValueConverter
    {
        #region Implementation for the interface IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values.Length<4 || values.Any(elem => elem == null || elem.Equals(DependencyProperty.UnsetValue))) return false;

                return (bool) values[0] && (bool) values[1] &&!(bool)values[2] && !((WindowState)values[3]).Equals(WindowState.Minimized);
            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }


    public class FlipConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var bol = (bool?)value;

            return new ScaleTransform((bol ?? false) ? -1d : 1, 1d);

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class PathToNameConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var str = (string)value;

            return Path.GetFileName(str);

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string)) return null;
            if ((value as string).Contains(@"\"))
            {
                Uri uri = new Uri("pack://application:,,,/Images/diskdrive.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = new Uri("pack://application:,,,/Images/open.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }




    public enum ErrorSeverity { None,Error,Information,Warning}


#region rechtangle
    public class RectangleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var border = value as Border;
            if (border != null)
            {
                double width = border.ActualWidth;
                double height = border.ActualHeight;
                return new Rect(0, 0, Math.Truncate(width), Math.Truncate(height));
            }

            return new Rect(0, 0, 1, 1);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class TopRectangleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var border = value as Border;
            if (border != null)
            {
                double width = border.ActualWidth;
                double height = border.ActualHeight;
                return new Rect(0, 0, Math.Truncate(width) , Math.Truncate(height)- 19);
            }

            return new Rect(0, 0, 1, 1);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BottomRectangleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var border = value as Border;
            if (border != null)
            {
                double width = border.ActualWidth;
                double height = border.ActualHeight;

                return new Rect(0, Math.Truncate(height) - 20, Math.Truncate(width), 20);
            }


            return new Rect(0, 0, 1, 1);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BottomRectangleConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var border = value as Border;
            if (border != null)
            {
                double height = border.ActualHeight;

                return new Rect(0, Math.Truncate(height) - 20, 21, 20);
            }


            return new Rect(0, 0, 1, 1);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StopwatchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var span = (TimeSpan?)value;
            return span?.TotalSeconds;
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class IsEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class IsNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class VisibilityBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility?) value == Visibility.Collapsed
                ? new CornerRadius(0, 5, 5, 0)
                : new CornerRadius(0, 0, 5, 0);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class MaxHeightConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return SystemInformation.VirtualScreen.Height - 50;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class CollapseOnEmptyCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var col = value as IList;
            return (col == null || col.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class CollapseZeroConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var col = value as int?;
            return (col == null || col.Value == 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class EntryTextConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var col = value as int?;

            if (col == null || col.Value == 0) return string.Empty;

            var text = col.Value == 1 ? Resources.Entry : Resources.Entries;
            return $"({col.Value} {text})";
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class FolderStyleConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            var val = (bool)o;
            return val ? 17d : 12d;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            var val = (bool)o;
            return val ? Brushes.Transparent : Brushes.OrangeRed;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class PercentTextConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            var val = (int)o;
            return val < 0 ? string.Empty : $"{val}%";
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }




    #endregion

}
