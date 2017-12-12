using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class PageCountSettings
    {
        public PageCountSettings()
        {
            InitializeComponent();
            Loaded += ErrorWindow_Loaded;
        }

        public static readonly DependencyProperty PageCountElementProperty =
     DependencyProperty.Register("PageCountElement", typeof(PageCountElement), typeof(PageCountSettings),
         new PropertyMetadata(null));

        public PageCountElement PageCountElement
        {
            get { return (PageCountElement)GetValue(PageCountElementProperty); }
            set { SetValue(PageCountElementProperty, value); }
        }

        public static readonly DependencyProperty ColorsProperty =
     DependencyProperty.Register("PredefinedColors", typeof(ObservableCollection<Color>), typeof(PageCountSettings), new PropertyMetadata(new ObservableCollection<Color>()));

        public ObservableCollection<Color> PredefinedColors
        {
            get { return (ObservableCollection<Color>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }
       
        void ErrorWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

     
        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            if (win == null) return;

            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }


        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button)sender).Tag;                
        }

        private void ScrollToElement(object sender, SelectionChangedEventArgs e)
        {
            var sdr = ((ListBox)sender);
            if (sdr.IsFocused)
                return;

            var item = sdr.SelectedItem;

            sdr.ScrollIntoView(item);

            var element = item as FontElement;
            if (element != null&& PageCountElement!= null)
                PageCountElement.FontFamily = element.Family;

        }

        private void UpdateColor(object sender, RoutedEventArgs e)
        {
            SetColor(sender as Button);

        }

        private void SetColor( Button bord)
        {
            if (bord == null) return;

            var brush = bord.Background;
            PageCountElement.ForgroundBrush = brush;
        }

        private void CheckText(object sender, TextCompositionEventArgs e)
        {
            var check = Utilities.IsAllowedText(e.Text);
            e.Handled = !check;
        }
    }

}
