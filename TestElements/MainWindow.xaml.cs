using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFTagControl;

namespace TestElements
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }

        private void closeme(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Ctl_Tags_OnTagRemoved(object sender, TagEventArgs e)
        {
        }

        private void Ctl_Tags_OnTagEdited(object sender, TagEventArgs e)
        {
            var suggestedTags = ((MainWindowViewModel)DataContext).SuggestedTags;
            if (!suggestedTags.Contains(e.Item.Text))
            {
                suggestedTags.Add(e.Item.Text);
            }
        }

        private void Ctl_Tags_TagAdded(object sender, TagEventArgs e)
        {
            var suggestedTags = ((MainWindowViewModel)DataContext).SuggestedTags;
            if (!suggestedTags.Contains(e.Item.Text))
            {
                suggestedTags.Add(e.Item.Text);
            }
        }
    }
}
