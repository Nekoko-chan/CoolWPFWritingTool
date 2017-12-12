using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ComplexWriter
{
    public class AutoCorrectElement :INotifyPropertyChanged
    {
        private string _tooltipText;
        private string _originalValue;
        private TextPointer _startPointer;
        private TextPointer _endPointer;
        private string _correctedValue;

        public string TooltipText
        {
            get { return _tooltipText; }
            set { _tooltipText = value; OnPropertyChanged();}
        }

        public string OriginalValue
        {
            get { return _originalValue; }
            set { _originalValue = value; OnPropertyChanged(); }
        }

        public string CorrectedValue
        {
            get { return _correctedValue; }
            set { _correctedValue = value; OnPropertyChanged(); }
        }

        public TextPointer StartPointer
        {
            get { return _startPointer; }
            set { _startPointer = value; OnPropertyChanged(); }
        }

        public TextPointer EndPointer
        {
            get { return _endPointer; }
            set { _endPointer = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); 
        }
    }
}
