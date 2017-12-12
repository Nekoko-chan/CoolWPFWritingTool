using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class XamlMessageBox
    {
        public XamlMessageBox()
        {
            InitializeComponent();
        }
  
        public static readonly DependencyProperty MessageProperty =
       DependencyProperty.Register("Message", typeof(string), typeof(XamlMessageBox), new PropertyMetadata(string.Empty));
        
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty SubMessageProperty =
   DependencyProperty.Register("SubMessage", typeof(string), typeof(XamlMessageBox), new PropertyMetadata(string.Empty));

        public string SubMessage
        {
            get { return (string)GetValue(SubMessageProperty); }
            set { SetValue(SubMessageProperty, value); }
        }


        public static readonly DependencyProperty MessageIconProperty =
      DependencyProperty.Register("MessageIcon", typeof(MessageBoxImage), typeof(XamlMessageBox), new PropertyMetadata(MessageBoxImage.None));

        public MessageBoxImage MessageIcon
        {
            get { return (MessageBoxImage)GetValue(MessageIconProperty); }
            set { SetValue(MessageIconProperty, value); }
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

        public static MessageBoxResult ShowMessage(Window owner,string text,string title,MessageBoxImage image)
        {
            var dlg = new XamlMessageBox() {Title = title, SubMessage = text,Owner = owner,MessageIcon = image};
            dlg.ShowDialog();

            return dlg.Result;

        }

        private void FocusFirst(object sender, RoutedEventArgs e)
        {
            ForFocus.Focus();
        }

        public void ActivateOk()
        {
            Result = MessageBoxResult.OK;
        }

        public static void ShowErrorMessage(MainWindow mainWindow, Exception exception, string title=null)
        {
            var dlg = new XamlMessageBox() { Title = title??"Fehler", Message = exception.Message, SubMessage = exception.StackTrace,Owner = mainWindow, MessageIcon = MessageBoxImage.Error};
            dlg.ShowDialog();
        }
    }
}
