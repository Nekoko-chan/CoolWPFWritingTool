using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace ComplexWriter
{
    public static class FlowDocumentExtensions
    {
        #region FontReplacing

        public static bool CheckAndChangeFonts(this FlowDocument document, ComplexStyle oldStyle, ComplexStyle newStyle)
        {
            var update = false;
            foreach (var block in document.Blocks.ToList())
            {
                update |= block.ChangeMainFont(oldStyle,newStyle);
                update |= block.CheckAndChangeFonts(oldStyle, newStyle);
            }
            return update;
        }


        public static bool CheckAndChangeFonts(this Block block, ComplexStyle oldStyle, ComplexStyle newStyle)
        {
            var section = block as Section;
            if (section != null)
            {
                return UpdateFonts(section, oldStyle, newStyle);
            }

            var paragraph = block as Paragraph;
            if (paragraph != null)
            {

                return UpdateFonts(paragraph, oldStyle, newStyle);
            }

            var list = block as List;
            return list != null && UpdateFonts(list, oldStyle, newStyle);
        }

        public static bool CheckAndChangeFonts(this ListItem listItem, ComplexStyle oldStyle, ComplexStyle newStyle)
        {
            return listItem.Blocks.ToList().Aggregate(false, (current, block) => current | block.CheckAndChangeFonts(oldStyle, newStyle));
        }

        private static bool UpdateFonts(Section section, ComplexStyle oldStyle, ComplexStyle newStyle)
        {
            var updated = false;
            foreach (var block in section.Blocks.ToList())
            {
                updated |= block.ChangeMainFont(oldStyle, newStyle);
                updated |= block.CheckAndChangeFonts(oldStyle, newStyle);
            }
            return updated;
        }

        private static bool UpdateFonts(Paragraph paragraph, ComplexStyle oldStyle, ComplexStyle newStyle)
        {
            return paragraph.Inlines.ToList().Aggregate(false, (current, inline) => current | inline.ChangeMainFont(oldStyle, newStyle));
        }

        private static bool UpdateFonts(List list, ComplexStyle oldStyle, ComplexStyle newStyle)
        {
            return list.ListItems.ToList().Aggregate(false, (current, listItem) => current | listItem.CheckAndChangeFonts(oldStyle, newStyle));
        }


        public static bool ChangeMainFont(this TextElement elem, ComplexStyle oldStyle, ComplexStyle newStyle)
        {
            try
            {
                if (!elem.FontFamily.Equals(oldStyle.FontFamily))
                    return true;
                if (!oldStyle.FontSize.Equals(elem.FontSize))
                    return true;
                if (!oldStyle.Style.Equals(elem.FontStyle))
                    return true;
                if (!oldStyle.Weight.Equals(elem.FontWeight))
                    return true;
                var range = new TextRange(elem.ContentStart, elem.ContentEnd);

                Brush brush;
                try
                {
                    brush= (SolidColorBrush)range.GetPropertyValue(FlowDocument.ForegroundProperty);
                    if (brush != null && !oldStyle.ForegroundBrush.ToString().Equals(brush.ToString()))
                        return true;
                }
                catch (Exception)
                {
                    return true;
                }

                try
                {
                    brush = (SolidColorBrush)range.GetPropertyValue(FlowDocument.BackgroundProperty);
                    if (brush!= null &&!oldStyle.BackgroundBrush.ToString().Equals(brush.ToString()))
                        return true;
                }
                catch (Exception)
                {
                    return true;
                }

                var dec = (TextDecorationCollection) range.GetPropertyValue(Inline.TextDecorationsProperty);
                if (!AreEqualTextDecorations(dec,oldStyle.Decoration))
                    return true;

                range.ApplyPropertyValue(TextElement.FontFamilyProperty,newStyle.FontFamily);
                range.ApplyPropertyValue(TextElement.FontSizeProperty, newStyle.FontSize);
                range.ApplyPropertyValue(TextElement.FontWeightProperty, newStyle.Weight);
                range.ApplyPropertyValue(TextElement.FontStyleProperty, newStyle.Style);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, newStyle.ForegroundBrush);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, newStyle.BackgroundBrush);
                range.ApplyPropertyValue(Inline.TextDecorationsProperty, newStyle.Decoration);
                range.ApplyPropertyValue(TextElement.FontStretchProperty,newStyle.Stretch);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool AreEqualTextDecorations(TextDecorationCollection a, TextDecorationCollection b)
        {
            if (a == null && b == null)
                return true;
            if (a == null && !b.Any())
                return true;

            if (b == null && !a.Any())
                return true;

            if (a != null && b != null)
                return Equals(a.FirstOrDefault(), b.FirstOrDefault());
            return false;
        }

        #endregion


        #region FontReplacing

        public static bool CheckAndChangeMyFonts(this FlowDocument document, ObservableCollection<FontFamily> possibleFonts)
        {
            var update = false;
            foreach (var block in document.Blocks.ToList())
            {
                update |= block.ChangeMyMainFont(possibleFonts);
                update |= block.CheckAndChangeMyFonts(possibleFonts);
            }
            return update;
        }


        public static bool CheckAndChangeMyFonts(this Block block, ObservableCollection<FontFamily> possibleFonts)
        {
            var section = block as Section;
            if (section != null)
            {
                return UpdateMyFonts(section, possibleFonts);
            }

            var paragraph = block as Paragraph;
            if (paragraph != null)
            {

                return UpdateMyFonts(paragraph, possibleFonts);
            }

            var list = block as List;
            return list != null && UpdateFonts(list, possibleFonts);
        }

        public static bool CheckAndChangeMyFonts(this ListItem listItem, ObservableCollection<FontFamily> possibleFonts)
        {
            return listItem.Blocks.ToList().Aggregate(false, (current, block) => current | block.CheckAndChangeMyFonts(possibleFonts));
        }

        private static bool UpdateMyFonts(Section section, ObservableCollection<FontFamily> possibleFonts)
        {
            var updated = false;
            foreach (var block in section.Blocks.ToList())
            {
                updated |= block.ChangeMyMainFont(possibleFonts);
                updated |= block.CheckAndChangeMyFonts(possibleFonts);
            }
            return updated;
        }

        private static bool UpdateMyFonts(Paragraph paragraph, ObservableCollection<FontFamily> possibleFonts)
        {
            return paragraph.Inlines.ToList().Aggregate(false, (current, inline) => current | inline.ChangeMyMainFont(possibleFonts));
        }

        private static bool UpdateFonts(List list, ObservableCollection<FontFamily> possibleFonts)
        {
            return list.ListItems.ToList().Aggregate(false, (current, listItem) => current | listItem.CheckAndChangeMyFonts(possibleFonts));
        }

        public static bool ChangeMyMainFont(this TextElement elem, ObservableCollection<FontFamily> possibleFonts)
        {
            if (!elem.FontFamily.Source.StartsWith("file:///"))
                return false;

            var fontFamily = elem.FontFamily.GetFontFamily(possibleFonts);
            if (elem.FontFamily.Equals(fontFamily)) return false;

            elem.FontFamily = fontFamily;
            return true;
        }

        public static FontFamily GetFontFamily(this FontFamily fontFamily, ObservableCollection<FontFamily> possibleFonts)
        {
            var fontname =
                  fontFamily.Source.Substring(fontFamily.Source.LastIndexOf("/#", StringComparison.OrdinalIgnoreCase));


            FontFamily newFont = possibleFonts.FirstOrDefault(elem => elem.Source.EndsWith(fontname));

            return newFont ?? fontFamily;
        }

        #endregion

        public static List<ComplexStyle> GetStyles(this FlowDocument document, double defaultSize)
        {
            var ret = new List<ComplexStyle>();
            document.CollectStyles(ref ret,defaultSize);
            return ret;
        }

        public static void CollectStyles(this FlowDocument document, ref List<ComplexStyle> styles, double defaultSize)
        {
            foreach (var block in document.Blocks.ToList())
            {
                block.CollectStyles(ref styles, defaultSize);
            }
        }

        public static void CollectStyles(this Section section, ref List<ComplexStyle> styles, double defaultSize)
        {
            foreach (var block in section.Blocks.ToList())
            {
                block.CollectStyles(ref styles, defaultSize);
            }
        }

        public static void CollectStyles(this Paragraph paragraph, ref List<ComplexStyle> styles, double defaultSize)
        {
            foreach (var inline in paragraph.Inlines.ToList())
            {
                inline.CollectStyles(ref styles, defaultSize);
            }
        }

        public static void CollectStyles(this List list, ref List<ComplexStyle> styles, double defaultSize)
        {
            foreach (var listItem in list.ListItems.ToList())
            {
                listItem.CollectStyles(ref styles, defaultSize);
            }
        }

        public static void CollectStyles(this ListItem item, ref List<ComplexStyle> styles, double defaultSize)
        {
            foreach (var block in item.Blocks.ToList())
            {
                block.CollectStyles(ref styles, defaultSize);
            }
        }

        public static void CollectStyles(this TextElement text, ref List<ComplexStyle> styles, double defaultSize)
        {
            var run = text as Run;
            if(run!= null &&string.IsNullOrEmpty(run.Text)) return;
            var range = new TextRange(text.ContentStart, text.ContentEnd);
            var style = GetCurrentStyle(range, defaultSize);
            if(!styles.Any(elem=>ComplexStyle.Equals(elem,style)))
                styles.Add(style);
        }

        public static void CollectStyles(this Block block, ref List<ComplexStyle> style, double defaultSize)
        {
            var section = block as Section;
            if (section != null)
            {
                section.CollectStyles(ref style, defaultSize);
                return;
            }

            var paragraph = block as Paragraph;
            if (paragraph != null)
            {
                paragraph.CollectStyles(ref style, defaultSize);
                return;
            }

            var list = block as List;
            if(list != null)
                list.CollectStyles(ref style, defaultSize);
        }


        public static ComplexStyle GetCurrentStyle(TextRange range, double defaultsize)
        {
            var test = range.GetPropertyValue(FlowDocument.FontSizeProperty);

            var applyableStyle = new ComplexStyle();
            applyableStyle.FontFamily = range.GetPropertyValue(Control.FontFamilyProperty) as FontFamily;
            applyableStyle.FontSize = test != DependencyProperty.UnsetValue ? (double)test : defaultsize;
            applyableStyle.IsBold =  range.GetPropertyValue(Control.FontWeightProperty).Equals(FontWeights.Bold);
            applyableStyle.IsItalic = range.GetPropertyValue(Control.FontStyleProperty).Equals(FontStyles.Italic);
            applyableStyle.IsCondenced = range.GetPropertyValue(Control.FontStretchProperty).Equals(FontStretches.Condensed);
            applyableStyle.IsUnderlined = Equals(range.GetPropertyValue(Inline.TextDecorationsProperty),TextDecorations.Underline);
            applyableStyle.IsStrikedOut = Equals(range.GetPropertyValue(Inline.TextDecorationsProperty),TextDecorations.Strikethrough);
            applyableStyle.BackgroundBrush = range.GetPropertyValue(FlowDocument.BackgroundProperty) as SolidColorBrush;
            applyableStyle.ForegroundBrush = range.GetPropertyValue(FlowDocument.ForegroundProperty) as SolidColorBrush;
            return applyableStyle;
        }


        public static int CountStyle(this FlowDocument doc, ComplexStyle style, double defaultSize, ref TextPointer position, bool positionOnly)
        {
            var counter = 0;
            foreach (var block in doc.Blocks.ToList())
            {
                counter+=block.CountStyle(style, defaultSize, ref position, positionOnly);
            }
            return counter;
        }

        public static int CountStyle(this Section section, ComplexStyle style, double defaultSize, ref TextPointer position, bool positionOnly)
        {
            var counter = 0;
            foreach (var block in section.Blocks.ToList())
            {
                counter+=block.CountStyle(style, defaultSize, ref position, positionOnly);
            }
            return counter;
        }

        public static int CountStyle(this Paragraph paragraph, ComplexStyle style, double defaultSize, ref TextPointer position, bool positionOnly)
        {
            if (position != null) return 0;
            var count = 0;
            foreach (Inline inline in paragraph.Inlines)
            {
                TextPointer pos = null;
                if (!inline.IsStyle(style, defaultSize, out pos)) continue;

                if (position == null)
                    position = pos;
                count++;
            }
            return count;
        }

        public static int CountStyle(this List list, ComplexStyle style, double defaultSize, ref TextPointer position, bool positionOnly)
        {
            if (position != null) return 0;
            var counter = 0;
            foreach (var listItem in list.ListItems.ToList())
            {
                counter+=listItem.CountStyle(style, defaultSize, ref position, positionOnly);
            }
            return counter;
        }

        public static int CountStyle(this ListItem item, ComplexStyle style, double defaultSize, ref TextPointer position, bool positionOnly)
        {
            var counter = 0;
            if (position != null) return 0;
            foreach (var block in item.Blocks.ToList())
            {
                counter+=block.CountStyle(style, defaultSize, ref position, positionOnly);
            }
            return counter;
        }

        public static int CountStyle(this Block block, ComplexStyle style, double defaultSize, ref TextPointer position, bool positionOnly)
        {
            if (position != null) return 0;
            var section = block as Section;
            if (section != null)
            {
                return section.CountStyle(style, defaultSize, ref position,positionOnly);
            }

            var paragraph = block as Paragraph;
            if (paragraph != null)
            {
                return paragraph.CountStyle(style, defaultSize, ref position, positionOnly);
            }

            var list = block as List;
            if (list != null)
                return list.CountStyle(style, defaultSize, ref position, positionOnly);
            return 0;
        }

        public static bool IsStyle(this  TextElement text, ComplexStyle style,double defaultSize, out TextPointer position)
        {
            position = null;
            var run = text as Run;
            if (run != null && string.IsNullOrEmpty(run.Text)) return false;
            var range = new TextRange(text.ContentStart, text.ContentEnd);
            var sty = GetCurrentStyle(range, defaultSize);
            position = text.ContentStart;
            return ComplexStyle.Equals(sty, style);
        }


        /// <summary>
        /// Kopiert ein FlowDocument und passt die 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static FlowDocument CopyWithTickness(this FlowDocument from, Thickness thickness)
        {
            var text = CopyIntoByteArray(@from);

            var flowDocument = GetDocumentFromByreArray(text);
            if (flowDocument == null)
                return null;

            foreach (var block in flowDocument.Blocks.OfType<Paragraph>())
            {
                block.Margin = new Thickness(block.Margin.Left, thickness.Top, block.Margin.Right, thickness.Bottom);
            }

            foreach (var block in flowDocument.Blocks.OfType<List>())
            {
                block.Margin = new Thickness(block.Margin.Left, thickness.Top, block.Margin.Right, thickness.Bottom);
            }
            return flowDocument;
        }

        private static byte[] CopyIntoByteArray(FlowDocument document)
        {
            using (var fs = new MemoryStream())
            {
                var content = new TextRange(document.ContentStart, document.ContentEnd);
                content.Save(fs, DataFormats.XamlPackage, true);
                return fs.ToArray();
            }
        }

        private static FlowDocument GetDocumentFromByreArray(byte[] array)
        {
            var document = new FlowDocument();
            using (var fs = new MemoryStream(array))
            {
                var content = new TextRange(document.ContentStart, document.ContentEnd);
                content.Load(fs, DataFormats.XamlPackage);
            }

            return document;
        }


        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
  this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

    }

}
