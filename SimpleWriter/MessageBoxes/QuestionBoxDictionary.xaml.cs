using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ComplexWriter;
using ComplexWriter.MessageBoxes;

namespace ComplexWriter
{
    public partial class QuestionBoxDictionary
    {
        private void CloseMe(object sender, RoutedEventArgs e)
        {
            var window = ((Button) sender).Tag as QuestionBox;
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
