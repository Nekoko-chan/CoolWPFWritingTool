using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ComplexWriter
{
    public class MatchEventArgs : RoutedEventArgs
    {
        public bool Success { get; private set; }
        public TextRange Range { get; private set; }
        public bool FromReplace { get; private set; }

        public MatchEventArgs(RoutedEvent handler,bool success,TextRange range,bool fromReplace):base(handler)
        {
            Success = success;
            Range = range;
            FromReplace = fromReplace;
        }
    }

    public class ReplaceEventArgs : RoutedEventArgs
    {
        public ReplaceEventArgs(RoutedEvent handler,  string oldValue,string newValue, int startPosition,int length, bool so, bool ra, RegexOptions op):base(handler)
        {
            StartPosition = startPosition;
            Length = length;
            NewValue = newValue;
            OldValue = oldValue;
            SelectOnly = so;
            ReplaceAll = ra;
            RegexOptions = op;
        }

        public int StartPosition { get; private set; }
        public int Length{ get; private set; }
        public bool SelectOnly { get; private set; }
        public string NewValue { get; private set; }
        public string OldValue { get; private set; }
        public RegexOptions RegexOptions { get; private set; }
        public bool ReplaceAll { get; private set; }
    }
}
