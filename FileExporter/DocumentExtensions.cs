using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileExporter
{
    public static class DocumentExtensions
    {
        public static FlowDocument AddFlowDocument(this FlowDocument d1, FlowDocument d2)
        {
            var range = new TextRange(d2.ContentStart,d2.ContentEnd);
            range.ApplyPropertyValue(FlowDocument.FontSizeProperty, 15.25d);
            range.ApplyPropertyValue(FlowDocument.FontFamilyProperty, new FontFamily("Amazon Ember"));

            var ret = d1;
            ret.Blocks.Add(new Paragraph {BreakPageBefore = true});
                foreach (Block block in d2.Blocks.ToList())
                {
                    ret.Blocks.Add(block);
                }
            return d1;
        }
    }
}
