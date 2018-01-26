using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace FileExporter
{
    public static class DocumentExtensions
    {
        public static FlowDocument AddFlowDocument(this FlowDocument d1, FlowDocument d2)
        {
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
