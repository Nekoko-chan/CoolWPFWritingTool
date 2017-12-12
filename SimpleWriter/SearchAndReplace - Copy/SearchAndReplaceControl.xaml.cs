using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ComplexWriter.SearchAndReplace
{
    /// <summary>
    /// Interaction logic for SearchAndReplaceControl.xaml
    /// </summary>
    public partial class SearchAndReplaceControl : UserControl
    {
        public SearchAndReplaceControl()
        {
            InitializeComponent();
            Search.PropertyChanged += Search_PropertyChanged;
        }

        void Search_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SearchString")
                ErrorProvider.Visibility = SearchTheString(0, false) ? Visibility.Collapsed : Visibility.Visible;
        }

        public static readonly DependencyProperty SearchProperty =
            DependencyProperty.Register("Search", typeof(RegexSearch), typeof(MainWindow),
                new PropertyMetadata(new RegexSearch()));

        public RegexSearch Search
        {
            get { return (RegexSearch)GetValue(SearchProperty); }
            set { SetValue(SearchProperty, value); }
        }



        public string SourceString { get { return Search.Text; } set { Search.Text = value; } }

        private void SearchNext(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorProvider.Visibility = SearchTheString(-1, true) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }

        private void ReplaceMe(object sender, RoutedEventArgs e)
        {
            ReplaceOccurance(-1, false);
        }

        private void ReplaceAllMe(object sender, RoutedEventArgs e)
        {
            var success = ReplaceOccurance(0, true);
            while (success)
            {
                success = ReplaceOccurance(-1, true);
            }
            ErrorProvider.Visibility = SearchTheString(0,false) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void HideMe(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            RaiseIsClosedFromWithinEvent();
        }

        private bool _success;

        public bool SearchTheString(int position, bool restart)
        {
            _success = position != -1 ? Search.FindNext(position) : Search.FindNext();
            if (!_success && restart)
                _success = Search.FindNext(0);

            RaiseMatchingChangedEvent(_success, Search.MatchPosition, Search.MatchLength, false, false, false);

            return _success;
        }

        private bool ReplaceOccurance(int position, bool replaceAll)
        {
            if (!_success) // Es wurde noch nichts gefunden
            {
                _success = position == -1 ? Search.FindNext() : Search.FindNext(position);
            }
            if (!_success)
            {
                return false;
            }
            RaiseReplaceStringEvent(
                        replaceBox.Text, SearcBox.Text,Search.MatchPosition, Search.MatchLength, false, replaceAll,
                        RegexSearch.GetOptions(Search.CaseSensitive));
            return _success;
        }



        public static readonly RoutedEvent IsClosedFromWithinEvent = EventManager.RegisterRoutedEvent(
          "IsClosedFromWithin", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchAndReplaceControl));

        public event RoutedEventHandler IsClosedFromWithin
        {
            add { AddHandler(IsClosedFromWithinEvent, value); }
            remove { RemoveHandler(IsClosedFromWithinEvent, value); }
        }

        private void RaiseIsClosedFromWithinEvent()
        {
            var newEventArgs = new RoutedEventArgs(IsClosedFromWithinEvent);
            RaiseEvent(newEventArgs);
        }


        public static readonly RoutedEvent MatchingChangedEvent = EventManager.RegisterRoutedEvent(
          "MatchingChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchAndReplaceControl));

        public event RoutedEventHandler MatchingChanged
        {
            add { AddHandler(MatchingChangedEvent, value); }
            remove { RemoveHandler(MatchingChangedEvent, value); }
        }

        private void RaiseMatchingChangedEvent(bool success, int start, int length, bool replace, bool fromNext, bool replaceAll)
        {
            var newEventArgs = new MatchEventArgs(MatchingChangedEvent, success, start, length, replace, fromNext, replaceAll);
            RaiseEvent(newEventArgs);
        }

        
  public static readonly RoutedEvent  ReplaceStringEvent = EventManager.RegisterRoutedEvent(
          "ReplaceString", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchAndReplaceControl));

        public event RoutedEventHandler ReplaceString
        {
            add { AddHandler( ReplaceStringEvent, value); }
            remove { RemoveHandler( ReplaceStringEvent, value); }
        }

        private void RaiseReplaceStringEvent(string oldText, string newText, int start, int length, bool replace, bool replaceAll, RegexOptions options)
        {
            var newEventArgs = new ReplaceEventArgs(ReplaceStringEvent,oldText, newText, start, length, replace, replaceAll, options);
            RaiseEvent(newEventArgs);
        }

    }
}
