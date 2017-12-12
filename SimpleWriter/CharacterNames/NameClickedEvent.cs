using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace ComplexWriter.CharacterNames
{
    internal class NameClickedRoutedEventArgs: RoutedEventArgs
    {
        public NameClickedRoutedEventArgs(RoutedEvent eventName) : base(eventName) { }

        public string ClickedText { get; set; }
    }
}
