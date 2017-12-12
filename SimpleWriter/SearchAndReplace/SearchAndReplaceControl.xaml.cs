using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

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
            VisibilityChanged += SearchAndReplaceControl_VisibilityChanged;
            Visibility = Visibility.Collapsed;
            ReplaceHelper.MatchingChangedEvent += ReplaceHelper_MatchingChangedEvent;
        }

        void ReplaceHelper_MatchingChangedEvent(object sender, MatchEventArgs e)
        {
            RaiseMatchingChangedEvent(e);
        }

        void SearchAndReplaceControl_VisibilityChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        if (ReplaceHelper.FocusReplace)
                            ReplaceBox.Focus();
                        else
                            SearcBox.Focus();
                    }, DispatcherPriority.Render);
                }
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }

        //private FindAndReplaceManager _manager;

        public FlowDocument FlowDocument
        {
            get { return ReplaceHelper.FlowDocument; }
            set { ReplaceHelper.FlowDocument = value; }
        }

        public void ResetManager()
        {
             ReplaceHelper.ResetManager();
        }

        private void SearchNext(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchNextElement();
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }

       

     //   public static readonly DependencyProperty WholeWordProperty =
     //    DependencyProperty.Register("WholeWord", typeof(bool), typeof(SearchAndReplaceControl), new PropertyMetadata(false));

     //   public bool WholeWord
     //   {
     //       get { return (bool)GetValue(WholeWordProperty); }
     //       set { SetValue(WholeWordProperty, value); }
     //   }

     //   public static readonly DependencyProperty CanReplaceProperty =
     //   DependencyProperty.Register("CanReplace", typeof(bool), typeof(SearchAndReplaceControl), new PropertyMetadata(false));

     //   public bool CanReplace
     //   {
     //       get { return (bool)GetValue(CanReplaceProperty); }
     //       set { SetValue(CanReplaceProperty, value); }
     //   }

     //   public static readonly DependencyProperty CaseSensitiveProperty =
     //DependencyProperty.Register("CaseSensitive", typeof(bool), typeof(SearchAndReplaceControl), new PropertyMetadata(false));

     //   public bool CaseSensitive
     //   {
     //       get { return (bool)GetValue(CaseSensitiveProperty); }
     //       set { SetValue(CaseSensitiveProperty, value); }
     //   }

        private Visibility _visibility;
        //private FlowDocument _flowDocument;

        public new Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (value != _visibility)
                {
                    _visibility = value;
                    base.Visibility = value;
                }
                RaiseVisibilityChangedEvent();
            }
        }

        //public bool FocusReplace { get; set; }

        //private FindOptions GetFindOptions()
        //{
        //    var options =  FindOptions.None;
        //    if (WholeWord)
        //        options |= FindOptions.MatchWholeWord;

        //    if (CaseSensitive)
        //        options |= FindOptions.MatchCase;


        //    return options;
        //}

        public void SwitchVisibility(bool switchIt)
        {
            bool b = Visibility == Visibility.Visible;
            if (!b)
                Visibility = Visibility.Visible;
            else
            {
                Visibility = !switchIt ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        private void ReplaceMe(object sender, RoutedEventArgs e)
        {
            ReplaceMe();
        }

        private void ReplaceMe()
        {
            try
            {
                TextPointer test;
                ErrorProvider.Visibility = ReplaceHelper.ReplaceAndSelect(MainWindow.Global.Caretposition, false, out test,SearcBox.Text,ReplaceBox.Text)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
            catch (Exception e)
            {
                MainWindow.Global.AddException(e);
            }
        }

        private void ReplaceAllMe(object sender, RoutedEventArgs e)
        {
            try
            {
                TextPointer contentStart = FlowDocument.ContentStart;
                int counter = 1;
                while (ReplaceHelper.ReplaceAndSelect(contentStart, false, out contentStart, SearcBox.Text, ReplaceBox.Text))
                {
                    counter++;
                }

                MessageBoxes.MessageBox.ShowMessage(MainWindow.Global,
                    string.Format("{0} Ersetzungen vorgenommen", counter), "Ersetzungen", MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }


        //private bool ReplaceAndSelect(TextPointer start, bool round, out TextPointer endPosition, string findText, string replaceText)
        //{
        //    endPosition = null;

        //    if (String.IsNullOrEmpty(findText )||start == null)
        //    {
        //        return true;
        //    }

        //    if (_manager == null)
        //    {
        //        _manager = new FindAndReplaceManager(FlowDocument);
        //    }

        //    _manager.CurrentPosition = start;

        //    TextRange textRange = _manager.Replace(findText, replaceText, GetFindOptions());
        //    if (textRange != null)
        //    {
        //        endPosition = textRange.End;
        //        return SearchString(endPosition, true,true);
        //    }
        //    if (_manager.CurrentPosition.CompareTo(FlowDocument.ContentEnd) == 0 && round)
        //    {
        //        return ReplaceAndSelect(FlowDocument.ContentStart, false,out endPosition,findText,replaceText);
        //    }
        //    RaiseMatchingChangedEvent(true, null,true);
        //    return false;
        //}

        private void HideMe(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            RaiseIsClosedFromWithinEvent();
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

        public static readonly RoutedEvent VisibilityChangedEvent = EventManager.RegisterRoutedEvent(
          "VisibilityChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchAndReplaceControl));

        public event RoutedEventHandler VisibilityChanged
        {
            add { AddHandler(VisibilityChangedEvent, value); }
            remove { RemoveHandler(VisibilityChangedEvent, value); }
        }

        private void RaiseVisibilityChangedEvent()
        {
            var newEventArgs = new RoutedEventArgs(VisibilityChangedEvent);
            RaiseEvent(newEventArgs);
        }


        public static readonly RoutedEvent MatchingChangedEvent = EventManager.RegisterRoutedEvent(
          "MatchingChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchAndReplaceControl));

        public event RoutedEventHandler MatchingChanged
        {
            add { AddHandler(MatchingChangedEvent, value); }
            remove { RemoveHandler(MatchingChangedEvent, value); }
        }

        private void RaiseMatchingChangedEvent(MatchEventArgs e)
        {
            if (e.RoutedEvent == null)
                e.RoutedEvent = MatchingChangedEvent;
            RaiseEvent(e);
        }
       
        private void SearchMe(object sender, TextChangedEventArgs e)
        {
            try
            {
                ReplaceHelper.SearchText = SearcBox.Text;
                SearchNextElement(MainWindow.Global.Caretposition,false);
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }

        public void SearchNextElement(TextPointer pointer,bool fromReplace)
        {
            bool searchString = ReplaceHelper.SearchString(pointer, true, fromReplace);
            ReplaceHelper.CanReplace = !string.IsNullOrEmpty(SearcBox.Text) && searchString;

            ErrorProvider.Visibility = searchString ? Visibility.Collapsed : Visibility.Visible;
        }

        public void SearchNextElement()
        {
            try
            {
                SearchNextElement(ReplaceHelper.Manager.CurrentPosition,false);
            }
            catch (Exception e)
            {
                MainWindow.Global.AddException(e);
            }
        }

        private void CheckSeachrKey(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.Key == Key.F3 ||e.Key == Key.Return)
                    SearchNextElement();
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }

        private void ReplaceSingle(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key != Key.Enter)
                    return;
                ReplaceMe();
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }


        public static readonly DependencyProperty ReplaceHelperProperty =
        DependencyProperty.Register("ReplaceHelper", typeof(ReplaceLogic), typeof(SearchAndReplaceControl), new PropertyMetadata(new ReplaceLogic()));

        public ReplaceLogic ReplaceHelper
        {
            get { return (ReplaceLogic)GetValue(ReplaceHelperProperty); }
            set { SetValue(ReplaceHelperProperty, value); }
        }

        public bool FocusReplace { get { return ReplaceHelper.FocusReplace; } set
        {
            try
            {
                ReplaceHelper.FocusReplace = value;
            }
            catch (Exception e)
            {
                MainWindow.Global.AddException(e);
            }
        } }
    }
}
