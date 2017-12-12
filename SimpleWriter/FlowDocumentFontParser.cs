using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ComplexWriter
{
    public static class FlowDocumentFontParserExtension
    {
        private static string _fontPath;

        public static bool CheckAndChangeFonts(this FlowDocument document)
        {
            var update = false;
            foreach (var block in document.Blocks)
            {
                update|= block.ChangeMainFont();
                update|= block.CheckAndChangeFonts();
            }
            return update;
        }


        public static bool CheckAndChangeFonts(this Block block)
        {
            var section = block as Section;
            if (section != null)
            {
                return UpdateFonts(section);;
            }

            var paragraph = block as Paragraph;
            if (paragraph != null)
            {
                
                return UpdateFonts(paragraph);
            }

            var list = block as List;
            return list != null && UpdateFonts(list);
        }

        public static bool CheckAndChangeFonts(this ListItem listItem)
        {
            return listItem.Blocks.Aggregate(false, (current, block) => (bool) (current | block.CheckAndChangeFonts()));
        }

        private static bool UpdateFonts(Section section)
        {
            var updated = false;
            foreach (var block in section.Blocks)
            {
                updated |= block.ChangeMainFont();
                updated |= block.CheckAndChangeFonts();
            }
            return updated;
        }

        private static bool UpdateFonts(Paragraph paragraph)
        {
            return paragraph.Inlines.Aggregate(false, (current, inline) => current | inline.ChangeMainFont());
        }

        private static bool UpdateFonts(List list)
        {
            return list.ListItems.Aggregate(false, (current, listItem) => (bool) (current | listItem.CheckAndChangeFonts()));
        }

        private const string File = "file:///{0}";
        private const string File2 = "file:///{0}{1}";

        public static bool ChangeMainFont(this TextElement elem)
        {
            if (!elem.FontFamily.Source.StartsWith("file:///"))
                return false;

            string format = string.Format(File,FontPath.Replace("\\","/"));
            if (elem.FontFamily.Source.StartsWith(format))
                return false;
            
            

            elem.FontFamily = GetFontFamily(elem.FontFamily);
            return true;
        }

        private static FontFamily GetFontFamily(FontFamily fontFamily)
        {
          var fontname =
                fontFamily.Source.Substring(fontFamily.Source.LastIndexOf("/#", StringComparison.OrdinalIgnoreCase));

            var newFontName = string.Format(File2, FontPath.Replace("\\", "/"),fontname);

            var newFont = Utilities.PossibleFonts.FirstOrDefault(elem => elem.Source.Equals(newFontName));

            return newFont ?? Utilities.PossibleFonts.FirstOrDefault();
        }

        public static string FontPath
        {
            get
            {
                if (string.IsNullOrEmpty(_fontPath))
                {
                    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    if (string.IsNullOrEmpty(path))
                        return string.Empty;
                    _fontPath = Path.Combine(path, "PossibleFonts");
                }
                return _fontPath;
            }
        }

        
    }
   
}
