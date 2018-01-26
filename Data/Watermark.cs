using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Writer.Data
{
    public class Watermark:INotifyPropertyChanged
    {
        private ImageSource _imageSource;
        private Size _size;
        private double _opacity;

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value;OnPropertyChanged(); }
        }

        public Size Size
        {
            get { return _size; }
            set { _size = value;OnPropertyChanged(); }
        }

        public double Opacity
        {
            get { return _opacity; }
            set { _opacity = value;OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public Watermark CopyOf()
        {
            if (ImageSource == null)
                return null;

            var waterMark = new Watermark {ImageSource = _imageSource.Clone(),Opacity = _opacity,Size = _size};
            
            return waterMark;
        }

    }
}