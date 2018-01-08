using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ColorTool.Annotations;

namespace ColorTool
{
    
    public class ColorElement:INotifyPropertyChanged
    {
        private Color _color;
        private string _key;

        public ColorElement(string key, ResourceDictionary dict)
        {
            Key = key;
            var elem = dict[key];

            if (elem == null)
                Color = Colors.Transparent;
            if(elem is Color)
            {
                Color = (Color) elem;
                return;
            }

            var brush = elem as SolidColorBrush;
            Color = brush?.Color ?? Colors.Transparent;
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (value.Equals(_color)) return;
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public string Key   
        {
            get { return _key; }
            set
            {
                if (value == _key) return;
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
