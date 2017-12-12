using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
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
    public partial class SymbolWindow
    {
        public SymbolWindow()
        {
            InitializeComponent();
        }

        public string Symbol     { get; set; }

        public FontElement DefaultFamily { get; set; }

        public static readonly DependencyProperty SelectedFamilyProperty =
          DependencyProperty.Register("SelectedFamily", typeof(FontElement), typeof(SymbolWindow),
              new PropertyMetadata(null));

        public FontElement SelectedFamily
        {
            get { return (FontElement)GetValue(SelectedFamilyProperty); }
            set { SetValue(SelectedFamilyProperty, value); }
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

      

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button) sender).Tag;
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            theSymbols.ItemsSource =
                SpecialUnicode.Unicodes.Select(
                    elem => new UnicodeElement {Description = elem[1], UnicodeString = elem[0]}).OrderBy(elem=>elem.UnicodeString).ToList();

            var firstOrDefault = MainWindow.Global.FontFamilies.FirstOrDefault(
               elem => elem.Family.FamilyNames.Any(el => el.Value == "Arial Unicode MS"));
            if(firstOrDefault == null)
            {
                MessageBox.ShowMessage(MainWindow.Global, "Die Schriftart \"Arial Unicode MS\" wurde nicht gefunden.\nSomit steht Ihnen der \"Standard-Schrift verwenden\"-Button nicht zur Verfügung.\n\nKopieren sie die Schrift wenn gewünscht in den Ordner \"PossibleFonts\".", "Fehlende Schrift", MessageBoxImage.Error);
                ArialBtn.IsEnabled = false;
            }
        }

        private void ReturnElement(object sender, RoutedEventArgs e)
        {
            var symbol = ((Button)sender).Tag as UnicodeElement;
            if (symbol == null)
                return;
            Symbol = symbol.UnicodeString;

            Result = MessageBoxResult.OK;
        }

        private void SetArialAsFont(object sender, RoutedEventArgs e)
        {
            SelectedFamily =
                MainWindow.Global.FontFamilies.FirstOrDefault(
                    elem => elem.Family.FamilyNames.Any(el => el.Value == "Arial Unicode MS"));
        }

        private void SetStandardAsFont(object sender, RoutedEventArgs e)
        {
            SelectedFamily = DefaultFamily;
        }
    }
}
