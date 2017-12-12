using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class QuestionBox 
    {
        public QuestionBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MessageProperty =
       DependencyProperty.Register("Message", typeof(string), typeof(QuestionBox), new PropertyMetadata(string.Empty));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageFontSizeProperty =
     DependencyProperty.Register("MessageFontSize", typeof(double), typeof(QuestionBox), new PropertyMetadata(15d));

        public double MessageFontSize
        {
            get { return (double)GetValue(MessageFontSizeProperty); }
            set { SetValue(MessageFontSizeProperty, value); }
        }

        public static readonly DependencyProperty CanCanelProperty =
       DependencyProperty.Register("CanCanel", typeof(bool), typeof(QuestionBox), new PropertyMetadata(true));


        public bool CanCanel
        {
            get { return (bool)GetValue(CanCanelProperty); }
            set { SetValue(CanCanelProperty, value); }
        }

      

        public void CancelMe()
        {
            Result = MessageBoxResult.Cancel;
            
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

        public static MessageBoxResult ShowMessage(Window owner,string text,string title,bool canCancel=true, double maxwidth =1600, double fontsize=20d)
        {
            var dlg = new QuestionBox() {Title = title, Message = text,Owner = owner,CanCanel =canCancel, MaxWidth = maxwidth,MessageFontSize = fontsize};
            dlg.ShowDialog();

            return dlg.Result;

        }


        private void FocusFirst(object sender, RoutedEventArgs e)
        {
            ForFocus.Focus();
        }
    }
}
