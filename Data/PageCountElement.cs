using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Windows.Media;

namespace Writer.Data
{
    public class PageCountElement :INotifyPropertyChanged
    {
        public PageCountElement()
        {
            FontSize = 10;
            ForgroundBrush = Brushes.Black;
            FontFamily = Utilities.PossibleFonts[0].Family;
        }

        private FontFamily _fontFamily;
        private Brush _forgroundBrush;
        private bool _useLeadingZero;
        private double _fontSize;
        public event PropertyChangedEventHandler PropertyChanged;

        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; OnPropertyChanged();}
        }

        public Brush ForgroundBrush
        {
            get { return _forgroundBrush; }
            set { _forgroundBrush = value;
                ForgroundBrushString = value== null? string.Empty:XamlWriter.Save(value); OnPropertyChanged();}
        }

        public bool UseLeadingZero
        {
            get { return _useLeadingZero; }
            set { _useLeadingZero = value;OnPropertyChanged(); }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; OnPropertyChanged(); }
        }

        public string ForgroundBrushString { get; set; }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
        {
            try
            {
                var val2 = obj as PageCountElement;
                return val2 != null && val2.UseLeadingZero.Equals(UseLeadingZero) &&
                       XamlWriter.Save(val2.ForgroundBrush).Equals(XamlWriter.Save(ForgroundBrush)) &&
                       FontFamily.Source.Equals(val2.FontFamily.Source) && FontSize.Equals(val2.FontSize);
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return FontFamily.GetHashCode() + ForgroundBrush.GetHashCode() + UseLeadingZero.GetHashCode();
        }
    }
}
