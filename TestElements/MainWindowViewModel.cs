using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using WPFTagControl;

namespace TestElements
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private List<string> _suggestedTags;
        private ObservableCollection<string> _selectedTags;

        public MainWindowViewModel()
        {
            SuggestedTags = new List<string> {"Kino","Schließen","Versuch","Blubb"};
            SelectedTags = new ObservableCollection<string>();
        }

        public List<string> SuggestedTags
        {
            get { return _suggestedTags; }
            set
            {
                if (_suggestedTags == value) return;
                _suggestedTags = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<string> SelectedTags
        {
            get { return _selectedTags; }
            set
            {
                if (_selectedTags == value) return;

                if (_selectedTags != null)
                    _selectedTags.CollectionChanged -= CollectionChanged;
                _selectedTags = value;
                if (_selectedTags != null)
                    _selectedTags.CollectionChanged += CollectionChanged;
                OnPropertyChanged();
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedTags));
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}