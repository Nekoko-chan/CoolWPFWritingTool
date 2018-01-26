using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FileExporter.Annotations;

namespace FileExporter
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

        public bool IsFolder
        {
            get { return _isFolder; }
            set
            {
                if (value == _isFolder) return;
                _isFolder = value;
                OnPropertyChanged(nameof(IsFolder));
            }
        }
    }
}
