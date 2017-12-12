using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ComplexWriter.MessageBoxes
{
    public partial class MessageBoxDictionary
    {
        private void CloseMe(object sender, RoutedEventArgs e)
        {
            var window = ((Button)sender).Tag as MessageBoxes.MessageBox;
            if (window != null)
                window.ActivateOk();
        }

       
        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
         
        }


    }
}
