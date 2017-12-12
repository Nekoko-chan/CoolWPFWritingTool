using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using ComplexWriter.Properties;
using ExtensionObjects;


namespace ComplexWriter
{
    [Serializable]
    public class TextFile : INotifyPropertyChanged, ISerializable, ICloneable
    {
        private FlowDocument _document;
        private string _filepath=string.Empty;
        private bool _isChanged;

        public bool IsExtended
        {
            get { return Filepath == null || Filepath.EndsWith(".etf"); }
            set { OnPropertyChanged(); }
        }
        private string _lang= "de-DE";
        public string Language { get { return _lang; } set { _lang = value; } }

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value;OnPropertyChanged(); }
        }

        public ObservableCollection<Color> Colors
        {
            get { return _colors; }
            set { _colors = value;OnPropertyChanged(); }
        }

        public Watermark Watermark
        {
            get { return _watermark; }
            set { _watermark = value;OnPropertyChanged(); }
        }

        public Brush PrintBackground
        {
            get { return _printBackground; }
            set { _printBackground = value;
                PrintBackgroundString = value== null ? string.Empty:XamlWriter.Save(value); OnPropertyChanged(); }
        }

        public string PrintBackgroundString { get; set; }

        public ButtonState QuotationButtonState
        {
            get { return _quotationButtonState; }
            set { _quotationButtonState = value; }
        }

        public int CaretOffset
        {
            get { return _caretOffset; }
            set
            {
                if (_caretOffset.Equals(value)|| value<0) return;

                IsChanged = !ReadOnly;
                _caretOffset = value; 
                OnPropertyChanged();
            }
        }

        public double ScrollOffset
        {
            get { return _scrollOffset; }
            set
            {
                if (_scrollOffset.Equals(value) || value < 0) return;

                IsChanged = !ReadOnly;
                _scrollOffset = value;
                OnPropertyChanged();
            }
        }

        [OptionalField]
        private ObservableCollection<Color> _colors;

        private Watermark _watermark;
        [OptionalField]
        private PageCountElement _pageCountElement;

        [OptionalField]
        private ComplexStyles _styles;

        //[OptionalField]
        //private ComplexStyle _defaultStyle;

        [OptionalField]
        private Brush _printBackground;

        [OptionalField]
        private string _password;

        [OptionalField]
        private string _passwordQuestion;

        [OptionalField]
        private bool _spellCheckEnabled;

        [OptionalField]
        private int _incFontWhenPrinting;

        [OptionalField]
        private ButtonState _quotationButtonState;

        [OptionalField]
        private ObservableCollection<string> _characterNames;


        [OptionalField]
        private string _openingQuote;
        [OptionalField]
        private string _closingQuote;
        [OptionalField]
        private string _singleOpeningQuote;
        [OptionalField]
        private string _singleClosingQuote;
        [OptionalField]
        private bool _useBlackAndWhite;
        [OptionalField]
        private bool _showPageNumber;
        [OptionalField]
        private bool _useWatermark;
        [OptionalField]
        private bool _useOldNumbering;
        [OptionalField]
        private int _caretOffset;

        [NonSerialized] private bool _readOnly;
        [OptionalField]
        private double _scrollOffset;

        [NonSerialized]
        private bool _isWriteable=true;

        public bool UseBlackAndWhite
        {
            get { return _useBlackAndWhite; }
            set { _useBlackAndWhite = value; OnPropertyChanged(); }
        }

        public bool ShowPageNumber
        {
            get { return _showPageNumber; }
            set { _showPageNumber = value; OnPropertyChanged(); }
        }

        public bool UseWatermark
        {
            get { return _useWatermark; }
            set { _useWatermark = value; OnPropertyChanged(); }
        }

        public bool UseOldNumbering
        {
            get { return _useOldNumbering; }
            set { _useOldNumbering = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> CharacterNames
        {
            get { return _characterNames; }
            set { _characterNames = value; OnPropertyChanged(); }
        }

        [NonSerialized]
        ObservableCollection<Character> _characters;

        public ObservableCollection<Character> Characters
        {
            get { return _characters; }
            set
            {

                if (_characters!= null)
                    _characters.CollectionChanged -= CharactersCollectionChanged;
                _characters = value;
                if (_characters != null)
                {
                    foreach (var character in _characters)
                    {
                        character.PropertyChanged += NameItemChangedProperty;
                    }
                    _characters.CollectionChanged += CharactersCollectionChanged;
                }
                OnPropertyChanged();
            }
        }


        public String Password
        {
            get { return _password; }
            set { _password = value;OnPropertyChanged(); }
        }

        public string OpeningQuote
        {
            get { return _openingQuote; }
            set { _openingQuote = value; OnPropertyChanged(); }
        }

        public string ClosingQuote
        {
            get { return _closingQuote; }
            set { _closingQuote = value; OnPropertyChanged(); }
        }

        public string SingleOpeningQuote
        {
            get { return _singleOpeningQuote; }
            set { _singleOpeningQuote = value; OnPropertyChanged(); }
        }

        public string SingleClosingQuote
        {
            get { return _singleClosingQuote; }
            set { _singleClosingQuote = value; OnPropertyChanged(); }
        }

        public string PasswordQuestion
        {
            get { return _passwordQuestion; }
            set { _passwordQuestion = value; }
        }

        public bool SpellCheckEnabled
        {
            get
            {
                return _spellCheckEnabled;
            }
            set
            {
                if (_spellCheckEnabled == value) return;
                
                _spellCheckEnabled = value;
                IsChanged = true;
                OnPropertyChanged();
            }
        }
        
        public ComplexStyles Styles
        {
            get { return _styles; }
            set
            {
                if (_styles != null)
                    Styles.PropertyChanged -= StylesPropertyChanged;
                _styles = value;
                if (_styles != null)
                    Styles.PropertyChanged += StylesPropertyChanged;
                OnPropertyChanged();
            }
        }

        public ComplexStyle DefaultStyle
        {
            get { return Styles.Styles.FirstOrDefault(elem => elem.IsDefault); }
            //set
            //{
            //    _defaultStyle = value;
            //    Document.FontFamily = _defaultStyle.FontFamily;
            //    OnPropertyChanged();
            //}
        }

        void StylesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsChanged = true;
        }

        public TextFile(SerializationInfo info, StreamingContext context)
        {
            foreach (var entry in info)
            {
                switch (entry.Name)
                {
                    case "FlowDocument": LoadDocument(info);break;
                    case "Watermark": LoadWatermark(info);break;
                    case "Colors": LoadColor(info);break;
                    case "FontFamilyName": LoadPageCount(info);break;
                    case "ComplexStyles": LoadComplexStyle(info);break;
                    case "PrintBackgroundBrush": LoadBrush(info);break;
                    case "Password": LoadPassword(info);break;
                    //case "DefaultStyle": LoadDefaultStyle(info);break;
                    case "SpellCheckEnabled": LoadSpellcheck(info);break;
                    case "IncFontWhenPrinting": LoadIncFont(info);break;
                    case "QuotationButtonState": LoadQuotationState(info);break;
                    case "CaretOffset": LoadCartOffset(info);break;
                    case "ScrollOffset": LoadScrollOffset(info);break;
                    case "ReadOnly": LoadReadOnly(info);break;
                    case "Language": LoadLanguage(info);break;
                    case "Characters":LoadCharactersNew(info);break;
                    case "CharacterNames": LoadCharacters(info);break;
                    case "OpeningQuote": LoadQuoutes(info);break;
                    case "UseBlackAndWhite": LoadPrintBools(info);break;
                    case "UseCharacters":
                        LoadCharacterUse(info);
                        break;
                }
            }
        }

        private bool _charactersLoaded;

        private void LoadCharactersNew(SerializationInfo info)
        {
            try
            {
                var str = info.GetString("Characters");
                Characters = Character.FromXmlString(str);
                _charactersLoaded = true;
            }
            catch
            {
                _charactersLoaded = false;
            }
        }

        private void LoadCharacterUse(SerializationInfo info)
        {
            try
            {
                UseCharacters = info.GetBoolean("UseCharacters");
            }
            catch
            {
                UseCharacters = false;
            }
        }

        private void LoadLanguage(SerializationInfo info)
        {
            try
            {
                Language = info.GetString("Language");
            }
            catch
            {
                Language = "en-US";
            }
        }

        private void LoadReadOnly(SerializationInfo info)
        {
            try
            {
                ReadOnly = info.GetBoolean("ReadOnly");
            }
            catch
            {
                ReadOnly = false;
            }
        }

        private void LoadScrollOffset(SerializationInfo info)
        {
            try
            {
                ScrollOffset = (double) info.GetValue("ScrollOffset", typeof (double));
            }
            catch
            {
                ScrollOffset = 0;
            }
        }

        private void LoadCartOffset(SerializationInfo info)
        {
            try
            {
                CaretOffset = (int) info.GetValue("CaretOffset", typeof (int));
            }
            catch
            {
                CaretOffset = 0;
            }
        }

        private void LoadQuotationState(SerializationInfo info)
        {
            try
            {
                QuotationButtonState = (ButtonState) info.GetValue("QuotationButtonState", typeof (ButtonState));
            }
            catch
            {
                QuotationButtonState = Settings.Default.QuoteStyle;
            }
        }

        private void LoadIncFont(SerializationInfo info)
        {
            try
            {
                IncFontWhenPrinting = (int) info.GetValue("IncFontWhenPrinting", typeof (int));
            }
            catch
            {
                IncFontWhenPrinting = 0;
            }
        }

        private void LoadSpellcheck(SerializationInfo info)
        {
            try
            {
                SpellCheckEnabled = info.GetBoolean("SpellCheckEnabled");
            }
            catch
            {
                SpellCheckEnabled = true;
            }
        }

        private void LoadCharacters(SerializationInfo info)
        {
            if (_charactersLoaded) return;
            try
            {
                var names = (List<string>)info.GetValue("CharacterNames", typeof(List<string>));

                string[] descriptions;
                try
                {
                    descriptions = ((List<string>)info.GetValue("CharacterDescriptions", typeof(List<string>))).ToArray();
                }
                catch
                {
                    descriptions = new string[names.Count];
                }

              

                var charas = new ObservableCollection<Character>();

                for (int i = 0; i < names.Count; i++)
                {
                    charas.Add(new Character {Name = names[i], Description= descriptions[i] });
                }


                Characters = new ObservableCollection<Character>(charas);
                foreach (var elem in Characters)
                {
                    elem.PropertyChanged += NameItemChangedProperty;
                }
            }
            catch
            {
                Characters = new ObservableCollection<Character>();
            }
            Characters.CollectionChanged += CharactersCollectionChanged;
        }

        private void LoadPrintBools(SerializationInfo info)
        {
            try
            {
                UseBlackAndWhite = info.GetBoolean("UseBlackAndWhite");
                UseOldNumbering = info.GetBoolean("UseOldNumbering");
                UseWatermark = info.GetBoolean("UseWatermark");
                ShowPageNumber = info.GetBoolean("ShowPageNumber");
            }
            catch
            {
                UseBlackAndWhite = false;
                UseWatermark = true;
                UseOldNumbering = false;
                ShowPageNumber = true;
            }
        }

        private void LoadQuoutes(SerializationInfo info)
        {
            try
            {
                OpeningQuote = info.GetString("OpeningQuote");
                ClosingQuote = info.GetString("ClosingQuote");
                SingleOpeningQuote = info.GetString("SingleOpeningQuote");
                SingleClosingQuote = info.GetString("SingleClosingQuote");
            }
            catch
            {
                OpeningQuote = Utilities.GetOpeningQuotation(QuotationButtonState);
                ClosingQuote = Utilities.GetClosingQuotation(QuotationButtonState);
                SingleOpeningQuote = Utilities.GetSingleOpeningQuotation(QuotationButtonState);
                SingleClosingQuote = Utilities.GetSingleClosingQuotation(QuotationButtonState);
                
            }
        }


        private void LoadPassword(SerializationInfo info)
        {
            try
            {
                Password = info.GetString("Password");
            }
            catch
            {
                // ignored
            }
            try
            {
                PasswordQuestion = info.GetString("PasswordQuestion");
            }
            catch
            {
                // ignored
            }
        }


        public const string COMPLEX_STYLE_TITLE = "Aktuelles Dokument";

        //private void LoadDefaultStyle(SerializationInfo info)
        //{
        //    try
        //    {
        //        var styleStr = info.GetString("DefaultStyle");

        //        DefaultStyle = new ComplexStyleConverter().ConvertFrom(styleStr) as ComplexStyle ?? new ComplexStyle();

        //    }
        //    catch {DefaultStyle = new ComplexStyle(); }
        //}

        private void LoadComplexStyle(SerializationInfo info)
        {
            try
            {
                var styleStr = info.GetString("ComplexStyles");

                Styles = new ComplexStylesConverter().ConvertFrom(styleStr) as ComplexStyles ?? new ComplexStyles();
                Styles.Title = COMPLEX_STYLE_TITLE;
                }
            catch
            {
                Styles = new ComplexStyles {Title = COMPLEX_STYLE_TITLE};
            }
        }

        private void LoadPageCount(SerializationInfo info)
        {
            try
            {
                PageCountElement = new PageCountElement();
                var name = info.GetString("FontFamilyName");

                PageCountElement.FontFamily =
                    Utilities.PossibleFonts.FirstOrDefault(f => f.Family.FamilyNames.Any(fn => fn.Value.Equals(name)))?.Family;

                var brush = info.GetString("ResultBrush");
                PageCountElement.ForgroundBrush = (Brush)XamlReader.Parse(brush);

                PageCountElement.UseLeadingZero= info.GetBoolean("UseZero");
                PageCountElement.FontSize = info.GetDouble("FontSize");

            }
            catch
            {
                PageCountElement = new PageCountElement{FontFamily = Utilities.PossibleFonts[0].Family, ForgroundBrush = Brushes.Black,FontSize = 15};
            }

        }

        private void LoadColor(SerializationInfo info)
        {
            try
            {
                Colors = new ObservableCollection<Color>();
                var str = info.GetString("Colors");

                if (string.IsNullOrEmpty(str)) return;

                foreach (var s in str.Split("|".ToCharArray(),StringSplitOptions.RemoveEmptyEntries))
                {
                    var convertFromString = ColorConverter.ConvertFromString(s);
                    if(convertFromString!= null)
                        Colors.Add((Color)convertFromString);
                }
            }
            catch (Exception)
            {
                Colors = new ObservableCollection<Color>();
            }
        }

       

        private void LoadWatermark(SerializationInfo info)
        {
            try
            {
                Watermark  =new Watermark();
                var arr = (byte[])info.GetValue("Watermark", typeof(byte[]));

                if (arr == null)
                    return;

                Watermark.ImageSource =Utilities.GetImageSourceFromByteArray(arr);
                Watermark.Size = (Size)info.GetValue("WatermarkSize", typeof(Size));
                Watermark.Opacity = info.GetDouble("WatermarkOpacity");

            }
            catch (Exception)
            {
                Watermark = new Watermark{Size = new Size(1,1),Opacity = 1d};
            }
        }

       

        //private void LoadCaretPosition(SerializationInfo info)
        //{
        //    try
        //    {
        //        var offset = (int) info.GetValue("CaretOffset", typeof (int));
        //        CaretPosition = Document.ContentStart.GetPositionAtOffset(offset);
        //    }
        //    catch
        //    {
        //        CaretPosition = null;
        //    }
        //}

        private void LoadDocument(SerializationInfo info)
        {
            try
            {
                var arr = (byte[]) info.GetValue("FlowDocument", typeof (byte[]));

              
                using (var stream = new MemoryStream(arr))
                {
                    try
                    {
                        Document = (FlowDocument)XamlReader.Load(stream); // Für die alten Daten
                    }
                    catch
                    {
                        Document = new FlowDocument();
                        var ran = new TextRange(Document.ContentStart, Document.ContentEnd);


                        ran.Load(stream, DataFormats.XamlPackage);
                    }
                }

            }
            catch 
            {
                Document = new FlowDocument();
            }
        }

        public TextFile()
        {
            //ScrollOffset = -1d;
            IncFontWhenPrinting = 0;
            Watermark=new Watermark{Size = new Size(1,1),Opacity = 1d};
            PageCountElement = new PageCountElement
            {
                FontFamily = MainWindow.Global.FindResource("defaultFont") as FontFamily,
                ForgroundBrush = Brushes.Black,
                UseLeadingZero = false
            };
            SpellCheckEnabled = true;
            Styles = new ComplexStyles();
            OpeningQuote = ClosingQuote = "\u0022";
            SingleOpeningQuote = SingleClosingQuote = "\u0027";
            ReadOnly = false;
            Characters = new ObservableCollection<Character>();
            Characters.CollectionChanged += CharactersCollectionChanged;
        }

        void CharactersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Character elem in e.NewItems)
                {
                    elem.PropertyChanged += NameItemChangedProperty;
                }
                return;
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Character elem in e.OldItems)
                {
                    elem.PropertyChanged -= NameItemChangedProperty;
                }
            }
        }

        void NameItemChangedProperty(object sender, PropertyChangedEventArgs e)
        {
            IsChanged = true;
        }

        public FlowDocument Document
        {
            get { return _document; }
            set { _document = value;OnPropertyChanged(); }
        }

        public string Filepath
        {
            get { return _filepath; }
            set
            {
                if (!String.IsNullOrEmpty(_filepath)&&_filepath.Equals(value))
                    return;
                _filepath = value; 
                OnPropertyChanged();
                Display = "foo";
                IsExtended = false;
            }
        }
        
        public string Display
        {
            get
            {
                var pat = string.IsNullOrEmpty(Filepath) ? "<leer>" : Path.GetFileNameWithoutExtension(Filepath);
                var sav = Settings.Default.SaveAutomatical ? string.Empty : IsChanged ? "*" : string.Empty;
                return $"{sav}{pat}";
            }
            private set { OnPropertyChanged();}
        }

        public string IsChangeString
        {
            get { return String.Empty; }
            set { OnPropertyChanged();}
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                if(_isChanged.Equals(value)) return;

                _isChanged = value;
                OnPropertyChanged();
                IsChangeString = "foo";
                Display = "foo";
            }
        }

        public PageCountElement PageCountElement
        {
            get { return _pageCountElement; }
            set { _pageCountElement = value;OnPropertyChanged();
                IsChanged = true;
            }
        }

        public int IncFontWhenPrinting
        {
            get { return _incFontWhenPrinting; }
            set { _incFontWhenPrinting = value; OnPropertyChanged();}
        }

        public bool UseCharacters
        {
            get { return _useCharacters; }
            set { _useCharacters = value;OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public TextFile CopyMe()
        {
            return new TextFile()
            {
                Document = Document.Clone()
            };
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var elem = Clone() as TextFile;
            if (elem == null) return;
            using (var stream = new MemoryStream())
            {
                var cont = new TextRange(elem.Document.ContentStart, elem.Document.ContentEnd);
                cont.Save(stream, DataFormats.XamlPackage);
                info.AddValue("FlowDocument",stream.ToArray());
            }
            StoreColors(info);

            if(PageCountElement!= null)
            {
                if (PageCountElement.FontFamily == null)
                    PageCountElement.FontFamily = Utilities.PossibleFonts.FirstOrDefault()?.Family;

                info.AddValue("FontFamilyName", PageCountElement.FontFamily?.FamilyNames.FirstOrDefault().Value);
                info.AddValue("ResultBrush", PageCountElement.ForgroundBrushString);
                info.AddValue("UseZero", PageCountElement.UseLeadingZero);
                info.AddValue("FontSize",PageCountElement.FontSize);
            
                PageCountElement.UseLeadingZero = info.GetBoolean("UseZero");
            }


            var converter = new ComplexStylesConverter();
            string value = converter.ConvertTo(Styles, typeof(string)) as string;
            info.AddValue("ComplexStyles", value);

            //var converter2 = new ComplexStyleConverter();
            //string value2 = converter2.ConvertTo(DefaultStyle, typeof(string)) as string;
            //info.AddValue("DefaultStyle", value2);

            if (!string.IsNullOrEmpty(Password))
                info.AddValue("Password", Password);

            if (!string.IsNullOrEmpty(PasswordQuestion))
                info.AddValue("PasswordQuestion", PasswordQuestion);
            SaveBrush(info);

            info.AddValue("SpellCheckEnabled", SpellCheckEnabled);
            info.AddValue("IncFontWhenPrinting", IncFontWhenPrinting);
            info.AddValue("QuotationButtonState", QuotationButtonState);


            info.AddValue("SingleOpeningQuote", SingleOpeningQuote);
            info.AddValue("SingleClosingQuote", SingleClosingQuote);
            info.AddValue("OpeningQuote", OpeningQuote);
            info.AddValue("ClosingQuote", ClosingQuote);

            info.AddValue("UseBlackAndWhite", UseBlackAndWhite);
            info.AddValue("UseOldNumbering", UseOldNumbering);
            info.AddValue("UseWatermark", UseWatermark);
            info.AddValue("UseCharacters",UseCharacters);
            info.AddValue("ShowPageNumber", ShowPageNumber);
            info.AddValue("CaretOffset", CaretOffset);
            info.AddValue("ScrollOffset", ScrollOffset);
            info.AddValue("ReadOnly", ReadOnly);

            info.AddValue("Characters",Character.ToXmlString(Characters));
          
            info.AddValue("Language", Language);

            //Letzte Property... hier nach darf nichts mehr kommen!
            if (Watermark?.ImageSource == null) return;

            var bytes = Utilities.ConvertImageToByteArray(Watermark.ImageSource);
            info.AddValue("Watermark", bytes);

            info.AddValue("WatermarkSize", Watermark.Size);
            info.AddValue("WatermarkOpacity",Watermark.Opacity);
        }

        private void SaveBrush(SerializationInfo info)
        {
            if (PrintBackground == null) return;  // Kein Pinsel angegeben
            
            if(PrintBackground is SolidColorBrush) // Wenn es ein solider Pinsel ist
            {
                info.AddValue("PrintBackgroundBrush",PrintBackgroundString);
                return;
            }
            
            var image = PrintBackground.Clone() as ImageBrush;
            if (image == null) return;  // Wenn es auch kein ImageBrush ist, zurück

            info.AddValue("PrintImage",Utilities.ConvertImageToByteArray(image.ImageSource)); //Bild speichern
            image.ImageSource = null; // ImageSource leeren, weil der Pinsel sonst beim Laden probleme macht
            info.AddValue("PrintBackgroundBrush", PrintBackgroundString);
        }

         private void LoadBrush(SerializationInfo info)
        {
            try
            {
                var brushstring = info.GetString("PrintBackgroundBrush");
                var brush = (Brush)XamlReader.Parse(brushstring);
                if (brush == null) return;

                var imageBrush = brush as ImageBrush;
                if (imageBrush != null)
                    imageBrush.ImageSource =
                        Utilities.GetImageSourceFromByteArray((byte[]) info.GetValue("PrintImage", typeof (byte[])));

                PrintBackground = brush;
            }
            catch
            {
                // ignored
            }
        }

        private void StoreColors(SerializationInfo info)
        {
            var str = string.Empty;
            if (Colors == null) return;

            str = Colors.Aggregate(str, (current, color) => current + (color.ToHexColor() + "|"));
            info.AddValue("Colors",str);
        }
        
        private bool _useCharacters;

        public bool Save(string fileName, out bool saveas)
        {
            saveas = false;
            if (!_isWriteable) return false;
            _isWriteable = false;
            try
            {
                var flowDocument = Document;
                if (fileName.EndsWith(".etf"))
                {
                    var formater = new BinaryFormatter();
                    var r = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                    Debug.WriteLine(r.Text);

                    using (
                        var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        formater.Serialize(stream, this);
                    }
                }
                else
                    SaveFile(fileName, flowDocument);

            }
            finally
            {
                _isWriteable = true;
            }


            IsChanged = false;
            _isWriteable = true;
            return true;
        }

        public static void SaveFile(string fileName, FlowDocument flowDocument)
        {
            if (fileName.EndsWith(".rtf"))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                    textRange.Save(fs, DataFormats.Rtf);
                }
            }
            else if (fileName.EndsWith(".rtxt"))
            {
                var format = DataFormats.XamlPackage;
                var range = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                var fStream = new FileStream(fileName, FileMode.Create);
                range.Save(fStream, format);
                fStream.Close();
            }
            else
            {
                throw new Exception("Falsche Dateiendung");
            }
        }

        public static TextFile Load(string fileName)
        {
            if(!File.Exists(fileName)) return null;
            if(fileName.EndsWith(".rtxt"))
            {
                var doc = new FlowDocument();
                var format = DataFormats.XamlPackage;
                var range = new TextRange(doc.ContentStart, doc.ContentEnd) { Text = String.Empty };
                var fStream = new FileStream(fileName, FileMode.OpenOrCreate);
                range.Load(fStream, format);
                fStream.Close();

                return new TextFile
                {
                    Document = doc,
                    Filepath = fileName,
                    IsChanged = false,
                };
            }

            if(fileName.EndsWith(".etf"))
            {
                var formater = new BinaryFormatter();
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var textFile = (TextFile)formater.Deserialize(stream);
                    textFile.Filepath = fileName;
                    return textFile;
                }
            }
            return new TextFile { Filepath = fileName};
        }

        public bool CheckAndChangeFonts()
        {
            Styles?.CheckAndUpdateFonts();

            var updateBreaks = Document.UpdateBreaks();
            var checkAndChangeFonts = Document.CheckAndChangeMyFonts(new ObservableCollection<FontFamily>(Utilities.PossibleFonts.Select(elem=>elem.Family)));
            return updateBreaks|| checkAndChangeFonts;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void ClearDefaultWithout(ComplexStyle sty)
        {
            foreach (var complexStyle in Styles.Styles.Where(elem=>!ComplexStyle.Equals(elem,sty) && elem.IsDefault))
            {
                complexStyle.IsDefault = false;
            }
        }
    }
}
