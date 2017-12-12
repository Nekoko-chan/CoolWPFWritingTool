using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComplexWriter
{
    public class HomeGroupStyleTemplateSelector : StyleSelector
    {
        public Style NullGroupStyle { get; set; }
        public Style DefaultStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            var group = item as CollectionViewGroup;

            if (element != null && group != null && group.Name == null)
            {
                return this.NullGroupStyle;
            }

            return this.DefaultStyle;
        }
    }
}
