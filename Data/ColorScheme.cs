using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Writer.Data
{
    
    public class ColorElement:INotifyPropertyChanged
    {
        private Color _color;
        private string _key;
        private string _display;

        public ColorElement(string key, string display, ResourceDictionary dict)
        {
            Key = key;
            Display = display;
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

        public ColorElement(string key, string display, Color color)
        {
            Key = key;
            Color = color;
            Display = display;
        }

        public string Display
        {
            get { return _display; }
            set
            {
                if (value.Equals(_display)) return;
                _display = value; 
                OnPropertyChanged(nameof(Display));
            }
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ColorComparer : IEqualityComparer<ColorElement>
    {

        public bool Equals(ColorElement x, ColorElement y)
        {
            //Check whether the objects are the same object. 
            if (ReferenceEquals(x, y)) return true;

            //Check whether the products' properties are equal. 
            return x != null && y != null && x.Color.Equals(y.Color) && x.Key.Equals(y.Key);
        }

        public int GetHashCode(ColorElement obj)
        {
            //Get hash code for the Name field if it is not null. 
            int hashProductName = obj.Key?.GetHashCode() ?? 0;

            //Get hash code for the Code field. 
            int hashProductCode = obj.Color.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductName ^ hashProductCode;
        }
    }
}
