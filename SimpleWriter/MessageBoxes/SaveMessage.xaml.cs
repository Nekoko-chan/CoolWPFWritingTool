using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ComplexWriter.Properties;
using ExtensionObjects;
using global::ComplexWriter.global;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class SaveMessage
    {
        public SaveMessage()
        {
            InitializeComponent();
            Loaded += ErrorWindow_Loaded;
        }

        public static readonly DependencyProperty FilenameProperty =
          DependencyProperty.Register("Filename", typeof(string), typeof(SaveMessage),
              new PropertyMetadata(string.Empty));
        
        public string Filename
        {
            get { return (string)GetValue(FilenameProperty); }
            set { SetValue(FilenameProperty, value); }
        }
        
        void ErrorWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

     
        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }


        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button)sender).Tag;                
        }
    }

}
