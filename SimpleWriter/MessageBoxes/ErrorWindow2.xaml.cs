using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
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
    public partial class ErrorWindow2
    {
        public ErrorWindow2()
        {
            InitializeComponent();
            Width = Settings.Default.LastSizeErrorWindow.Width;
            Height = Settings.Default.LastSizeErrorWindow.Height;
            Left = Settings.Default.StartPositionErrorWindow.X;
            Top = Settings.Default.StartPositionErrorWindow.Y;
            MainWindow.Global.ErrorMessages.CollectionChanged += ErrorMessages_CollectionChanged;

          

            Loaded += ErrorWindow_Loaded;
        }


        public static readonly DependencyProperty CurrentGroupProperty =
           DependencyProperty.Register("CurrentGroup", typeof(string), typeof(ErrorWindow2),
               new PropertyMetadata(string.Empty));

        public string CurrentGroup
        {
            get { return (string)GetValue(CurrentGroupProperty); }
            set { SetValue(CurrentGroupProperty, value); }
        }
        
        void ErrorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var current = MainWindow.Global.ErrorMessages.OrderByDescending(elem => elem.OccuranceTime).ToList();

            if (current.Any())
                CurrentGroup = current[0].GroupName;
        }

        void ErrorMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.OfType<ErrorMessageItem>().Any())
            {
                CurrentGroup = ((ErrorMessageItem) e.NewItems[0]).GroupName;
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
       
    }

    public class GroupContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item,
            DependencyObject container)
        {
            var group = item as CollectionViewGroup;

            if (group == null)
            {
                return MainWindow.Global.FindResource("NonExpandableGroup") as Style;
            }

            return MainWindow.Global.FindResource(group.Name.Equals("") ? "NonExpandableGroup" :
                group.Name.Equals(ErrorMessageItem.ERROR) ? "ExpandableGroupError" :
                "ExpandableGroup") as Style;
        }
    }
   
}
