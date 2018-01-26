using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using ExtensionObjects;
using Microsoft.Win32;
using FlowDocumentExtensions = Writer.Data.FlowDocumentExtensions;

namespace Writer.Data
{
    [TypeConverter(typeof(SpezialSettingsConverter))]
    public class ComplexStyle :INotifyPropertyChanged
    {
        private FontFamily _fontFamily;
        private double _fontSize;
        private bool _isItalic;
        private bool _isBold;
        private bool _isUnderlined;
        private bool _isStrikedOut;
        private string _fontFamilyString;
        private string _foregroundBrushString;
        private string _backgroundBrushString;
        private string _caption;
        private bool _isCondenced;
        private bool _isDefault;
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public FontFamily FontFamily
        {
            get
            {
                if (_fontFamily!= null && _fontFamily.Source.Equals(FontFamilyString))
                    return _fontFamily;

                return string.IsNullOrEmpty(FontFamilyString)? Utilities.PossibleFonts.First().Family: new FontFamily(FontFamilyString);
            }
            set { _fontFamily = value;
                FontFamilyString = value == null? string.Empty: value.Source;
                OnPropertyChanged();
                OnPropertyChanged("FontGroupString");
            }
        }

        [XmlIgnore]
        public string FontGroupString
        {
            get
            {
                var fam =Utilities.PossibleFonts.FirstOrDefault(elem => elem.Family.Equals(FontFamily));
                return fam == null ? string.Empty : fam.Group;
            }
        }

        [XmlElement("IsDefault")]
        public bool IsDefault 
        {
            get { return _isDefault; }
            set { _isDefault = value;OnPropertyChanged(); }
        }

        [XmlElement("FontFamily")]
        public string FontFamilyString
        {
            get { return _fontFamilyString; }
            set { _fontFamilyString = value; OnPropertyChanged(); }
        }

