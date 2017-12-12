using System.Windows;
using CustomControls;

namespace ComplexWriter
{
    public class Sva : DependencyObject
    {
        public static readonly DependencyProperty TickVisibilityProperty = DependencyProperty.RegisterAttached("TickVisibility", typeof(Visibility), typeof(Sva),new PropertyMetadata(Visibility.Visible));

        public static Visibility GetTickVisibility(DependencyObject d)
        {
            return (Visibility)d.GetValue(TickVisibilityProperty);
        }

        public static void SetTickVisibility(DependencyObject d, Visibility value)
        {
            d.SetValue(TickVisibilityProperty, value);
        }
    }
    
}
