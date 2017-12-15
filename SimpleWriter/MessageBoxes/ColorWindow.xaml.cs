using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class ColorWindow
    {
        public ColorWindow()
        {
            InitializeComponent();
        }
  
        public static readonly DependencyProperty MessageProperty =
       DependencyProperty.Register("Message", typeof(string), typeof(ColorWindow), new PropertyMetadata(string.Empty));
        
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty SubMessageProperty =
   DependencyProperty.Register("SubMessage", typeof(string), typeof(ColorWindow), new PropertyMetadata(string.Empty));

        public string SubMessage
        {
            get { return (string)GetValue(SubMessageProperty); }
            set { SetValue(SubMessageProperty, value); }
        }
        public static readonly DependencyProperty ShowPredefinedProperty =
 DependencyProperty.Register("ShowPredefined", typeof(bool), typeof(ColorWindow), new PropertyMetadata(true));

        public bool ShowPredefined
        {
            get { return (bool)GetValue(ShowPredefinedProperty); }
            set { SetValue(ShowPredefinedProperty, value); }
        }

        public static readonly DependencyProperty ColorsProperty =
       DependencyProperty.Register("PredefinedColors", typeof(ObservableCollection<Color>), typeof(ColorWindow), new PropertyMetadata(new ObservableCollection<Color>()));

        public ObservableCollection<Color> PredefinedColors
        {
            get { return (ObservableCollection<Color>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }


        public static readonly DependencyProperty MessageIconProperty =
      DependencyProperty.Register("MessageIcon", typeof(MessageBoxImage), typeof(ColorWindow), new PropertyMetadata(MessageBoxImage.None));

        public MessageBoxImage MessageIcon
        {
            get { return (MessageBoxImage)GetValue(MessageIconProperty); }
            set { SetValue(MessageIconProperty, value); }
        }

        public Color SelectedColor
        {
            get { return cPicker.SelectedColor; }
        }

        public Color StartColor
        {
            get { return cPicker.StartColor; }
            set { cPicker.StartColor = cPicker.SelectedColor = value; }

        }


        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult) ((Button) sender).Tag;
        }

        private void FocusFirst(object sender, RoutedEventArgs e)
        {
            ForFocus.Focus();
        }

        public static void ShowErrorMessage(MainWindow mainWindow, Exception exception)
        {
            var dlg = new ColorWindow() { Title = Properties.Resources.Error, Message = exception.Message, SubMessage = exception.StackTrace,Owner = mainWindow, MessageIcon = MessageBoxImage.Error};
            dlg.ShowDialog();
        }

        private Color m_color;

        private void UpdateSelectedColor(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (e.NewValue != m_color)
            {
                ForFocus.IsEnabled = true;
            }
        }
    }
}
