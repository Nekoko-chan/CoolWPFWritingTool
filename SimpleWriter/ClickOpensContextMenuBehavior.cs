using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ComplexWriter
{
    public class ClickOpensContextMenuBehavior
    {
        private static readonly DependencyProperty ClickOpensContextMenuProperty =
          DependencyProperty.RegisterAttached(
            "Enabled", typeof(bool), typeof(ClickOpensContextMenuBehavior),
            new PropertyMetadata(new PropertyChangedCallback(HandlePropertyChanged))
          );

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClickOpensContextMenuProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(ClickOpensContextMenuProperty, value);
        }

        private static void HandlePropertyChanged(
          DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is Image)
            {
                var image = obj as Image;
                image.MouseLeftButtonDown -= ExecuteMouseDown;
                image.MouseLeftButtonDown += ExecuteMouseDown;
            }

            if (obj is Hyperlink)
            {
                var hyperlink = obj as Hyperlink;
                hyperlink.Click -= ExecuteClick;
                hyperlink.Click += ExecuteClick;
            }
        }

        private static void ExecuteMouseDown(object sender, MouseEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            bool enabled = (bool)obj.GetValue(ClickOpensContextMenuProperty);
            if (enabled)
            {
                if (sender is Image)
                {
                    var image = (Image)sender;
                    if (image.ContextMenu != null)
                        image.ContextMenu.IsOpen = true;
                }
            }
        }

        private static void ExecuteClick(object sender, RoutedEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            bool enabled = (bool)obj.GetValue(ClickOpensContextMenuProperty);
            if (enabled)
            {
                if (sender is Hyperlink)
                {
                    var hyperlink = (Hyperlink)sender;
                    if (hyperlink.ContextMenu != null)
                        hyperlink.ContextMenu.IsOpen = true;
                }
            }
        }
    }

}
