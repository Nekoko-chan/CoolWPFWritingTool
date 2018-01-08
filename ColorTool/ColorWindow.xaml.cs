using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColorTool
{
    /// <summary>
    /// Interaction logic for ColorWindow.xaml
    /// </summary>
    public partial class ColorWindow : Window
    {
        public ColorWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// DependencyProperty for 'PredefinedColors'
        /// </summary>
        public static readonly DependencyProperty PredefinedColorsProperty =
        DependencyProperty.Register("PredefinedColors", typeof(ObservableCollection<Color>), typeof(ColorWindow), new UIPropertyMetadata(new ObservableCollection<Color>()));

        private MessageBoxResult _result;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Color> PredefinedColors
        {
            get { return (ObservableCollection<Color>)GetValue(PredefinedColorsProperty); }
            set { SetValue(PredefinedColorsProperty, value); }
        }
        /// <summary>
        /// DependencyProperty for 'StartColor'
        /// </summary>
        public static readonly DependencyProperty StartColorProperty =
        DependencyProperty.Register("StartColor", typeof(Color), typeof(ColorWindow), new UIPropertyMetadata(Colors.Black));

        /// <summary>
        /// 
        /// </summary>
        public Color StartColor
        {
            get { return (Color)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }



        public Color SelectedColor { get; set; }

        private void UpdateSelectedColor(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SelectedColor = e.NewValue;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button)sender).Tag;
        }
        public MessageBoxResult Result
        {
            get { return _result; }
            set { _result = value; DialogResult = true; }
        }

    }
}
