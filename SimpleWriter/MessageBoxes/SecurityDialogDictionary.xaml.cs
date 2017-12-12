using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ComplexWriter.MessageBoxes
{
    public partial class SecurityDialogDictionary
    {
        private void CloseMe(object sender, RoutedEventArgs e)
        {
            var window = ((Button)sender).Tag as MessageResultWindow;
            if (window != null)
            {
                window.Cancel();
            }
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            if(win == null)
                return;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }
    }
}
