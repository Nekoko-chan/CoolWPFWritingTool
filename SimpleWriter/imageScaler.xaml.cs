using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CustomControls;
using CustomControls.Annotations;
using ExtensionObjects;
using UserControl = System.Windows.Controls.UserControl;

namespace ComplexWriter
{
    /// <summary>
    /// Interaction logic for imageScaler.xaml
    /// </summary>
    public partial class ImageScaler : UserControl
    {
        public ImageScaler(string path,Size size) :this()
        {
            this.path = path;
            BitmapImage bitmap = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            bitmap.DecodePixelHeight = (int)size.Height;
            bitmap.DecodePixelWidth = (int)size.Width;
            Source = bitmap;
        }

        public ImageScaler()
        {
            InitializeComponent();
            Loaded += (s, e) => 
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = "Fant";
                itm.Tag = BitmapScalingMode.Fant;
                thecombo.Items.Add(itm);

                itm = new ComboBoxItem();
                itm.Content = Properties.Resources.HighQuality;
                itm.Tag = BitmapScalingMode.HighQuality;
                thecombo.Items.Add(itm);
                
                itm = new ComboBoxItem();
                itm.Content = "Linear";
                itm.Tag = BitmapScalingMode.Linear;
                thecombo.Items.Add(itm);

                itm = new ComboBoxItem();
                itm.Content = Properties.Resources.NextNeighbor;
                itm.Tag = BitmapScalingMode.NearestNeighbor;
                thecombo.Items.Add(itm);
                if(ShowRenderingOptions)
                    thecombo.SelectedItem = thecombo.Items[0];
            };
        }

       

