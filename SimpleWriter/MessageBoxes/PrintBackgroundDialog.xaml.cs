using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ComplexWriter.Properties;
using ExtensionObjects;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class PrintBackgroundDialog
    {
        public PrintBackgroundDialog()
        {
            InitializeComponent();
            Loaded += ErrorWindow_Loaded;
        }


        public PrintBackgroundDialog(Brush brush):this()
        {
            ImageScaler.InitialDirectory = Settings.Default.ImageFolder;
            ComboBox.SelectedIndex = 0;

            var solid = brush as SolidColorBrush;
            if (solid != null)
            {
                ColorRadio.IsChecked = true;
                ColorBrush = solid;
                return;
            }


            var image = brush as ImageBrush;
            if(image == null) return;

            ImageButton.IsChecked = true;

            ImageScaler.Source = image.ImageSource;
            ImageScaler.ImageWidth = image.Viewport.Width;
            ImageScaler.ImageHeight = image.Viewport.Height;
            ImageScaler.ImageOpacity = image.Opacity;
            ComboBox.SelectedItem = image.TileMode;
        }


        public Brush ResultBrush
        {
            get
            {
                if (ColorRadio.IsChecked == true) return ColorBrush;

                return ImageScaler.Source == null ? null:
                    BuildImageBrush();
            }
        }

        private ImageBrush BuildImageBrush()
        {
            return new ImageBrush(ImageScaler.Source)
            {
                Stretch = Stretch.Uniform,
                Viewport = new Rect(0, 0, ImageScaler.ImageWidth.GetSizeValue(), ImageScaler.ImageHeight.GetSizeValue()),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile,
                Opacity = ImageScaler.ImageOpacity
            };
        }

        

        public static readonly DependencyProperty ColorBrushProperty =
DependencyProperty.Register("ColorBrush", typeof(SolidColorBrush), typeof(PrintBackgroundDialog),
 new PropertyMetadata(Brushes.Black));

        public SolidColorBrush ColorBrush
        {
            get { return (SolidColorBrush)GetValue(ColorBrushProperty); }
            set { SetValue(ColorBrushProperty, value); }
        }

        public static readonly DependencyProperty ColorsProperty =
     DependencyProperty.Register("PredefinedColors", typeof(ObservableCollection<Color>), typeof(PrintBackgroundDialog), new PropertyMetadata(new ObservableCollection<Color>()));

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
            Settings.Default.ImageFolder = ImageScaler.InitialDirectory;
            Settings.Default.Save();
            Result = (MessageBoxResult)((Button)sender).Tag;                
        }
        
        private void UpdateColor(object sender, RoutedEventArgs e)
        {
            SetColor(sender as Button);

        }

        private void SetColor( Button bord)
        {
            if (bord == null) return;

            var brush = bord.Background;
            ColorBrush = (SolidColorBrush)brush;
        }

    }

}
