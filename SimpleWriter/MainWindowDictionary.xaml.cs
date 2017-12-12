using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ComplexWriter;
using ComplexWriter;
using CustomControls;
using ExtensionObjects;

namespace ComplexWriter
{
    public partial class MainWindowDictionary
    {
        private void CloseMe(object sender, RoutedEventArgs e)
        {
            var window = ((Button)sender).Tag as MainWindow;
            if (window != null)
                window.CloseIt();
        }

        // private void MinimizeMe(object sender, RoutedEventArgs e)
        //{
        //    var window = ((MaskedImageButtonEnlarge)sender).Tag as Border;
        //     if (window == null) 
        //         return;
        //     bool b = window.Visibility ==Visibility.Collapsed;

        //     window.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
        //}

   
        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as MainWindow;
            if (win == null) return;

            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;

            if (!win.PopupIsAttachedToMainWindow) return;

            win.StylePopup2.VerticalOffset += e.VerticalChange;
            win.StylePopup2.HorizontalOffset += e.HorizontalChange;
        }

       
        private void DragAThumb(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            Window transparentWindow = thumb.TemplatedParent as Window;
            if (thumb != null && transparentWindow.WindowState == WindowState.Normal)
            {
                Rect windowRect = new Rect(transparentWindow.Left, transparentWindow.Top, transparentWindow.ActualWidth, transparentWindow.ActualHeight);
                double maxReducedHeight = Math.Max(0, transparentWindow.ActualHeight - transparentWindow.MinHeight);
                double maxReducedWidth = Math.Max(0, transparentWindow.ActualWidth - transparentWindow.MinWidth);
                double reducedHeight = e.VerticalChange;
                double reducedWidth = e.HorizontalChange;
                if (thumb.Name.Equals("PART_SizeSE"))
                {
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    transparentWindow.Width = Math.Max(transparentWindow.ActualWidth + reducedWidth, transparentWindow.MinWidth);
                    transparentWindow.Height = Math.Max(transparentWindow.ActualHeight + reducedHeight, transparentWindow.MinHeight);
                }
                else if (thumb.Name.Equals("PART_SizeNW"))
                {
                    reducedWidth = Math.Min(reducedWidth, maxReducedWidth);
                    reducedHeight = Math.Min(reducedHeight, maxReducedHeight);
                    windowRect.Y += reducedHeight;
                    windowRect.X += reducedWidth;
                    windowRect.Width -= reducedWidth;
                    windowRect.Height -= reducedHeight;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeSW"))
                {
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    reducedWidth = Math.Min(reducedWidth, maxReducedWidth);
                    windowRect.X += reducedWidth;
                    windowRect.Width -= reducedWidth;
                    windowRect.Height += reducedHeight;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeNE"))
                {
                    reducedHeight = Math.Min(reducedHeight, maxReducedHeight);
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    windowRect.Y += reducedHeight;
                    windowRect.Height = transparentWindow.ActualHeight - reducedHeight;
                    windowRect.Width = transparentWindow.ActualWidth + reducedWidth;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeN"))
                {
                    reducedHeight = Math.Min(reducedHeight, maxReducedHeight);
                    windowRect.Y += reducedHeight;
                    windowRect.Height = transparentWindow.ActualHeight - reducedHeight;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeS"))
                {
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    transparentWindow.Height = transparentWindow.ActualHeight + reducedHeight;
                }
                else if (thumb.Name.Equals("PART_SizeW"))
                {
                    reducedWidth = Math.Min(reducedWidth, maxReducedWidth);
                    windowRect.X += reducedWidth;
                    windowRect.Width = transparentWindow.ActualWidth - reducedWidth;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeE"))
                {
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    transparentWindow.Width = transparentWindow.ActualWidth + reducedWidth;
                }
            }
        }

        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            new About(){Owner = MainWindow.Global}.ShowDialog();
        }

        private void FocusBox(object sender, RoutedEventArgs e)
        {
            var window = ((ToggleButton)sender).Tag as MainWindow;
            if (window != null)
            {
                window.FocusTextbox();
            }
        }

        private void UpdateTopMost(object sender, RoutedEventArgs e)
        {
            var window = ((ToggleButton)sender).Tag as MainWindow;
            if (window != null)
            {
                window.Topmost = ((ToggleButton) sender).IsChecked!=true;
            }
        }

        private void RaiseDragStart(object sender, DragEventArgs dragEventArgs)
        {
            var window = ((Thumb) sender).Tag as MainWindow;
            if (window == null) return;
            window.HideStylePopup();
        }

        private void RaiseDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var window = ((Thumb)sender).Tag as MainWindow;
            if (window == null ||!window.PopupIsAttachedToMainWindow) return;
                window.ShowStylePopupIfAllowed();
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var window = ((Thumb)sender).Tag as MainWindow;
            if (window == null || !window.PopupIsAttachedToMainWindow) return;
            window.HideStylePopup();
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit += delegate {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location,"-n");
            };
            Application.Current.Shutdown();
        }
    }
}
