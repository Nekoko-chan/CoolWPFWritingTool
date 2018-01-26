using System;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace Writer.Data
{
    public static class PageBreakHelper
    {

        public static void HidePageBreaker(this FlowDocument doc)
        {
            foreach (var para in doc.Blocks.OfType<Paragraph>().Where(para => para.BreakPageBefore))
            {
                para.Background = Brushes.Transparent;
                para.FontSize = 0.1;
            }
        }


        public static bool UpdateBreaks(this FlowDocument doc)
        {
            return doc.Blocks.Aggregate(false, (current, block) => current | block.UpdateBreaks());
        }

        public static bool UpdateBreaks(this ListItem block)
        {
            return block.Blocks.Aggregate(false, (current, block1) => current | block1.UpdateBreaks());
        }

        public static bool UpdateBreaks(this Block block)
        {
            var para = block as Paragraph;
            if (para != null)
            {
                var b = para.Background as DrawingBrush;
                para.BreakPageBefore = b != null;// && b.ImageSource.ToString().Equals("pack://application:,,,/ComplexWriter;component/Images/Seitenumbruch.png", StringComparison.Ordinal);
                return para.BreakPageBefore;
            }

            var sec = block as Section;
            if (sec != null)
            {
                return sec.Blocks.Aggregate(false, (current, block1) => current | block1.UpdateBreaks());
            }

            var lis = block as List;
            if (lis != null)
            {
                return lis.ListItems.Aggregate(false, (current, block1) => current | block1.UpdateBreaks());
            }
            return false;
        }


        public static void ChangeFontSize(this FlowDocument doc, int change)
        {
            if (change == 0) return;

            foreach (var block in doc.Blocks.ToList())
            {
                block.ChangeBlockFontSize(change);
            }
        }

        public static void ChangeBlockFontSize(this Block block, int change)
        {
            if (change == 0) return;

            //block.FontSize = Math.Max(1, block.FontSize + change);
            var para = block as Paragraph;
            if (para != null)
            {
                para.ChangeBlockFontSize(change);
                return;
            }
            var list = block as List;
            if(list != null)
            {

                list.ChangeBlockFontSize(change);
                return;
            }
            var tabl = block as Table;
            if (tabl == null) return;

            tabl.FontSize = Math.Max(1, tabl.FontSize + change);
        }

        public static void ChangeBlockFontSize(this Paragraph block, int change)
        {
            if (change == 0) return;

            //block.FontSize = Math.Max(1, block.FontSize + change);
            foreach (var inline in block.Inlines.ToList())
            {
                inline.ChangeFontSize(change);
            }
        }

        //public static void ChangeBlockFontSize(this Table block, int change)
        //{
        //    if (change == 0) return;

        //    block.FontSize = Math.Max(1, block.FontSize + change);
        //    foreach (var inline in block.RowGroups)
        //    {
        //        foreach (var row in inline.Rows)
        //        {

        //        }
        //    }
        //}

        public static void ChangeFontSize(this Inline inline, int change)
        {
            inline.FontSize = Math.Max(1, inline.FontSize + change);
        }

        public static void ChangeBlockFontSize(this List block, int change)
        {
            if (change == 0) return;

            //block.FontSize = Math.Max(1, block.FontSize + change);
            foreach (var listItem in block.ListItems.ToList())
            {
                listItem.ChangeBlockFontSize(change);
            }
        }

        public static void ChangeBlockFontSize(this ListItem block, int change)
        {
            if (change == 0) return;

            //block.FontSize = Math.Max(1, block.FontSize + change);
            foreach (var block1 in block.Blocks.ToList())
            {
                block1.ChangeBlockFontSize(change);
            }
        }


        /// <summary>
        /// Erweitert die Ursprüngliche methode dahingehend, dass der Paragraph nur hinzugefügt wird, wenn er nicht leer ist
        /// </summary>
        /// <param name="col"></param>
        /// <param name="reference"></param>
        /// <param name="newParapgraph"></param>
        /// <param name="removeEmpty"></param>
        public static void InsertBefore(this BlockCollection col, Paragraph reference, Paragraph newParapgraph,bool removeEmpty)
        {
            if (newParapgraph.IsEmpty() && removeEmpty) return;

            col.InsertBefore(reference,newParapgraph);
        }
    }
}
