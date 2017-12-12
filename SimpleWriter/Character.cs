using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace ComplexWriter
{
    [Serializable]
    public class Character : INotifyPropertyChanged
    {
        private string _name=string.Empty;
        private string _description = string.Empty;
        private NameType _type;
        private bool _isMain;
        private BitmapSource _image;

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.Equals(_name, value)) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        [XmlElement]
        public string Description
        {
            get { return _description; }
            set
            {
                if (string.Equals(_description, value)) return;

                _description = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public NameType Type
        {
            get { return _type; }
            set
            {
                if (_type.Equals(value)) return;

                _type = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public bool IsMain
        {
            get { return _isMain; }
            set
            {
                if (_isMain.Equals(value)) return;
                _isMain = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public BitmapSource Image
        {
            get { return _image; }
            private set
            {
                if (_image?.Equals(value) ?? false) return;

                _image = value;
                OnPropertyChanged();
            }
        }

        [XmlElement("Image")]
        public byte[] ImageBuffer
        {
            get
            {
                byte[] imageBuffer;

                if (Image == null) return null;
                using (var stream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder(); // or some other encoder
                    encoder.Frames.Add(BitmapFrame.Create(Image));
                    encoder.Save(stream);
                    imageBuffer = stream.ToArray();
                }

                return imageBuffer;
            }
            set
            {
                if (value == null)
                {
                    Image = null;
                }
                else
                {
                    using (var stream = new MemoryStream(value))
                    {
                        var decoder = BitmapDecoder.Create(stream,
                            BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        Image = decoder.Frames[0];
                    }
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static string ToXmlString(ObservableCollection<Character> characters)
        {
            if (characters == null || !characters.Any()) return string.Empty;
            var serializer = new XmlSerializer(typeof(List<Character>));

            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, characters.ToList());
                return textWriter.ToString();
            }
        }

        public static ObservableCollection<Character> FromXmlString(string str)
        {
            if(string.IsNullOrEmpty(str)) return  new ObservableCollection<Character>();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Character>));
            StringReader textReader = new StringReader(str);
            var chr =(List<Character>)xmlSerializer.Deserialize(textReader);
            return new ObservableCollection<Character>(chr);
        }

        public Character Clone()
        {
            return (Character)MemberwiseClone();
        }
    }
}