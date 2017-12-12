using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace ComplexWriter
{
    public class PrintSettings
    {
        public FlowDocument Document { get; set; }
        public string PdfName { get; set; }
        public bool UseBlackAndWhite { get; set; }
        public bool ShowPageNumber { get; set; }
        public bool UseWatermark { get; set; }
        public bool UseOldNumbering { get; set; }
        public Watermark Watermark { get; set; }
        public Brush BackgroundBrush { get; set; }
        public int From { get; set; }
        public int Till { get; set; }
        public PageCountElement PageCountElement { get; set; }
        public int FontSizeModifier { get; set; }

        public FontFamily FontFamily { get; set; }
        public double FontSize { get; set; }

        public bool UseCharacters { get; set; }
    }

    // bool useLeading, double fontSize, int from, int till, bool useOldNumbering, 
}
