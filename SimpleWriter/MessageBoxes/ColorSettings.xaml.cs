using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class ColorSettings
    {
        public ColorSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DependencyProperty for 'Colors'
        /// </summary>
        public static readonly DependencyProperty ColorsProperty =
        DependencyProperty.Register("Colors", typeof(ObservableCollection<ColorElement>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<ColorElement>()));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ColorElement> Colors
        {
            get { return (ObservableCollection<ColorElement>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button) sender).Tag;
        }

        private void UpdateColor(object sender, RoutedEventArgs e)
        {
            var colorelem = ((Button)sender).Tag as ColorElement;
            if (colorelem == null) return;

            var colorwd = new ColorWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                StartColor = colorelem.Color,
                PredefinedColors = new ObservableCollection<Color>(Colors.Select(elem => elem.Color)),
                Title = colorelem.Key
            };

            if (colorwd.ShowDialog() != true || colorwd.Result == MessageBoxResult.Cancel) return;


            colorelem.Color = colorwd.SelectedColor;
        }
    }
}
