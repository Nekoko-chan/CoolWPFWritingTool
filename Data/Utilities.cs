using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtensionObjects;
using Color = System.Windows.Media.Color;

namespace Writer.Data
{
    public static class Utilities
    {
        private static ObservableCollection<FontElement> _possibleFonts;

        public static int IndexOfWhitespaceOrNewline(this string str, int startindex)
        {
            var ndxSpace = str.IndexOf(" ", startindex, StringComparison.Ordinal);
            var ndxNewLine = str.IndexOf(Environment.NewLine, startindex, StringComparison.Ordinal);

            if (ndxSpace == -1)
                return ndxNewLine;
            if (ndxNewLine == -1)
                return ndxSpace;

            return Math.Min(ndxNewLine, ndxSpace);
        }

        public static bool HasFolderWritePermission(string destDir)
        {
            try
            {
                // Attempt to get a list of security permissions from the file. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                Directory.GetDirectories(destDir);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static ObservableCollection<FontElement> PossibleFonts
        {
            get
            {
                if(_possibleFonts != null)
                    return _possibleFonts;

                _possibleFonts = InitFonts();
                return _possibleFonts;
            }
        }

        public static bool IsAllowedText(string text)
        {
            var regex = new Regex("[^0-9,]"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }


        public static void SetThickness(ref FlowDocument flowDocument, Thickness s)
        {
            foreach (var block in flowDocument.Blocks.OfType<Paragraph>())
            {
                block.Margin = new Thickness(0);
            }

            foreach (var block in flowDocument.Blocks.OfType<List>())
            {
                block.Margin = new Thickness(0);
            }
        }

     public static void AddRange(this ItemCollection collection, IEnumerable<object> entries)
        {
            foreach (var entry in entries)
            {
                collection.Add(entry);
            }
        }


        public static int UpperCaseCount(this string original)
        {
            MatchCollection a = Regex.Matches(original, "*[A-ZÄÖÜ]*[a-zäöü]");
            return a.Count; // the Count will return the number of upper cased characters
        }

        public static bool IsCorrectLettering(this string original)
        {
            Match lettering;
            lettering = Regex.Match(original, "[0-9]*"); // Wenn Zahlen vorkomme, alles ignorieren
            if (lettering.Success)
                return true;
            lettering = Regex.Match(original, "[^0-9a-zA-Z *]"); // Wenn Zeichen auftreten, die keine Buchstaben sind, dann ist auch alles egal
            if (lettering.Success)
                return true;
            lettering = Regex.Match(original, "([A-ZÄÖÜ]{0}([a-zäöü]*))");
            if (lettering.Value.Equals(original))
                return true;
            lettering = Regex.Match(original, "([A-ZÄÖÜ]*)");
            if (lettering.Value.Equals(original))
                return true;

            lettering = Regex.Match(original, "([A-ZÄÖÜ]{1}([a-zäöü]*))");
            if (lettering.Value.Equals(original))
                return true;
            lettering = Regex.Match(original, "([A-ZÄÖÜ]{3,}([a-zäöü]*))");
            return lettering.Value.Equals(original);
        }

       
        private static ObservableCollection<FontElement> InitFonts()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if(String.IsNullOrEmpty(path))
                return new ObservableCollection<FontElement>();

            var combine = Path.Combine(path,"Fonts");
            var dir = Directory.GetDirectories(combine);
            var observableCollection = new ObservableCollection<FontElement>();
            foreach (var s in dir)
            {
                var fonts = LoadFonts(combine,string.Format("{0}\\",Path.GetFileName(s)));
                observableCollection.AddRange(fonts);

            }
            var installedFonts = GetInstalledFonts();
            observableCollection.AddRange(installedFonts);

            return observableCollection;
        }

        private static List<FontElement> GetInstalledFonts()
        {
            return Fonts.SystemFontFamilies.Select(elem => new FontElement {Family =elem ,Group = "Installierte Schriftarten",Name = elem.FamilyNames.FirstOrDefault().Value}).DistinctBy(elem=>elem.Name).ToList();
        }



        private static List<FontElement> LoadFonts(string path,string title)
        {
            var fonts = Fonts.GetFontFamilies(Path.Combine(path,title));

            var info = Path.Combine(Path.Combine(path, title.Replace("\\", "")),"info.txt");
            if (!File.Exists(info)) return fonts.Select(elem => new FontElement { Family = elem, Group = title.Replace("\\", "") }).ToList();

            var text = File.ReadAllText(info);
            return fonts.Select(elem => new FontElement { Family = elem, Group = text.Trim() }).ToList();
        }


        public static object GetImageBrushFromSource(string imageName)
        {
            var name = String.Format("pack://application:,,,/Images/{0}", imageName);


            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(name, UriKind.RelativeOrAbsolute);
            logo.EndInit();

            return new ImageBrush(logo) { Stretch = Stretch.Uniform };
        }


        public static ComplexStyle ComplexStyleFromXmlString(this string input)
        {
            using (var reader = new StringReader(input))
            {
                return input.FromReader<ComplexStyle>(reader);
            }
        }

        public static ComplexStyles ComplexStylesFromXmlString(this string input)
        {
            using (var reader = new StringReader(input))
            {
                return input.FromReader<ComplexStyles>(reader);
            }
        }

        public static string ToHexColor(this Color color, bool alphaChannel=true)
        {
            return String.Format("#{0}{1}{2}{3}",
                                 alphaChannel ? color.A.ToString("X2") : String.Empty,
                                 color.R.ToString("X2"),
                                 color.G.ToString("X2"),
                                 color.B.ToString("X2"));
        }

        /// <summary>
        /// Ersetzt die Vorkommen von Wert 1 in Wert 2, funtioniert nur, wenn ein gutes Equal existiert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colors"></param>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        public static ObservableCollection<Color> Replace(this ObservableCollection<Color> colors, Color color1,
           Color color2)
        {
            var ret = new ObservableCollection<Color>();
            foreach (var color in colors)
            {
                var hexColor = color.ToHexColor();
                var value = color1.ToHexColor();
                ret.Add(hexColor.Equals(value) ? color2 : color);
            }
            return ret;
        }

        public static bool IsEmpty(this Paragraph para)
        {
            var range = new TextRange(para.ContentStart, para.ContentEnd);

            return range.IsEmpty || String.IsNullOrEmpty(range.Text);
        }


        public static string CalculateGoodLead(this int value, int reference)
        {
            if (reference < 10)
                return value.ToString(CultureInfo.InvariantCulture);

            if (reference < 100)
                return value.ToString("00");

            if (reference < 1000)
                return value.ToString("000");

            if (reference < 10000)
                return value.ToString("0000");
            if (reference < 100000)
                return value.ToString("00000");

            if (reference < 1000000)
                return value.ToString("000000");

            if (reference < 10000000)
                return value.ToString("0000000");

            return reference < 100000000 ? value.ToString("00000000") : value.ToString(CultureInfo.InvariantCulture);
        }

        public static byte[] ConvertImageToByteArray(ImageSource source)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)source));
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            catch
            {
                return new byte[0];
            }


        }

        public static BitmapImage GetImageSourceFromByteArray(byte[] arr)
        {
            using (var stream = new MemoryStream(arr))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }


        public static string GetOpeningQuotation(ButtonState state)
        {

            switch (state)
            {
                case ButtonState.State1:
                    return "\u00BB"; // frensch
                case ButtonState.State2:
                    return "\u201E"; // german
                case ButtonState.State3:
                    return "\u201C"; // englisch
                case ButtonState.State5:
                    return "\u300C";
                default:
                    return "\u0022"; // normaler Text
            }
        }

        public static string GetClosingQuotation(ButtonState state)
        {

            switch (state)
            {
                case ButtonState.State1:
                    return "\u00AB"; // frensch
                case ButtonState.State2:
                    return "\u201F"; // german
                case ButtonState.State3:
                    return "\u201D"; // englisch
                case ButtonState.State5:
                    return "\u300D";
                default:
                    return "\u0022"; // normaler Text
            }
        }

        public static string GetSingleOpeningQuotation(ButtonState state)
        {

            switch (state)
            {
                case ButtonState.State1:
                    return "\u203A"; // frensch
                case ButtonState.State2:
                    return "\u201A"; // german
                case ButtonState.State3:
                case ButtonState.State5:
                    return "\u2018";
                default:
                    return "\u0027"; // normaler Text
            }
        }


        public static string GetSingleClosingQuotation(ButtonState state)
        {

            switch (state)
            {
                case ButtonState.State1:
                    return "\u2039"; // frensch
                case ButtonState.State2:
                    return "\u2018"; // german
                case ButtonState.State3:
                case ButtonState.State5:
                    return "\u2019";
                default:
                    return "\u0027"; // normaler Text
            }
        }

      



        public static bool ParagraphIsPageBreaker(Paragraph para)
        {
            return (para.Background as ImageBrush) != null;
        }
    }


}
