using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using ComplexWriter.Properties;
using global::ComplexWriter.global;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class ErrorWindow 
    {
        public ErrorWindow()
        {
            InitializeComponent();
            messageList.ItemsSource = MainWindow.Global.ErrorMessages;
            Width = Settings.Default.LastSizeErrorWindow.Width;
            Height = Settings.Default.LastSizeErrorWindow.Height;
            Left = Settings.Default.StartPositionErrorWindow.X;
            Top = Settings.Default.StartPositionErrorWindow.Y;
            messageList.Items.SortDescriptions.Clear();
            messageList.Items.SortDescriptions.Add(new SortDescription("OccuranceTime",ListSortDirection.Ascending));
            MainWindow.Global.ErrorMessages.CollectionChanged += ErrorMessages_CollectionChanged;
            Loaded += ErrorWindow_Loaded;
        }

        void ErrorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var last = MainWindow.Global.ErrorMessages.OrderBy(elem => elem.OccuranceTime).LastOrDefault();
            if (last != null)
                messageList.ScrollIntoView(last);
        }

        void ErrorMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.OfType<ErrorMessageItem>().Any())
            {
                messageList.ScrollIntoView(e.NewItems.OfType<ErrorMessageItem>().First());
                foreach (var errorMessageItem in MainWindow.Global.ErrorMessages)
                {
                    errorMessageItem.ForUpdateTrigger = !errorMessageItem.ForUpdateTrigger;
                }
            }
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void results_Click(object sender, RoutedEventArgs e)
        {
            //ToDO: Sortierung übernehmen
        }

        private void ClearList(object sender, RoutedEventArgs e)
        {
            MainWindow.Global.ErrorMessages.Clear();
        }

        private void ExpandAll(object sender, RoutedEventArgs e)
        {
            foreach (var errorMessageItem in MainWindow.Global.ErrorMessages.Where(errorMessageItem => errorMessageItem.Exception != null && !errorMessageItem.ShowsDetails))
            {
                errorMessageItem.ShowsDetails = true;

            }
        }

        private void CollapseAll(object sender, RoutedEventArgs e)
        {
            foreach (var errorMessageItem in MainWindow.Global.ErrorMessages.Where(errorMessageItem => errorMessageItem.ShowsDetails))
            {
                errorMessageItem.ShowsDetails = false;
            }
        }

        private void UpdateSizeAndLocation(object sender, CancelEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                Settings.Default.LastSizeErrorWindow = new Size(ActualWidth,ActualHeight);
                Settings.Default.StartPositionErrorWindow = new Point(Left, Top);
                Settings.Default.Save();
            }
        }

        private void UpdateFilter(object sender, RoutedEventArgs e)
        {
            if (messageList == null)
                return;
            messageList.Items.Filter = null;
            messageList.Items.Filter = Filter;
        }

        private bool Filter(object o)
        {
            var errorItem = o as ErrorMessageItem;
            if (errorItem == null)
                return true; // Andere Objekte immer anzeigen

            if (errorItem.Severity == ErrorSeverity.Error && ErrorBtn.IsChecked == true)
                return true;

            //if (errorItem.Severity == ErrorSeverity.Warning && WarningBtn.IsChecked == true)
            //    return true;

            if (errorItem.Severity == ErrorSeverity.Information && Infobtn.IsChecked == true)
                return true;

            return false;
            //return errorItem.Severity == ErrorSeverity.None && NoneBtn.IsChecked == true;
        }
    }
}