        [XmlElement("FontSize")]
        public double FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; OnPropertyChanged(); }
        }

        [XmlElement("IsItalic")]
        public bool IsItalic
        {
            get { return _isItalic; }
            set { _isItalic = value; OnPropertyChanged(); }
        }

        [XmlElement("IsBold")]
        public bool IsBold
        {
            get { return _isBold; }
            set { _isBold = value; OnPropertyChanged(); Weight=new FontWeight();}
        }

        [XmlElement("IsCondenced")]
        public bool IsCondenced
        {
            get { return _isCondenced; }
            set { _isCondenced = value; OnPropertyChanged(); Stretch = new FontStretch(); }
        }

        public FontStyle Style
        {
            get { return IsItalic ? FontStyles.Italic : FontStyles.Normal; }
            set { OnPropertyChanged(); }
        }

        public FontWeight Weight
        {
            get { return IsBold ? FontWeights.Bold : FontWeights.Normal; }
            set { OnPropertyChanged();}
        }

        public FontStretch Stretch {
            get { return IsCondenced ? FontStretches.Condensed : FontStretches.Normal; }
            set { OnPropertyChanged(); }
        }

        public TextDecorationCollection Decoration
        {
            get
            {
                return IsStrikedOut ? TextDecorations.Strikethrough : IsUnderlined ? TextDecorations.Underline : null;
            }
            set { OnPropertyChanged(); }
        }


        [XmlElement("IsUnderline")]
        public bool IsUnderlined
        {
            get { return _isUnderlined; }
            set
            {
                if (_isUnderlined == value)
                    return;
                _isUnderlined = value; OnPropertyChanged(); Decoration= new TextDecorationCollection();
            }
        }

        [XmlElement("IsStrikedOut")]
        public bool IsStrikedOut
        {
            get { return _isStrikedOut; }
            set
            {
                if (_isStrikedOut == value)
                    return;
                _isStrikedOut = value;
                OnPropertyChanged();
                Decoration = new TextDecorationCollection();
            }
        }


        [XmlElement("Caption")]
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; OnPropertyChanged();
                HasCaption = true;
            }
        }

        [XmlIgnore]
        public bool HasCaption
        {
            get { return !string.IsNullOrEmpty(Caption); }
            set { OnPropertyChanged();}
        }


        [XmlIgnore]
        public Brush ForegroundBrush
        {
            get { return ForegroundBrushString.ToBrush(); }
            set { ForegroundBrushString = value == null? string.Empty:value.ToXamlString(); OnPropertyChanged(); }
        }

        [XmlElement("ForegroundBrush")]
        public string ForegroundBrushString
        {
            get { return _foregroundBrushString; }
            set { _foregroundBrushString = value; OnPropertyChanged(); }
        }

        [XmlIgnore]
        public Brush BackgroundBrush
        {
            get { return BackgroundBrushString.ToBrush(); }
            set { BackgroundBrushString = value== null ? string.Empty: value.ToXamlString(); OnPropertyChanged(); }
        }

        public string BackgroundBrushString
        {
            get { return _backgroundBrushString; }
            set { _backgroundBrushString = value; OnPropertyChanged(); }
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); 
        }

        public override bool Equals(object obj)
        {
            try
            {
                var sty = obj as ComplexStyle;
                if(sty == null)
                    return false;
        
                return sty.IsBold == IsBold && sty.IsItalic == IsItalic && sty.IsStrikedOut == IsStrikedOut &&
                       sty.IsUnderlined == IsUnderlined &&
                       FontSize.Equals(sty.FontSize) &&
                       EqualNullable(sty)&&
                       sty.IsCondenced == IsCondenced;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool EqualNullable(ComplexStyle sty)
        {
            return IsSamebrush(sty.BackgroundBrush, BackgroundBrush) &&
                   IsSamebrush(sty.ForegroundBrush, ForegroundBrush) &&IsSameFamily(sty.FontFamily,FontFamily);
        }

        private static bool IsSamebrush(Brush b1, Brush b2)
        {
            var s1 = b1 as SolidColorBrush;
            var s2 = b2 as SolidColorBrush;

            if (s1 == null && s2 == null) return true;
            if (s1 == null ) return false;
            return s2 != null && Color.Equals(s1.Color, s2.Color);
        }

        private static bool IsSameFamily(FontFamily f1, FontFamily f2)
        {
            try
            {
                if (f1 == null && f2 == null) return true;
                if (f1 == null) return false;
                var objA = f1.FamilyNames.First().Value;
                var objB = f2.FamilyNames.First().Value;
                return string.Equals(objA, objB);
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public static bool Equals(ComplexStyle style1, ComplexStyle style2)
        {
            if (style1 == null && style2 == null) return true;
            if (style1 == null || style2 == null) return false;

            return style1.Equals(style2);
        }

        public override int GetHashCode()
        {
            return IsBold.GetHashCode() + IsItalic.GetHashCode() + IsStrikedOut.GetHashCode() +
                   IsUnderlined.GetHashCode() +
                   FontSize.GetHashCode() +
                   (string.IsNullOrEmpty(BackgroundBrushString) ? 0 : BackgroundBrushString.GetHashCode()) +
                   (string.IsNullOrEmpty(ForegroundBrushString) ? 0 : ForegroundBrushString.GetHashCode()) +
                   (string.IsNullOrEmpty(FontFamilyString) ? 0 : FontFamilyString.GetHashCode());
        }

        public ComplexStyle Copy()
        {
            return new ComplexStyle
            {
                FontSize = FontSize,
                FontFamily = FontFamily,
                IsBold = IsBold,
                IsItalic = IsItalic,
                IsStrikedOut = IsStrikedOut,
                IsUnderlined = IsUnderlined,
                IsCondenced = IsCondenced,
                ForegroundBrush = ForegroundBrush,
                BackgroundBrush = BackgroundBrush
            };
        }
    }

    public class ComplexStyles : INotifyPropertyChanged
    {
        public ComplexStyles()
        {
            Styles = new ObservableCollection<ComplexStyle>();
        }
        private ObservableCollection<ComplexStyle> _styles;
        private bool _hasItems;
        private string _title;

        [XmlArrayItem("ComplexStyle", typeof(ComplexStyle))]
        [XmlArray("TheComplexStyles")]
        public ObservableCollection<ComplexStyle> Styles
        {
            get { return _styles; }
            set
            {
                if (_styles != null)
                {
                    _styles.CollectionChanged -= CollectionChanged;
                }
                _styles = value;
                if(_styles!=null)
                    _styles.CollectionChanged += CollectionChanged;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool HasItems
        {
            get { return _hasItems; }
            set
            {
                if (_hasItems == value)
                    return;
                _hasItems = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string Title
        {
            get
            {
                return _title;
            }
            set { _title = value; OnPropertyChanged();}
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Styles");
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if(e.NewItems!= null)
                        foreach (var newItem in e.NewItems.OfType<ComplexStyle>())
                        {
                            newItem.PropertyChanged += ItemChangedProperty;
                        }
                    HasItems = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        foreach (var olditem in e.OldItems.OfType<ComplexStyle>())
                        {
                            olditem.PropertyChanged -= ItemChangedProperty;
                        }
                    HasItems = Styles.Any(); break;
                case NotifyCollectionChangedAction.Reset:
                    HasItems = false;break;
            }
        }

        private void ItemChangedProperty(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Styles");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Export(string defaultFolder)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "Style.sty",
                DefaultExt = "sty",
                Filter = "Stildatei|*.sty",
                InitialDirectory = defaultFolder,
                CheckFileExists = false
            };
            if (dlg.ShowDialog() == false) return;

            using (var writer=new StreamWriter(dlg.FileName))
                this.ToWriter(writer);
        }

        public static ComplexStyles Import(string defaultFolder)
        {
            var dlg = new OpenFileDialog
            {
                FileName = "Style.sty",
                DefaultExt = "sty",
                Filter = "Stildatei|*.sty",
                InitialDirectory = defaultFolder,
                CheckFileExists = true
            };
            if (dlg.ShowDialog() == false) return null;

            using (var reader = new StreamReader(dlg.FileName))
            {
                var lizer = XmlSerializer.FromTypes(new[] { typeof(ComplexStyles) })[0];
                var complexStyles = (ComplexStyles) lizer.Deserialize(reader);

                if (complexStyles != null)
                {
                    complexStyles.CheckAndUpdateFonts();
                }

                return complexStyles;
            }
        }

        public void CheckAndUpdateFonts()
        {
            foreach (var complexStyle in Styles)
            {
                complexStyle.FontFamily = FlowDocumentExtensions.GetFontFamily(complexStyle.FontFamily, new ObservableCollection<FontFamily>(Utilities.PossibleFonts.Select(elem=>elem.Family)));
            }
        }

    }

}
