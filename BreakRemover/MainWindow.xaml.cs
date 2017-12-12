using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace BreakRemover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadText(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = "Textdateien|*.txt"
                };

                if (! dlg.ShowDialog() ?? false) return;

                using (var str = new StreamReader(new FileStream(dlg.FileName, FileMode.Open),Encoding.UTF8))
                {
                    var text = str.ReadToEnd();

                    ResultBox.Text = text.Replace(Environment.NewLine, " ");
                
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(),"Fehler",MessageBoxButton.OK,MessageBoxImage.Error);
            }


        }
    }
}
