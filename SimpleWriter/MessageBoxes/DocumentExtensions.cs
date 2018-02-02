using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using MessageBox = ComplexWriter.MessageBoxes.MessageBox;

namespace ComplexWriter
{
    public static class DocumentExtensions
    {
        public const string REPLACER = "PAGEBREAK_MARTINAGRAEBER_PAGEBREAK";
        public static FlowDocument AddFlowDocument(this FlowDocument d1, FlowDocument d2, string title,bool breakPage, double fontsize, double titleFontsize, bool underlined, bool bold)
        {
            var range = new TextRange(d2.ContentStart,d2.ContentEnd);
            range.ApplyPropertyValue(FlowDocument.FontSizeProperty, fontsize);
            range.ApplyPropertyValue(FlowDocument.FontFamilyProperty, new FontFamily("Amazon Ember"));
            range.ApplyPropertyValue(FlowDocument.ForegroundProperty, Brushes.Black);
            range.ApplyPropertyValue(FlowDocument.LineHeightProperty, Double.NaN);
            
            var ret = d1;

            if(breakPage)
            {
                var para = new Paragraph(new Run(REPLACER));
                para.Inlines.Add(new Run(title));
                ret.Blocks.Add(para);
            }
            else
                ret.Blocks.Add(new Paragraph(new Run(title) {FontSize =titleFontsize, FontFamily = new FontFamily("Amazon Ember"), FontWeight = bold?FontWeights.Bold:FontWeights.Normal, TextDecorations = underlined?TextDecorations.Underline: null}));

            foreach (Block block in d2.Blocks.ToList())
            {
                if(block.BreakPageBefore)
                    ret.Blocks.Add(new Paragraph(new Run(REPLACER)));
                else
                    ret.Blocks.Add(block);
            }
            return d1;
        }

        public static FlowDocument PrepareForRtfExport(this FlowDocument document, string title)
        {
            var range = new TextRange(document.ContentStart, document.ContentEnd);
            range.ApplyPropertyValue(FlowDocument.FontSizeProperty, 15.25d);
            range.ApplyPropertyValue(FlowDocument.FontFamilyProperty, new FontFamily("Amazon Ember"));
            range.ApplyPropertyValue(FlowDocument.ForegroundProperty, Brushes.Black);
            range.ApplyPropertyValue(FlowDocument.LineHeightProperty,Double.NaN);

            var ret = new FlowDocument();

            if(!string.IsNullOrEmpty(title))
                ret.Blocks.Add(new Paragraph(new Run(title) {FontFamily = new FontFamily("Amazon Ember"), FontSize = 24.25d, FontWeight = FontWeights.Bold, TextDecorations = TextDecorations.Underline}));

            foreach (Block block in document.Blocks.ToList())
            {
                if (block.BreakPageBefore)
                    ret.Blocks.Add(new Paragraph(new Run(REPLACER)));
                else
                    ret.Blocks.Add(block);
            }

           

            return ret;
        }

        public static void SaveAsRtfFile(this FlowDocument document, string filename)
        {
            var test = ConvertXamlToRtf(XamlWriter.Save(document));
            File.WriteAllText(filename, test.Replace(REPLACER, "\\page"));
        }

        private static string ConvertXamlToRtf(string xamlContent)
        {
            var assembly = Assembly.GetAssembly(typeof(FrameworkElement));
            var xamlRtfConverterType = assembly.GetType("System.Windows.Documents.XamlRtfConverter");
            var xamlRtfConverter = Activator.CreateInstance(xamlRtfConverterType, true);
            var convertXamlToRtf = xamlRtfConverterType.GetMethod("ConvertXamlToRtf", BindingFlags.Instance | BindingFlags.NonPublic);
            var rtfContent = (string)convertXamlToRtf.Invoke(xamlRtfConverter, new object[] { xamlContent });
            return rtfContent;
        }
    }
}
