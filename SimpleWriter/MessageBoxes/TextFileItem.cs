using System.ComponentModel;
using System.IO;
using ComplexWriter.Annotations;

namespace ComplexWriter
{
    public class TextFileItem: INotifyPropertyChanged
    {
        private string _filename;
        private bool _isChecked;
        private bool _isFolder;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Filename
        {
            get { return _filename; }
            set
            {
                if (value == _filename) return;
                _filename = value;
                OnPropertyChanged(nameof(Filename));
            }
        }

        public bool IsChecked 
        {
            get { return _isChecked; }
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string Folder => Path.GetDirectoryName(Filename);

    }
}
