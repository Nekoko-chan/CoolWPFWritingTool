using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TestElements
{
    /// <summary>
    /// Interaction logic for DesignHelper.xaml
    /// </summary>
    public partial class DesignHelper : UserControl
    {
        public DesignHelper()
        {
            InitializeComponent();
            CommandBinding cbPaste = new CommandBinding(ApplicationCommands.Paste);
            cbPaste.Executed += PasteCommandHandler;
            Box.CommandBindings.Add(cbPaste);
        }

        /// <summary>
        /// DependencyProperty for 'Datetext'
        /// </summary>
        public static readonly DependencyProperty DatetextProperty =
        DependencyProperty.Register("Datetext", typeof(string), typeof(DesignHelper), new UIPropertyMetadata(GetCurrentDate()));

        private static string GetCurrentDate()
        {
            return $"{DateTime.Now:dd.MM-yyyy}";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Datetext
        {
            get { return (string)GetValue(DatetextProperty); }
            set { SetValue(DatetextProperty, value); }
        }


        private void Shutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void PasteCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Box.Selection.Text = Clipboard.GetText();
            Box.Selection.Select(Box.Selection.End,Box.Selection.End);
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
           
        }
    }
}
