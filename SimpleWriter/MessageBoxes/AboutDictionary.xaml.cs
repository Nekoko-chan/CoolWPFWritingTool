using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ComplexWriter;
using ComplexWriter.MessageBoxes;

namespace ComplexWriter
{
    public partial class AboutDictionary
    {
        private void CloseMe(object sender, RoutedEventArgs e)
        {
            var window = ((Button) sender).Tag as About;
            if (window != null)
                window.DialogResult = true;
        }
   
    }
}
