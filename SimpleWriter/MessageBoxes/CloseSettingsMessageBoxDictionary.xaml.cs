using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ComplexWriter.MessageBoxes;
using ComplexWriter;
using ComplexWriter.MessageBoxes;

namespace ComplexWriter
{
    public partial class CloseSettingsMessageBoxDictionary
    {
        private void CloseMe(object sender, RoutedEventArgs e)
        {
            var window = ((Button)sender).Tag as CloseSettingsMessageBox;
            if (window != null)
                window.CancelMe();
        }

       
        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
         
        }


    }
}
