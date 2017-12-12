using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Markup;
using ComplexWriter.SearchAndReplace;

namespace ComplexWriter
{
    public class ReplaceLogic : INotifyPropertyChanged
    {
        private FindAndReplaceManager _manager;
        private string _searchText;
        private bool _wholeWord;
        private bool _caseSensitive;
        private bool _focusReplace;
        private bool _canReplace;
        private FlowDocument _flowDocument;

        public FlowDocument FlowDocument
        {
            get { return _flowDocument; }
            set
            {
                _flowDocument = value;
            }
        }

        public void ResetManager()
        {
            _manager = null;
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value;OnPropertyChanged(); }
        }


        public bool WholeWord   
        {
            get { return _wholeWord; }
            set { _wholeWord = value; OnPropertyChanged(); }
        }

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set { _caseSensitive = value; OnPropertyChanged(); }
        }

        public bool FocusReplace
        {
            get { return _focusReplace; }
            set { _focusReplace = value; OnPropertyChanged(); }
        }

        public bool CanReplace
        {
            get { return _canReplace; }
            set { _canReplace = value; OnPropertyChanged(); }
        }

        public FindAndReplaceManager Manager { get { return _manager;} set {_manager=value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool SearchString(TextPointer start, bool round, bool fromReplace)
        {
            String findText = SearchText;
            if (String.IsNullOrEmpty(findText))
            {
                RaiseMatchingChangedEvent(false, null, fromReplace);
                return true;
            }

            if (_manager == null)
            {
                _manager = new FindAndReplaceManager(FlowDocument);
            }

            _manager.CurrentPosition = start;

            TextRange textRange = _manager.FindNext(findText, GetFindOptions());
            if (textRange != null)
            {
                RaiseMatchingChangedEvent(true, textRange, fromReplace);
                return true;
            }
            if (_manager.CurrentPosition.CompareTo(FlowDocument.ContentEnd) == 0 && round)
            {
                return SearchString(FlowDocument.ContentStart, false, fromReplace);
            }
            RaiseMatchingChangedEvent(true, null, false);
            return false;
        }

        public FindOptions GetFindOptions()
        {
            var options = FindOptions.None;
            if (WholeWord)
                options |= FindOptions.MatchWholeWord;

            if (CaseSensitive)
                options |= FindOptions.MatchCase;


            return options;
        }

        public bool ReplaceAndSelect(TextPointer start, bool round, out TextPointer endPosition, string findText,
            string replaceText)
        {
            endPosition = null;

            if (String.IsNullOrEmpty(findText) || start == null)
            {
                return true;
            }

            if (_manager == null)
            {
                _manager = new FindAndReplaceManager(FlowDocument);
            }

            _manager.CurrentPosition = start;

            TextRange textRange = _manager.Replace(findText, replaceText, GetFindOptions());
            if (textRange != null)
            {
                endPosition = textRange.End;
                return SearchString(endPosition, true, true);
            }
            if (_manager.CurrentPosition.CompareTo(FlowDocument.ContentEnd) == 0 && round)
            {
                return ReplaceAndSelect(FlowDocument.ContentStart, false, out endPosition, findText, replaceText);
            }
            RaiseMatchingChangedEvent(true, null, true);
            return false;
        }


        public void ReplaceAll(string input,string output)
        {
            TextPointer contentStart = FlowDocument.ContentStart;
            while (ReplaceAndSelect(contentStart, false, out contentStart, input,output))
            {
            }
        }

        public event EventHandler<MatchEventArgs> MatchingChangedEvent;

        private void RaiseMatchingChangedEvent(bool success, TextRange start, bool fromReplace)
        {
            var newEventArgs = new MatchEventArgs(null, success, start, fromReplace);
            if (MatchingChangedEvent != null)
                MatchingChangedEvent(this, newEventArgs);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}