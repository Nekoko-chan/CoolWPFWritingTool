using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ComplexWriter.SearchAndReplace
{
    public class RegexSearch :INotifyPropertyChanged
    {
        public RegexSearch(string text, string search, bool caseSensitive = false, bool whole = false)
        {
            _text = text;
            _searchString = search;
            _caseSensitive = caseSensitive;
            _wholeWord = whole;
            ResetSearch();
        }


        public RegexSearch()
        {
            ResetSearch();
        }

        private Regex _regex;
        private Match _match;
        private bool _caseSensitive;
        private string _searchString;
        private string _text;
        private bool _wholeWord;

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                _caseSensitive = value;
                ResetSearch();
                OnPropertyChanged();
            }
        }

        public int MatchLength
        {
            get
            {
                return _match == null ? 0 : _match.Length;
            }

            private set
            {
                OnPropertyChanged();
            }
        }

        public int MatchPosition
        {
            get { return _match == null ? -1 : _match.Index; }
            private set
            {
                OnPropertyChanged();
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                ResetSearch();
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                ResetSearch();
                OnPropertyChanged();
            }
        }

        public bool WholeWord
        {
            get { return _wholeWord; }
            set
            {
                _wholeWord = value;
                ResetSearch();
                OnPropertyChanged();
            }
        }

        public bool FindNext()
        {
            return FindNext(_match == null ? 0 : _match.Index + 1);
        }

        public bool FindNext(int position)
        {
            if (Text == null || SearchString == null)
            {
                _match = null;
                MatchLength = 0; //foo
                MatchPosition = 0; //foo
                return false;
            }
            _match = _regex.Match(Text, Math.Min(position, Text.Length));
            bool findNext = _match.Success && _match.Index >= position;
            if (!findNext)
                _match = null;
            MatchLength = 0; //foo
            MatchPosition = 0; //foo

            return findNext;
        }

        internal static RegexOptions GetOptions(bool caseSensitive)
        {
            return RegexOptions.CultureInvariant | (caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase) | RegexOptions.CultureInvariant;

        }
        private RegexOptions GetOptions()
        {
            return GetOptions(CaseSensitive);
        }

        private static string FormatSearchString(string searchString, bool wholeWord)
        {
            var fpattern = wholeWord ? @"(?<!\w){0}(?!\w)" : "{0}";

            return string.Format(fpattern, searchString);
        }

        private void ResetSearch()
        {
            _regex = new Regex(FormatSearchString(Regex.Escape(SearchString ?? ""), WholeWord), GetOptions());
            _match = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
