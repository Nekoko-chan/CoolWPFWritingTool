using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ComplexWriter.Properties;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class ImagePicker 
    {
        public ImagePicker()
        {
            InitializeComponent();
            ImageScaler.InitialDirectory = Settings.Default.ImageFolder;

        }

        public string Filename { get { return ImageScaler.Path; } }

        public ImageSource Image     { get { return ImageScaler.Source; }set { ImageScaler.Source = value; } }
        public double? ImageWidth { get { return ImageScaler.ImageWidth; } set { ImageScaler.ImageWidth = value; } }
        public double? ImageHeight { get { return ImageScaler.ImageHeight
            ; } set { ImageScaler.ImageHeight = value; } }
        public BitmapScalingMode ImageScale { get { return ImageScaler.ImageScale; } set { ImageScaler.ImageScale = value; } }
        public double ImageOpacity { get { return ImageScaler.ImageOpacity; }set { ImageScaler.ImageOpacity = value; }} // ToDo ImageScaler umbauen
        public bool CanDeleteImage { get { return ImageScaler.CanDeleteFile; } set
        {
            ImageScaler.CanDeleteFile = value;
        } }


        public bool CanEditScalingMode { get { return ImageScaler.CanEditScalingMode; } set
        {
            ImageScaler.CanEditScalingMode = value;
        } }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            if (win == null)
                return;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = (MessageBoxResult)((Button)sender).Tag;
       
            if(messageBoxResult!= MessageBoxResult.Cancel)
            {
                Settings.Default.ImageFolder = ImageScaler.InitialDirectory;
                Settings.Default.Save();
            }
            Result = messageBoxResult;
        }

        private void FocusFirst(object sender, RoutedEventArgs e)
        {
            ForFocus.Focus();
        }

       
    }
}