        public static DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageScaler),
            new PropertyMetadata(OnImageChanged));
        [System.ComponentModel.Category("ControlSpecific")]
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(ImageScaler),
           new PropertyMetadata(Properties.Resources.DoubleClickTooltip));
        [System.ComponentModel.Category("ControlSpecific")]
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }


        public static DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double?), typeof(ImageScaler),
           new PropertyMetadata(OnImageWidthChanged));
        [System.ComponentModel.Category("ControlSpecific")]
        public double? ImageWidth
        {
            get { return (double?)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static DependencyProperty StringListProperty = DependencyProperty.Register("StringList", typeof(ImageScalerTranslations), typeof(ImageScaler),
          new PropertyMetadata(new ImageScalerTranslations(), OnStringListChanged));

       

        [System.ComponentModel.Category("ControlSpecific")]
        public ImageScalerTranslations StringList
        {
            get { return (ImageScalerTranslations)GetValue(StringListProperty); }
            set { SetValue(StringListProperty, value); }
        }

        public static DependencyProperty ImageOpacityProperty = DependencyProperty.Register("ImageOpacity", typeof(double), typeof(ImageScaler),
          new PropertyMetadata(1d));
        [System.ComponentModel.Category("ControlSpecific")]
        public double ImageOpacity
        {
            get { return (double)GetValue(ImageOpacityProperty); }
            set { SetValue(ImageOpacityProperty, value); }
        }


        public static DependencyProperty CanEditOpacityProperty = DependencyProperty.Register("CanEditOpacity", typeof(bool), typeof(ImageScaler),
         new PropertyMetadata(true));
        [System.ComponentModel.Category("ControlSpecific")]
        public bool CanEditOpacity
        {
            get { return (bool)GetValue(CanEditOpacityProperty); }
            set { SetValue(CanEditOpacityProperty, value); }
        }

        public static DependencyProperty CanDeleteFileProperty = DependencyProperty.Register("CanDeleteFile", typeof(bool), typeof(ImageScaler),
       new PropertyMetadata(true));
        [System.ComponentModel.Category("ControlSpecific")]
        public bool CanDeleteFile
        {
            get { return (bool)GetValue(CanDeleteFileProperty); }
            set { SetValue(CanDeleteFileProperty, value); }
        }

        public static DependencyProperty CanEditScalingModeProperty = DependencyProperty.Register("CanEditScalingMode", typeof(bool), typeof(ImageScaler),
        new PropertyMetadata(true));
        [System.ComponentModel.Category("ControlSpecific")]
        public bool CanEditScalingMode
        {
            get { return (bool)GetValue(CanEditScalingModeProperty); }
            set { SetValue(CanEditScalingModeProperty, value); }
        }

        public static DependencyProperty ScaleProperty = DependencyProperty.Register("ImageScale", typeof(BitmapScalingMode), typeof(ImageScaler), new PropertyMetadata(BitmapScalingMode.Fant,OnRenderingChanged));
        [System.ComponentModel.Category("ControlSpecific")]
        public BitmapScalingMode ImageScale
        {
            get { return (BitmapScalingMode)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }


        public static DependencyProperty ShowRenderingOptionsProperty = DependencyProperty.Register("ShowRenderingOptions", typeof(bool), typeof(ImageScaler), new PropertyMetadata(true, OnScaleModeChanged));
        [System.ComponentModel.Category("ControlSpecific")]
        public bool ShowRenderingOptions
        {
            get { return (bool)GetValue(ShowRenderingOptionsProperty); }
            set { SetValue(ShowRenderingOptionsProperty, value); }
        }

        public static DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double?), typeof(ImageScaler),
          new PropertyMetadata(OnImageHeightChanged));
        [System.ComponentModel.Category("ControlSpecific")]
        public double? ImageHeight
        {
            get { return (double?)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }


        public static readonly DependencyProperty ImageOptionFontFamilyProperty =
      DependencyProperty.Register("ImageOptionFontFamily", typeof(FontFamily), typeof(ImageScaler), new PropertyMetadata(new FontFamily("Fennario")));

        public FontFamily ImageOptionFontFamily
        {
            get { return (FontFamily)GetValue(ImageOptionFontFamilyProperty); }
            set { SetValue(ImageOptionFontFamilyProperty, value); }
        }

        public static readonly DependencyProperty ControlFontFamilyProperty =
     DependencyProperty.Register("ControlFontFamily", typeof(FontFamily), typeof(ImageScaler), new PropertyMetadata(new FontFamily("Miriam")));

        public FontFamily ControlFontFamily
        {
            get { return (FontFamily)GetValue(ControlFontFamilyProperty); }
            set { SetValue(ControlFontFamilyProperty, value); }
        }

        public static DependencyProperty ButtonForegroundProperty = DependencyProperty.Register("ButtonForeground", typeof(Color), typeof(ImageScaler), new PropertyMetadata(DiverseUtilities.ColorFromString("#FF93B7F8")));
        [System.ComponentModel.Category("ImageScaler")]
        public Color ButtonForeground
        {
            get { return (Color)GetValue(ButtonForegroundProperty); }
            set { SetValue(ButtonForegroundProperty, value); }
        }

         public static readonly DependencyProperty TextboxBackgroundProperty =
      DependencyProperty.Register("TextboxBackground", typeof(Brush), typeof(ImageScaler), new PropertyMetadata(Brushes.White));

        public Brush TextboxBackground
        {
            get { return (Brush)GetValue(TextboxBackgroundProperty); }
            set { SetValue(TextboxBackgroundProperty, value); }
        }

        public static readonly DependencyProperty TextboxForegroundProperty =
     DependencyProperty.Register("TextboxForeground", typeof(Brush), typeof(ImageScaler), new PropertyMetadata(Brushes.Black));

        public Brush TextboxForeground
        {
            get { return (Brush)GetValue(TextboxForegroundProperty); }
            set { SetValue(TextboxForegroundProperty, value); }
        }


        public void AddResourceDictionary(ResourceDictionary dict)
        {
            this.Resources.MergedDictionaries.Add(dict);
        }

        public static DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Color), typeof(ImageScaler), new PropertyMetadata(DiverseUtilities.ColorFromString("#FF1E4D93"),LineColorChanged));
        [System.ComponentModel.Category("ImageScaler")]
        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        private static void LineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            ImageScaler scal = (ImageScaler)d;

            Color col = (Color)e.NewValue;
            col.A = 0;

            LinearGradientBrush br = new LinearGradientBrush((Color)e.NewValue, col, new Point(0, 0), new Point(1, 1));
            scal.trenner.Stroke = br;
        }

        public bool scaleWidth = true;
        public bool scaleHeight = true;

        private Size _originalSize;


        private static void OnRenderingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            RenderOptions.SetBitmapScalingMode(((ImageScaler)d).preview, (BitmapScalingMode)e.NewValue);
        }
        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
          
            ImageScaler s = (ImageScaler)d;

            if (e.NewValue == null)
            {
                Reset(s);
                return;
            }

            Image help = new Image();
            help.Source = (ImageSource)e.NewValue;
            try
            {
                s.scaleHeight = false;
                s.scaleWidth = false;

                BitmapSource SourceData = (BitmapSource) help.Source;
                s.ImageWidth = SourceData.PixelWidth;
                s.ImageHeight = SourceData.PixelHeight;
                s._originalSize = new Size(SourceData.PixelWidth, SourceData.PixelHeight);
                s.border1.BorderBrush = s.border2.BorderBrush = Brushes.DarkGray;
                s.noimage.Visibility = Visibility.Hidden;

            }
            catch
            {
                Reset(s);
            }
            finally
            {
                s.scaleHeight = true;
                s.scaleWidth = true;
            }
        }

        private static void Reset(ImageScaler s)
        {
            s.ImageWidth = null;
            s.ImageHeight = null;
            s.border1.BorderBrush =
                s.border2.BorderBrush = new SolidColorBrush(DiverseUtilities.ColorFromString("#FF283266"));
            s.noimage.Visibility = Visibility.Visible;
        }

        private static void OnScaleModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageScaler s = (ImageScaler)d;
            s.comboBorder.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
        }
        private static void OnImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
          
            ImageScaler s = (ImageScaler)d;
            var doub = e.NewValue as double?;
            if (!doub.IsRealNumber() || !s.ImageWidth.IsRealNumber())
                return;

            if (!s.scaleWidth || !s.linkedScale)
                return;
            if (e.NewValue == null || e.OldValue == null)
                return;
            s.scaleHeight = false;
            double sizeFactor = (double)e.NewValue / (double)e.OldValue;
            double newWidth = sizeFactor * (double)s.ImageWidth;

            s.ImageWidth = newWidth;
            s.scaleHeight = true;

        }

        private static void OnStringListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageScaler s = (ImageScaler)d;
            if (e.NewValue == null) return;

            s.Watermark = ((ImageScalerTranslations) e.NewValue).DoubleClick;
        }

            private static void OnImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ImageScaler s = (ImageScaler)d;
                var doub =e.NewValue as double?;
                if (!doub.IsRealNumber() || !s.ImageHeight.IsRealNumber())
                    return;

                if (!s.scaleHeight || !s.linkedScale)
                    return;

                if (e.NewValue == null || e.OldValue == null)
                    return;
                s.scaleWidth = false;

                double sizeFactor = (double)e.NewValue / (double)e.OldValue;
                double newHeight = sizeFactor * s.ImageHeight.Value;

                s.ImageHeight = newHeight;
                s.scaleWidth= true;

            }

         
            private void ChangeRenderOptions(object sender, SelectionChangedEventArgs e)
            { 
                BitmapScalingMode mode = (BitmapScalingMode)((ComboBoxItem)e.AddedItems[0]).Tag;
                ImageScale = mode;
            }

            private string path;
            public string Path 
            {
                get { return path; }
                set {
                    path = value;
                    if (System.IO.File.Exists(value))
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                        Source = bitmap;
                    }
                }
            }


            public string InitialDirectory { get; set; }

            private void changePicture(object sender, MouseButtonEventArgs e)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = ""; // Default file name
                dlg.DefaultExt = ".jpg"; // Default file extension
                dlg.Multiselect = false;
                dlg.Filter = "Bilddateien|*.jpg;*.bmp;*.png;*.gif|JPG-Dateien|*.jpg|Windows Bitmap|*.bmp|Portable Network Graphics|*.png|Compuserve GIF|*.gif"; // Filter files by extension
                if (!string.IsNullOrEmpty(InitialDirectory))
                    dlg.InitialDirectory = InitialDirectory;
                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    InitialDirectory = System.IO.Path.GetDirectoryName(dlg.FileName);
                    path = dlg.FileName;
                    BitmapImage bitmap = new BitmapImage(new Uri(dlg.FileName, UriKind.RelativeOrAbsolute));
                    Source = bitmap;
                }
                //ToDo: Bild laden
            }

            private bool linkedScale = true;
            private void changeLinke(object sender, RoutedEventArgs e)
            {


                linkedScale = !linkedScale;

               

                ImageSource img = FindResource(linkedScale ? "link" : "linkbreak") as ImageSource;

                string path = linkedScale ? @"/CustomControls;component/Images/1291295531_link.png" : @"/CustomControls;component/Images/1291295539_link_break.png";
                Anchored.ButtonMask = img;//new BitmapImage(new Uri(path,UriKind.Relative));

                if (!linkedScale)
                    return;

                if (ImageHeight == null || ImageWidth == null)
                    return;
                scaleHeight = scaleWidth = false;

                double sizeFactor = (double)ImageWidth / _originalSize.Width;
                ImageHeight = _originalSize.Height * sizeFactor;


                scaleHeight = scaleWidth = true;

            }

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            Source = null;
        }
    }

    public class ImageScalerTranslations : INotifyPropertyChanged
    {
        private string _doubleClick;
        private string _width;
        private string _height;
        private string _transparency;
        private string _linkButton;
        private string _deleteImage;

        public string DoubleClick
        {
            get { return _doubleClick; }
            set
            {
                if (value == _doubleClick) return;
                _doubleClick = value;
                change();
            }
        }

        public string Width
        {
            get { return _width; }
            set {
                if (value == _width) return; 
                _width = value;
                change();
            }
        }

        public string Height
        {
            get { return _height; }
            set
            {
                if (value == _height) return;
                _height = value;
                change();
            }
        }

        public string Transparency
        {
            get { return _transparency; }
            set
            {
                if (value == _transparency) return;
                _transparency = value;
                change();
            }
        }

        public string LinkButton
        {
            get { return _linkButton; }
            set
            {
                if (value == _linkButton) return;
                _linkButton = value;
                change();
            }
        }

        public string DeleteImage
        {
            get { return _deleteImage; }
            set
            {
                if (value == _deleteImage) return;
                _deleteImage = value;
                change();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void change([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    /*
     Doppelklick, um ein Bild zu öffnen
Breite
Höhe
Transparenz
Verankert / Löst die Werte mit- voneinander
Bild löschen
     */
}
