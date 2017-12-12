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
    public partial class QuoteWindow
    {
        public QuoteWindow()
        {
            InitializeComponent();
        }


        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        public static DependencyProperty OpeningQuotationProperty = DependencyProperty.Register("OpeningQuotation", typeof(string), typeof(QuoteWindow), new PropertyMetadata("\u0022"));

        public string OpeningQuotation
        {
            get { return (string) GetValue(OpeningQuotationProperty); }
            set { SetValue(OpeningQuotationProperty, value); }

        }

        public static DependencyProperty SingleOpeningQuotationProperty = DependencyProperty.Register("SingleOpeningQuotation", typeof(string), typeof(QuoteWindow), new PropertyMetadata("\u0027"));

        public string SingleOpeningQuotation
        {
            get { return (string)GetValue(SingleOpeningQuotationProperty); }
            set { SetValue(SingleOpeningQuotationProperty, value); }
        }


        public static DependencyProperty ClosingQuotationProperty = DependencyProperty.Register("ClosingQuotation", typeof(string), typeof(QuoteWindow), new PropertyMetadata("\u0027"));

        public string ClosingQuotation
        {
            get { return (string)GetValue(ClosingQuotationProperty); }
            set { SetValue(ClosingQuotationProperty, value); }
        }

        public static DependencyProperty SingleClosingQuotationProperty = DependencyProperty.Register("SingleClosingQuotation", typeof(string), typeof(QuoteWindow), new PropertyMetadata("\u0022"));

        public string SingleClosingQuotation
        {
            get { return (string)GetValue(SingleClosingQuotationProperty); }
            set { SetValue(SingleClosingQuotationProperty, value); }
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button) sender).Tag;
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            OpeningQuotes.ItemsSource = SpecialUnicode.Quotations;
            SingleOpeningQuotes.ItemsSource = SpecialUnicode.Quotations;
            ClosingQuotes.ItemsSource = SpecialUnicode.Quotations;
            ClosingSingleQuotes.ItemsSource = SpecialUnicode.Quotations;
        }

        private void ReturnElement(object sender, RoutedEventArgs e)
        {
          
        }

        private void SetOpening(object sender, RoutedEventArgs e)
        {
            OpeningQuotation= ((ToggleButton) sender).Content as string;

        }

        private void SetClosing(object sender, RoutedEventArgs e)
        {
            ClosingQuotation = ((ToggleButton)sender).Content as string;
        }

        private void SetSingleOpening(object sender, RoutedEventArgs e)
        {
            SingleOpeningQuotation = ((ToggleButton)sender).Content as string;
        }

        private void SetSingleClosing(object sender, RoutedEventArgs e)
        {
            SingleClosingQuotation = ((ToggleButton)sender).Content as string;
        }
    }
}
