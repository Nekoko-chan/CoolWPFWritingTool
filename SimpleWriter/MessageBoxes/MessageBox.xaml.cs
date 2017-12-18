using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class MessageBox
    {
        public MessageBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MessageProperty =
       DependencyProperty.Register("Message", typeof(string), typeof(MessageBox), new PropertyMetadata(string.Empty));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageFontSizeProperty =
      DependencyProperty.Register("MessageFontSize", typeof(double), typeof(MessageBox), new PropertyMetadata(22d));

        public double MessageFontSize
        {
            get { return (double)GetValue(MessageFontSizeProperty); }
            set { SetValue(MessageFontSizeProperty, value); }
        }

        public static readonly DependencyProperty MessageIconProperty =
      DependencyProperty.Register("MessageIcon", typeof(MessageBoxImage), typeof(MessageBox), new PropertyMetadata(MessageBoxImage.None));

        public MessageBoxImage MessageIcon
        {
            get { return (MessageBoxImage)GetValue(MessageIconProperty); }
            set { SetValue(MessageIconProperty, value); }
        }


        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            if(win == null)
                return;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult) ((Button) sender).Tag;
        }

        public static MessageBoxResult ShowMessage(Window owner, string text, string title, MessageBoxImage image=MessageBoxImage.Information, double maxwidth=600, double fontSize=22d)
        {
            var dlg = new MessageBox { Title = title, Message = text, Owner = owner, MessageIcon = image ,MaxWidth = 600,MessageFontSize = fontSize};
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

        public static void ShowErrorMessage(MainWindow mainWindow, Exception exception)
        {
            var dlg = new MessageBox { Title = exception.Message, Message = exception.StackTrace, Owner = mainWindow, MessageIcon = MessageBoxImage.Error,MessageFontSize = 10d};
            dlg.ShowDialog();
        }

        public static void ShowSimpleToDoMessage()
        {
            ShowMessage(MainWindow.Global, "Die Methode ist noch nicht implementiert", "Noch zu machen",
                MessageBoxImage.Warning);
        }
    }
}
