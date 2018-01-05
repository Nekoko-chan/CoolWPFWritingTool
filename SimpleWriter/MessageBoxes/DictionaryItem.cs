using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;

namespace ComplexWriter.MessageBoxes
{
    public class DictionaryItem: INotifyPropertyChanged
    {
        private string _value;
        private bool _isForDeletion;

        public bool IsForDeletion
        {
            get { return _isForDeletion; }
            set { _isForDeletion = value;OnPropertyChanged(); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value;
                FirstLetter = "foo";
                OnPropertyChanged();
            }
        }

        public string FirstLetter
        {
            get { return Value?.Substring(0, 1).ToUpperInvariant()??""; }
            set {OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
