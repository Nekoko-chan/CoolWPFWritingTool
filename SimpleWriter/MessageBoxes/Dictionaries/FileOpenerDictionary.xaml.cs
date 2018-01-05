using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ComplexWriter.MessageBoxes
{
    public partial class FileOpenerDictionary
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


        private void DragAThumb(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var win = thumb.TemplatedParent as Window;
            if (win == null)
                return;

            double maxReducedHeight = Math.Max(0, win.ActualHeight - win.MinHeight);
            double maxReducedWidth = Math.Max(0, win.ActualWidth - win.MinWidth);
            double reducedHeight = e.VerticalChange;
            double reducedWidth = e.HorizontalChange;
            switch (thumb.Name)
            {
                case "PART_SizeSE":
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    win.Width = Math.Max(win.ActualWidth + reducedWidth, win.MinWidth);
                    win.Height = Math.Max(win.ActualHeight + reducedHeight, win.MinHeight);
                    break;
                case "PART_SizeS":
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    win.Height = win.ActualHeight + reducedHeight; break;
                case "PART_SizeE":
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    win.Width = win.ActualWidth + reducedWidth;
                    break;
            }
        }
    }
}
