using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ComplexWriter.Properties;

namespace ComplexWriter
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen (BitmapImage image )
        {
            InitializeComponent ( );
            image2.Source = image;

            Loaded += LoadedScreen;
        }

        private void LoadedScreen(object sender, RoutedEventArgs e)
        {
            var pos = Settings.Default.StartPosition;
            var screen = Screen.FromPoint(new System.Drawing.Point((int)pos.X, (int)pos.Y));

            var x = screen.Bounds.X + (screen.Bounds.Width - ActualWidth) / 2;
            var y = (screen.Bounds.Height - ActualHeight) / 2;

            Top = y;
            Left = x;

        }
    }
}
