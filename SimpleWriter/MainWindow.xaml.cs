using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xaml;
using ComplexWriter.Commands;
using ComplexWriter.MessageBoxes;
using ComplexWriter.Properties;
using CustomControls;
using ExtensionObjects;
using global::ComplexWriter.global;
using MessageBox = ComplexWriter.MessageBoxes.MessageBox;
using ComplexWriter.CharacterNames;
using SplashDemo;
using Xceed.Wpf.Toolkit;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Clipboard = System.Windows.Clipboard;
using Control = System.Windows.Controls.Control;
using Cursor = System.Windows.Input.Cursor;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using RichTextBox = System.Windows.Controls.RichTextBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Timer = System.Timers.Timer;
using ToolTip = System.Windows.Controls.ToolTip;
using WindowStartupLocation = System.Windows.WindowStartupLocation;
using WindowState = System.Windows.WindowState;

namespace ComplexWriter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        public const string ComplexStyleCaption = "Global";

       

        
        public static readonly DependencyProperty CurrentStyleProperty =
            DependencyProperty.Register("CurrentStyle", typeof (ComplexStyle), typeof (MainWindow),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ErrorMessagesProperty =
            DependencyProperty.Register("ErrorMessages", typeof (ObservableCollection<ErrorMessageItem>),
                typeof (MainWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentTextFile =
            DependencyProperty.Register("CurrentText", typeof (TextFile), typeof (MainWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ApplyableStylesProperty =
            DependencyProperty.Register("ApplyableStyles", typeof (ComplexStyles), typeof (MainWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty UseBlackBackgroundProperty =
            DependencyProperty.Register("UseBlackBackground", typeof (bool), typeof (MainWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty AutoCorrectElementProperty =
            DependencyProperty.Register("AutoCorrectElement", typeof (AutoCorrectElement), typeof (MainWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CountDownProperty =
            DependencyProperty.Register("CountDown", typeof (CountDown), typeof (MainWindow),
                new PropertyMetadata(new CountDown()));

        public static readonly DependencyProperty CurrentFontSizeProperty =
            DependencyProperty.Register("CurrentFontSize", typeof (double), typeof (MainWindow),
                new PropertyMetadata(12d, UpdateFontSizeBox));

        public static readonly DependencyProperty CurrentSymbolProperty =
            DependencyProperty.Register("CurrentSymbol", typeof (string), typeof (MainWindow), new PropertyMetadata(""));

        public static readonly DependencyProperty CurrentSymbolFontProperty =
            DependencyProperty.Register("CurrentSymbolFont", typeof (FontFamily), typeof (MainWindow),
                new PropertyMetadata(Utilities.PossibleFonts[0].Family));

        public static readonly DependencyProperty TextFilesProperty =
            DependencyProperty.Register("TextFiles", typeof (ObservableCollection<TextFile>), typeof (MainWindow),
                new PropertyMetadata(new ObservableCollection<TextFile>()));

        public static readonly DependencyProperty ShowStylePopupProperty =
            DependencyProperty.Register("ShowStylePopup", typeof (bool), typeof (MainWindow),
                new PropertyMetadata(false, UpdatePopupOpen));

        public static readonly DependencyProperty PopupIsAttachedToMainWindowProperty =
            DependencyProperty.Register("PopupIsAttachedToMainWindow", typeof (bool), typeof (MainWindow),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsTopMostProperty =
            DependencyProperty.Register("IsTopMost", typeof (bool), typeof (MainWindow),
                new PropertyMetadata(false, UpdateTopMost));

        public static readonly DependencyProperty ShowToolbarProperty =
            DependencyProperty.Register("ShowToolbar", typeof (bool), typeof (MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty CurrentFontFamilyProperty =
            DependencyProperty.Register("CurrentFontFamily", typeof (FontElement), typeof (MainWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty FontFamiliesProperty =
            DependencyProperty.Register("FontFamilies", typeof (ObservableCollection<FontElement>), typeof (MainWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ErrorWindowProperty =
            DependencyProperty.Register("ErrorWindow", typeof (ErrorWindow2), typeof (MainWindow),
                new PropertyMetadata(null));

        private readonly string _isOpendWithWindow = string.Empty;
        private readonly Object _savelock = new object();
        private readonly Timer _t = new Timer {Interval = TimeSpan.FromSeconds(15).TotalMilliseconds};
        private bool _disableTextChange;
        private bool _dontDateUp;
        private bool _erro;
        private Storyboard _errorBoard;
        private bool _isErrorRunning;
        private bool _isSaveRunning;
        private Storyboard _saveBoard;
        private BackgroundWorker _worker;

        public MainWindow(string fName) : this()
        {
            _isOpendWithWindow = fName;
        }

        public MainWindow()
        {
            var currentUiCulture = new CultureInfo(Settings.Default.Language);
            Thread.CurrentThread.CurrentCulture = currentUiCulture;
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;


            InitializeComponent();
            Box.Language = XmlLanguage.GetLanguage(currentUiCulture.IetfLanguageTag);

            InitTheDesigner();
            
        }

        private void InitTheDesigner()
        {
            OpeningQuotes.ItemsSource = MessageBoxes.SpecialUnicode.Quotations;
            AddToSplash(Properties.Resources.InitializedComponents);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            CustomDict = Path.Combine(path, "Microsoft\\Spelling\\de-DE\\CoolWriter.dic");
            CustomDictEnglish = Path.Combine(path, "Microsoft\\Spelling\\en-US\\CoolWriter.dic");
            NameDict = System.IO.Path.Combine(path, "Microsoft\\Spelling\\de-DE\\CoolWriterNames.dic");
            CanAddMessage = true;
            CheckForUpdate();

            var location = GetStartPocition(Settings.Default.StartPosition);

            Width = Settings.Default.LastSize.Width;
            Height = Settings.Default.LastSize.Height;
            Left = location.X;
            Top = location.Y;
            IsTopMost = Settings.Default.TopMost;
            Global = this;
            ErrorWindow = null;
            ErrorMessages = new ObservableCollection<ErrorMessageItem>();
            CountDown.Elapsed += CountDown_Elapsed;
            ApplyableStyles = null;
            AddToSplash(Properties.Resources.SetVariables);
            UiServices.InitCursor((Cursor) FindResource("waitCursor"));

            FontFamilies = Utilities.PossibleFonts;

            CurrentSymbol = Settings.Default.Symbol;
            var firstOrDefault = FontFamilies.FirstOrDefault(
                f => f.Family.FamilyNames.Any(fo => fo.Value.Equals(Settings.Default.CurrentSymbolFont)));
            CurrentFontFamily = firstOrDefault;
            AddToSplash(Properties.Resources.LoadElements);

            InitFromSettings();
            AddToSplash(Properties.Resources.LoadSettings);
            
            InitRichtextBox();
            AddToSplash(Properties.Resources.InitEditor);
            LoadOpenFiles();
            SetAutoSaveTimer();

            if (CurrentText != null)
                CurrentText.IsChanged = false;

            FileList.ItemsSource = TextFiles;
            TextFiles.CollectionChanged += TextFiles_CollectionChanged;

            if (string.IsNullOrEmpty(_isOpendWithWindow))
                ShowTextfileInView(TextFiles.LastOrDefault());

            InitDefaultFolder();
            Splasher.CloseSplash();
        }

        private Point GetStartPocition(Point startPosition)
        {
            var rect = new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);

            var x = Math.Max(Math.Min(startPosition.X, rect.Right), rect.Left);
            var y = Math.Max(Math.Min(startPosition.Y, rect.Bottom), rect.Top);
            return new Point(x,y);
        }

        public ComplexStyle CurrentStyle
        {
            get { return (ComplexStyle) GetValue(CurrentStyleProperty); }
            set { SetValue(CurrentStyleProperty, value); }
        }

        public bool LastShowPopupValue { get; set; }

        public bool ShowImagePopup
        {
            get { return (bool)GetValue(ShowImagePopupProperty); }
            set
            {
                SetValue(ShowImagePopupProperty, value);
            }
        }

        public static readonly DependencyProperty ShowImagePopupProperty =
                 DependencyProperty.Register("ShowImagePopup", typeof(bool), typeof(MainWindow),
                     new PropertyMetadata(false));

        public ObservableCollection<ErrorMessageItem> ErrorMessages
        {
            get { return (ObservableCollection<ErrorMessageItem>) GetValue(ErrorMessagesProperty); }
            set { SetValue(ErrorMessagesProperty, value); }
        }

        public static MainWindow Global { get; set; }

        public TextFile CurrentText
        {
            get { return (TextFile) GetValue(CurrentTextFile); }
            set
            {
                SetValue(CurrentTextFile, value);
            }
        }

        public static readonly DependencyProperty IsMinimizedProperty =
           DependencyProperty.Register("IsMinimized", typeof(bool), typeof(MainWindow),
               new PropertyMetadata(false));
        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set
            {
                SetValue(IsMinimizedProperty, value);
            }
        }
        public ComplexStyles ApplyableStyles
        {
            get { return (ComplexStyles) GetValue(ApplyableStylesProperty); }
            set { SetValue(ApplyableStylesProperty, value); }
        }

        public bool UseBlackBackground
        {
            get { return (bool) GetValue(UseBlackBackgroundProperty); }
            set { SetValue(UseBlackBackgroundProperty, value); }
        }

        public AutoCorrectElement AutoCorrectElement
        {
            get { return (AutoCorrectElement) GetValue(AutoCorrectElementProperty); }
            set { SetValue(AutoCorrectElementProperty, value); }
        }

        public CountDown CountDown
        {
            get { return (CountDown) GetValue(CountDownProperty); }
            set { SetValue(CountDownProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FlipImageProperty =
            DependencyProperty.Register("FlipImage", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Get or set received FlipImage
        /// </summary>
        public bool FlipImage
        {
            get { return (bool)GetValue(FlipImageProperty); }
            set { SetValue(FlipImageProperty, value); }
        }


        public double CurrentFontSize
        {
            get { return (double) GetValue(CurrentFontSizeProperty); }
            set { SetValue(CurrentFontSizeProperty, value); }
        }

        public string CurrentSymbol
        {
            get { return (string) GetValue(CurrentSymbolProperty); }
            set { SetValue(CurrentSymbolProperty, value); }
        }

        public FontFamily CurrentSymbolFont
        {
            get { return (FontFamily) GetValue(CurrentSymbolFontProperty); }
            set { SetValue(CurrentSymbolFontProperty, value); }
        }

        public ObservableCollection<TextFile> TextFiles
        {
            get { return (ObservableCollection<TextFile>) GetValue(TextFilesProperty); }
            set { SetValue(TextFilesProperty, value); }
        }

        public bool ShowStylePopup
        {
            get { return (bool) GetValue(ShowStylePopupProperty); }
            set
            {
                SetValue(ShowStylePopupProperty, value);
            }
        }

        public bool PopupIsAttachedToMainWindow
        {
            get { return (bool) GetValue(PopupIsAttachedToMainWindowProperty); }
            set { SetValue(PopupIsAttachedToMainWindowProperty, value); }
        }

        //  public Point PopupLocation { get; set; }

        public bool IsTopMost
        {
            get { return (bool) GetValue(IsTopMostProperty); }
            set { SetValue(IsTopMostProperty, value); }
        }

        public bool ShowToolbar
        {
            get { return (bool) GetValue(ShowToolbarProperty); }
            set { SetValue(ShowToolbarProperty, value); }
        }

        public FontElement CurrentFontFamily
        {
            get { return (FontElement) GetValue(CurrentFontFamilyProperty); }
            set { SetValue(CurrentFontFamilyProperty, value); }
        }

        public string CustomDictEnglish { get; set; }
        public string CustomDict { get; set; }

        public ObservableCollection<FontElement> FontFamilies
        {
            get { return (ObservableCollection<FontElement>) GetValue(FontFamiliesProperty); }
            set { SetValue(FontFamiliesProperty, value); }
        }

        public Color Black
        {
            get { return (Color) FindResource("BackColor"); }
        }

        public ErrorWindow2 ErrorWindow
        {
            get { return (ErrorWindow2) GetValue(ErrorWindowProperty); }
            set { SetValue(ErrorWindowProperty, value); }
        }

        public TextPointer Caretposition
        {
            get { return Box.Selection.Start; }
        }

        public bool CanAddMessage { get; set; }
        public bool IgnoreFocus { get; set; }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            //StylePopup2.IsOpen = false;
        }

        private void CountDown_Elapsed(object sender, EventArgs e)
        {
        }

        private static void CheckForUpdate()
        {
            if (!Settings.Default.Upgraded)
            {
                Settings.Default.Upgrade();
                Settings.Default.Upgraded = true;
                Settings.Default.Save();
            }
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!Settings.Default.SaveAutomatical) return;
                UpdateCurrentText();

                var observableCollection =
                    TextFiles.Where(te => !string.IsNullOrEmpty(te.Filepath) && (te.IsChanged || te.ReadOnly));
                if (!observableCollection.Any()) return;

                _worker.RunWorkerAsync(observableCollection);
                NotifySave();
            });
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            FocusTextbox();
            //StylePopup2.IsOpen = ShowStylePopup;
        }

        private static void UpdateFontSizeBox(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindow = (MainWindow) d;
            mainWindow.SetFontSize((double) e.NewValue);
        }

        private static void UpdatePopupOpen(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var main = d as MainWindow;
            if (main == null) return;

            //main.StylePopup2.IsOpen = (bool) e.NewValue && main.IsActive;
            if (!Settings.Default.ShowFontStylePopup.Equals(e.NewValue))
            {
                Settings.Default.ShowFontStylePopup = (bool) e.NewValue;
                Settings.Default.Save();
            }
        }

        private static void UpdateTopMost(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var main = d as MainWindow;
            if (main == null)
                return;

            main.Topmost = (bool) e.NewValue;
        }


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowSaveFileProperty =
            DependencyProperty.Register("ShowSaveFile", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Get or set received ShowSaveFile
        /// </summary>
        public bool ShowSaveFile
        {
            get { return (bool)GetValue(ShowSaveFileProperty); }
            set { SetValue(ShowSaveFileProperty, value); }
        }

        private void AddToSplash(string str)
        {
            MessageListener.Instance.ReceiveMessage(str);
        }


        private void InitDefaultFolder()
        {
            if (string.IsNullOrEmpty(Settings.Default.DefaultFolder))
            {
                var defaultFolder = TextFiles.FirstOrDefault();
                if (defaultFolder != null && !string.IsNullOrEmpty(defaultFolder.Filepath))
                {
                    Settings.Default.DefaultFolder = Path.GetDirectoryName(defaultFolder.Filepath);
                }
            }
        }

        private void InitRichtextBox()
        {
            Box.AddHandler(ContextMenuOpeningEvent, new ContextMenuEventHandler(RichTextBox_ContextMenuOpening), true);
            if (Settings.Default.Styles == null)
                Settings.Default.Styles = new ComplexStyles();
            ApplyableStyles = Settings.Default.Styles;
            ApplyableStyles.Title = ComplexStyleCaption;
        }

        private void SetAutoSaveTimer()
        {
            _t.Elapsed += t_Elapsed;
            _t.Enabled = Settings.Default.SaveAutomatical;

            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (_erro)
                {
                    _erro = false;
                    return;
                }
                foreach (var textFile in TextFiles)
                {
                    textFile.IsChanged = false;
                }
            });
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateCurrentText();
            try
            {
                SaveAllAutomatic(e.Argument as IEnumerable<TextFile>);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
            NotifySave();
        }

        private void InitFromSettings()
        {
            var firstOrDefault =
                Utilities.PossibleFonts.FirstOrDefault(
                    elem => elem.Family.FamilyNames.Any(el => el.Value.Equals(Settings.Default.LastFontName))) ??
                Utilities.PossibleFonts.FirstOrDefault();

            CurrentFontFamily = firstOrDefault;
            CurrentFontSize = Settings.Default.LastFontSize;
            SetFontSize(CurrentFontSize, true);
            if (Settings.Default.FontStylePopupLocation == null)
            {
                Settings.Default.FontStylePopupLocation = new Point(Left - 280, Top + Height - 70);
                Settings.Default.Save();
            }

            if (Settings.Default.FontStylePopupLocation != null)
            {
                StylePopup2.HorizontalOffset = Settings.Default.FontStylePopupLocation.Value.X;
                StylePopup2.VerticalOffset = Settings.Default.FontStylePopupLocation.Value.Y;
            }

            PopupIsAttachedToMainWindow = Settings.Default.FontStylePopupIsAttached;
            ShowStylePopup = Settings.Default.ShowFontStylePopup;
            UseBlackBackground = Settings.Default.UseBlackBackground;
            ShowToolbar = Settings.Default.ShowToolbar;
            ShowNameList = Settings.Default.ShowNames;
            ShowSaveFile = !Settings.Default.SaveAutomatical;

            ShowImagePopup = Settings.Default.Watermark != null && Settings.Default.Watermark.Any();
            if (ShowImagePopup)
            {
                try
                {
                    imageForPopup.Source = Utilities.GetImageSourceFromByteArray(Settings.Default.Watermark);

                    ImagePopup.VerticalOffset = Settings.Default.ImagePopupLocation?.Y ?? 0d;
                    ImagePopup.HorizontalOffset = Settings.Default.ImagePopupLocation?.X ?? 0d;
                    imageForPopup.Opacity = Settings.Default.WatermarkOpacity;
                    ImagePopup.Width = imageForPopup.Width=Settings.Default.WatermarkSize.Width;
                    ImagePopup.Height = imageForPopup.Height =Settings.Default.WatermarkSize.Height;
                    Settings.Default.WatermarkSize = new Size(ImagePopup.Width, ImagePopup.Height);
                    FlipImage = Settings.Default.FlipImage;
                }
                catch
                {
                    ShowImagePopup = false;
                }
            }
        }

        private void TextFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            var text = e.NewItems[0] as TextFile;
            ShowTextfileInView(text);
        }

        private void ShowTextfileInView(TextFile text)
        {
            if (text == null)
                return;
            var test = FileList.ItemContainerGenerator.ContainerFromItem(text) as ContentPresenter;
            if (test != null)
                test.BringIntoView();
        }

        private void LoadOpenFiles()
        {
            List<string> opens;

            if (Settings.Default.OpenFilenames == null || Settings.Default.OpenFilenames.OpenFileNames == null)
                opens = new List<string>();
            else
                opens = Settings.Default.OpenFilenames.OpenFileNames.Where(elem => !string.IsNullOrEmpty(elem))
                    .Concat(new List<string>(new[] {_isOpendWithWindow})).Distinct().ToList();

            if (opens.All(f => !File.Exists(f)))
            {
                opens.Add(string.Empty);
            }

            var enumerable = opens.Where(File.Exists);
            var max = enumerable.Count();
            var counter = 1;
            foreach (var open in enumerable)
            {
                AddToSplash(string.Format(Properties.Resources.FileIsLoadedLarge,counter,max,Path.GetFileName(open)));
                counter++;
                TextFile file;
                var loaded = LoadFile(open, out file);
                if (!loaded && opens.Count == 1)
                {
                    file = GetNewTextFIle();
                }

                if (file == null)
                    continue;
                TextFiles.Add(file);
                ShowFile(file);
                file.IsChanged = false;

            }

            if (!string.IsNullOrEmpty(_isOpendWithWindow))
            {
                var argFile =
                    TextFiles.FirstOrDefault(tf => tf.Filepath != null && tf.Filepath.Equals(_isOpendWithWindow));
                if (argFile != null)
                {
                    ShowFile(argFile);
                    argFile.IsChanged = false;
                }
            }
            if (TextFiles.Any()) return;

            MakeTheFile();
        }



        public void LoadAndShowFile(string filename)
        {
            TextFile file;
            var loaded = LoadFile(filename, out file);
            if (!loaded)
            {
                var first = TextFiles.FirstOrDefault(el => el.Filepath != null && el.Filepath.Equals(filename));
                if (first == null) return;
                var saved = first.IsChanged;
                ShowFile(first);
                first.IsChanged = saved;
                return;
            }
            if (!CurrentText.IsChanged && string.IsNullOrEmpty(CurrentText.Filepath))
            {
                var index = TextFiles.IndexOf(CurrentText);
                TextFiles.RemoveAt(index);
                TextFiles.Insert(index, file);
                ShowFile(file);
            }
            else
            {
                TextFiles.Add(file);
                ShowFile(file);
            }
            file.IsChanged = false;
        }

        private TextFile GetNewTextFIle()
        {
            var files = new TextFile {Document = new FlowDocument()};
            files.CaretOffset = 0;
            files.ScrollOffset = 0;
            SetStartFont(CurrentFontSize, CurrentFontFamily == null ? null : CurrentFontFamily.Family, files.Document);

            return files;
        }

        private void ShowFile(TextFile file)
        {
            try
            {
                SearchAndReplaceControl.Hide();
                _dontDateUp = true;
                if (file == null)
                    return;

                if (CurrentText != null)
                {
                    CurrentText.Document = Box.Document;
                    CurrentText.CaretOffset = Box.Document.ContentStart.GetOffsetToPosition(Box.CaretPosition);
                }
                CurrentText = file;
                Box.Document = CurrentText.Document;

                //Box.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CurrentText.Language);

                if (!CurrentText.ReadOnly)
                    Box.CaretPosition =
                        Box.Document.ContentStart.GetPositionAtOffset(Math.Max(0, CurrentText.CaretOffset));

                if (!CurrentText.ReadOnly)
                {
                    if (Box.CaretPosition == null) return;
                    Box.Selection.Select(Box.CaretPosition, Box.CaretPosition);
                    //Rect r = Box.CaretPosition.GetCharacterRect(LogicalDirection.Backward);
                    //Box.ScrollToVerticalOffset(r.Y);
                }
                else
                {
                    if (CurrentText.ScrollOffset.Equals(-1d))
                        Box.ScrollToEnd();
                    Box.ScrollToVerticalOffset(CurrentText.ScrollOffset);
                }
                if (CurrentText.DefaultStyle != null)
                    Box.FontFamily = CurrentText.DefaultStyle.FontFamily;
            //InputLanguageManager.SetInputLanguage(Box, CultureInfo.CreateSpecificCulture(CurrentText.Language));

            }
            catch
            {
            }
            finally
            {
                _dontDateUp = false;
            }
        }


        private void RichTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var standard = GetStandardCommands();
            var spell = GetSpellingSuggestions();
            var other = GetOtherCommands();
            ContextMenu.Items.Clear();

            ContextMenu.Items.Add(BuildSeperator(Properties.Resources.StandardCommands));

            foreach (var menuItem in standard)
            {
                ContextMenu.Items.Add(menuItem);
            }
            if (spell.Any())
            {
                ContextMenu.Items.Add(BuildSeperator(Properties.Resources.CorrectText));
                foreach (var menuItem in spell)
                {
                    ContextMenu.Items.Add(menuItem);
                }
            }

            if(!ShowNameList)
            {
                var names = GetNameCommands(false, false);
                var names2 = GetNameCommands(true, false);
                var names3 = GetNameCommands(false, true);
                var names4 = GetNameCommands(true, true);
                if (names.Any())
                {
                    ContextMenu.Items.Add(BuildSeperator(Properties.Resources.Names));
                    var menu = new MenuItem { Header = Properties.Resources.Names, Icon = BuildIcon("IDIcon"), Style = Global.FindResource("menuItem") as Style };
                    foreach (var menuItem in names)
                    {
                        menu.Items.Add(menuItem);
                    }
                    ContextMenu.Items.Add(menu);
               
                    var menu2 = new MenuItem { Header = Properties.Resources.NameWithSpaceEnd, Icon = BuildIcon("IDIcon"),Style = Global.FindResource("menuItem") as Style };
                    foreach (var menuItem in names2)
                    {
                        menu2.Items.Add(menuItem);
                    }
                    ContextMenu.Items.Add(menu2);
                    var menu3 = new MenuItem { Header = Properties.Resources.NameWithSpaceStart, Icon = BuildIcon("IDIcon"), Style = MainWindow.Global.FindResource("menuItem") as Style };
                    foreach (var menuItem in names3)
                    {
                        menu3.Items.Add(menuItem);
                    }
                    ContextMenu.Items.Add(menu3);

                    var menu4 = new MenuItem { Header = Properties.Resources.NameWithSpaces, Icon = BuildIcon("IDIcon"), Style = MainWindow.Global.FindResource("menuItem") as Style };
                    foreach (var menuItem in names4)
                    {
                        menu4.Items.Add(menuItem);
                    }
                    ContextMenu.Items.Add(menu4);
                }
            }

            ContextMenu.Items.Add(BuildSeperator(Properties.Resources.ImportantFunctions));
            foreach (var menuItem in other)
            {
                ContextMenu.Items.Add(menuItem);
            }
        }


        public static string GetIconName(string name)
        {
            var character = Global.CurrentText.Characters.First(elem => elem.Name.Equals(name));

            return GetIconName(character.Type);
        }

        public static string GetDescriptionName(string name)
        {
            var character = Global.CurrentText.Characters.First(elem => elem.Name.Equals(name));

            return character.Description;
        }
        public static string GetIconName(NameType typ)
        {
            switch (typ)
            {
                case NameType.Other:
                    return "OtherIcon";
                case NameType.Species:
                    return "SpeciesIcon";
                case NameType.Locality:
                    return "LocalitiesIcon";
                case NameType.Support:
                    return "SideIcon";
                case NameType.Antagonist:
                    return "AntagonistIcon";
                case NameType.Protagonist:
                    return "ProtagonistIcon";
             
            }

            return string.Empty;
        }

        private IEnumerable<Control> GetOtherCommands()
        {
            var standardCommands = new List<Control>();

            var item = BuildItem(ChangeFontWithDialog, "Fonts.png", Properties.Resources.UpdateFont,"Ctrl + T");
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);
            item = item = BuildItem(EditingCommands.ToggleItalic, "Italic.png", Properties.Resources.ItalicOnOf);
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);
            item = BuildItem(EditingCommands.ToggleBold, "Bold.png", "Fett an/aus");
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);

            item = BuildItem(EditingCommands.ToggleUnderline, "Underline.png", Properties.Resources.UnderlineOnOf);
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);

            item = BuildItem(ChangesTextColor, "Textcolor.png", Properties.Resources.ChangeForeColor);
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);

            item = BuildItem(ChangesTextBackgroundColor, "bucket.png", Properties.Resources.ChangeBackColor);
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);

            item = BuildItem(InsertSpecialCharacter, "symbol.png", Properties.Resources.InsertUnicode);
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);

            item = BuildItem(InsertAnImage, "1291282236_image.png", Properties.Resources.InsertImage);
            item.IsEnabled = !CurrentText.ReadOnly;
            standardCommands.Add(item);

            item = BuildItem(UpdateSpellCheck, "spellcheck.png", Properties.Resources.SpellCheckOnOf);
            item.IsCheckable = true;
            item.IsChecked = !CurrentText.SpellCheckEnabled;
            standardCommands.Add(item);

            if (CanInsertPageBreakOnCaretPosition())
            {
                item = BuildItem(AddSectionWithPageBreak, "page.png", Properties.Resources.PageBreak);
                item.IsEnabled = !CurrentText.ReadOnly;
                standardCommands.Add(item);
            }

            item = BuildItem(delegate { AddStyleTo(CurrentText.Styles); }, null, Properties.Resources.AddSelectedStyle);

            standardCommands.Add(item);

            item = BuildItem(delegate { AddSelectedNameToDocument(); }, null, Properties.Resources.AddAsName, "Ctrl + N");

            standardCommands.Add(item);

            return standardCommands;
        }

        internal void AddSelectedNameToDocument()
        {
            var name = Box.Selection.Text;
            if (CurrentText.Characters.Any(eleme => eleme.Name.Equals(name)))
            {
                MessageBoxes.MessageBox.ShowMessage(this, string.Format(Properties.Resources.NameAlreadyExists, name), Properties.Resources.DoubleEntry);
                return;
            }

            var elem = new Character {Name = name};
            CurrentText.Characters.Add(elem);
            CharacterNameControl.SetDescription(elem, CurrentText.Characters);
        }

        public bool CanInsertPageBreakOnCaretPosition()
        {
            if (!string.IsNullOrEmpty(Box.Selection.Text))
                return false; // Methode soll nur ausführbar sein, wenn nichts selektiert wurde

            var p = Box.CaretPosition.Paragraph;
            if (p == null) return false;
            var parent = p.Parent as TextElement;
            while (parent != null)
            {
                if (parent is List)
                    return false;
                if (parent is ListItem)
                    return false;
                parent = parent.Parent as TextElement;
            }

            return true;
        }

        private void AddSectionWithPageBreak(object sender, RoutedEventArgs e)
        {
            if (!CanInsertPageBreakOnCaretPosition()) return;

            var p = Box.CaretPosition.Paragraph;
            if (p == null) return;

            var style = FindResource("pageBreakImage2") as Brush;
            var splitter = new Paragraph
            {
                Background = style, FontSize = 1d, Margin = new Thickness(0), BreakPageBefore = true, LineHeight = 16, AllowDrop = false
            };

            if (!p.Inlines.Any())
            {
                Box.Document.Blocks.InsertBefore(p, splitter);
                return;
            }

            var firstParagraph = new Paragraph();
            var secondParagraph = new Paragraph();
            try
            {
                var tp = Box.CaretPosition;
                var first = tp.GetTextInRun(LogicalDirection.Backward);
                //tp2 is at the start of this Run.
                var tp2 = tp.GetNextContextPosition(LogicalDirection.Backward);
                var useSecond = false;
                p.Inlines.ToList().Aggregate(tp2, (current, inline) => AddInlineToRightParagraph(current, inline, first, firstParagraph, secondParagraph, ref useSecond));

                Box.Document.Blocks.InsertBefore(p, firstParagraph, true);
                Box.Document.Blocks.InsertBefore(p, splitter);
                Box.Document.Blocks.InsertBefore(p, secondParagraph, true);

                Box.Document.Blocks.Remove(p);

                SetNewCaretPosition(secondParagraph, splitter);
            }
            catch (FileNotFoundException exception)
            {
                AddException(exception);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void SetNewCaretPosition(Paragraph paragraph, Block fallback)
        {
            if (!paragraph.IsEmpty())
                Box.CaretPosition = paragraph.ContentStart;
            else
            {
                var para = fallback.NextBlock;
                Box.CaretPosition = para != null ? para.ContentStart : fallback.ContentStart;
            }
        }

        private TextPointer AddInlineToRightParagraph(TextPointer tp2, Inline inline, string first, Paragraph firstParagraph, Paragraph secondParagraph, ref bool useSecond)
        {
//There can be more then one Inline inside a Paragraph. Let's find the one containing tp by comparing its start position with tp2.
            if (tp2 != null && inline.ContentStart.GetOffsetToPosition(tp2) <= 0)
            {
                //I've only considered the situation where you want to insert the image inside a Run. You must consider other scenarios to make your solution better.
                var run1 = inline as Run;
                if (run1 != null)
                {
                    var run = run1;
                    //Split the Run to two parts, insert the image between them, and delete the original one.
                    var second = run.Text.Substring(first.Length, run.Text.Length - first.Length);
                    firstParagraph.Inlines.Add(BuildRun(first, run1));
                    secondParagraph.Inlines.Add(BuildRun(second, run1));
                    useSecond = true;
                    tp2 = null;
                }
                return tp2;
            }
            if (useSecond)
                secondParagraph.Inlines.Add(inline);
            else
                firstParagraph.Inlines.Add(inline);
            return tp2;
        }

        private Run BuildRun(string text, Run referenceRun)
        {
            return new Run(text)
            {
                FontSize = referenceRun.FontSize, FontFamily = referenceRun.FontFamily, FontStyle = referenceRun.FontStyle, FontWeight = referenceRun.FontWeight, TextDecorations = referenceRun.TextDecorations, Foreground = referenceRun.Foreground, Background = referenceRun.Background
            };
        }

        private void UpdateSpellCheck(object sender, RoutedEventArgs e)
        {
            CurrentText.SpellCheckEnabled = !CurrentText.SpellCheckEnabled;
        }

        private MenuItem BuildItem(ICommand command, string iconName, string header)
        {
            return new MenuItem
            {
                Command = command, Icon = BuildIcon(iconName), Header = header, Style = FindResource("menuItem") as Style
            };
        }

        private MenuItem BuildItem(RoutedEventHandler handler, string iconName, string header, string inputGestureString="")
        {
            var buildItem = new MenuItem
            {
                Icon = BuildIcon(iconName), Header = header, Style = FindResource("menuItem") as Style,
                InputGestureText = inputGestureString
            };
            buildItem.Click += handler;
            return buildItem;
        }

        private IList<Control> GetSpellingSuggestions()
        {
            var spellingSuggestions = new List<Control>();
            var spellingError = Box.GetSpellingError(Box.CaretPosition);
            if (spellingError == null)
            {
                var addTo = BuildDictionaryViewer(false);
                spellingSuggestions.Add(addTo);

                var addTo2 = BuildDictionaryViewer(true);
                spellingSuggestions.Add(addTo2);
                return spellingSuggestions;
            }

            var spellingErrorRange = Box.GetSpellingErrorRange(Box.CaretPosition);
            var text = spellingErrorRange == null ? "" : spellingErrorRange.Text;

            var suggestions = spellingError.Suggestions.ToList();

            if (suggestions.Count() <= 15)
            {
                spellingSuggestions.AddRange(suggestions.Select(str => new MenuItem
                {
                    Header = str, Icon = BuildIcon("DictionaryEntry"), Command = EditingCommands.CorrectSpellingError, CommandParameter = str, ToolTip = BuildToolTip(string.Format(Properties.Resources.ReplaceText, text, str)), Style = FindResource("menuItem") as Style, CommandTarget = Box
                }));
            }
            else
            {
                var possibles = new MenuItem
                {
                    Header = "Mögliche Ersetzungen", Icon = BuildIcon("DictionaryEntry"), Command = EditingCommands.IgnoreSpellingError, CommandTarget = Box, Style = FindResource("menuItem") as Style
                };

                possibles.Items.AddRange(suggestions.Select(str => new MenuItem
                {
                    Header = str, Icon = BuildIcon("DictionaryEntry"), ToolTip = BuildToolTip(string.Format(Properties.Resources.ReplaceText, text, str)), Command = EditingCommands.CorrectSpellingError, CommandParameter = str, Style = FindResource("menuItem") as Style, CommandTarget = Box
                }));
                spellingSuggestions.Add(possibles);
                possibles.Items.Add(BuildSeperator(string.Empty));

                var addToDictionaryBut = BuildAddToDictionary(false);
                possibles.Items.Add(addToDictionaryBut);

                var addToNameDictionaryBut = BuildAddToDictionary(true);
                possibles.Items.Add(addToNameDictionaryBut);

                possibles.Items.Add(BuildSeperator(string.Empty));

                addToDictionaryBut = BuildDictionaryViewer(true);
                possibles.Items.Add(addToDictionaryBut);

                addToDictionaryBut = BuildDictionaryViewer(false);
                possibles.Items.Add(addToDictionaryBut);
            }

            if (suggestions.Any())
                spellingSuggestions.Add(BuildSeperator(string.Empty));

            var ignoreAll = new MenuItem
            {
                Header = Properties.Resources.IgnorenAll, Icon = BuildIcon("IgnoreEntry"), Command = EditingCommands.IgnoreSpellingError, CommandTarget = Box, ToolTip = BuildToolTip(Properties.Resources.IgnoreAllHint), Style = FindResource("menuItem") as Style
            };

            spellingSuggestions.Add(ignoreAll);

            var addToDictionary = BuildAddToDictionary(false);
            spellingSuggestions.Add(addToDictionary);

            var addToNameDictionary = BuildAddToDictionary(true);
            spellingSuggestions.Add(addToNameDictionary);

            spellingSuggestions.Add(BuildSeperator(string.Empty));

            addToDictionary = BuildDictionaryViewer(false);
            spellingSuggestions.Add(addToDictionary);

            addToDictionary = BuildDictionaryViewer(true);
            spellingSuggestions.Add(addToDictionary);

            return spellingSuggestions;
        }

        private object BuildToolTip(string str)
        {
            var tip = new ToolTip
            {
                Style = FindResource("tipTest") as Style, Content = str
            };

            return tip;
        }

        private object BuildToolTip()
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
            grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});

            var rich = new RichTextBox
            {
                Style = FindResource("tootipBox") as Style, Margin = new Thickness(2), Padding = new Thickness(5)
            };
            Grid.SetRow(rich, 1);

            var bord = new Border
            {
                BorderBrush = FindResource("BackColorBrush") as Brush, BorderThickness = new Thickness(0, 0, 5, 5), Effect = new BlurEffect {Radius = 7}
            };
            Grid.SetRow(bord, 1);

            var title = new TextBlock {Text = Properties.Resources.FileWouldBeInserted};

            grid.Children.Add(rich);
            grid.Children.Add(bord);
            grid.Children.Add(title);

            var tip = new ToolTip
            {
                Style = FindResource("tipTest") as Style, Content = grid,
            };

            return tip;
        }

        private MenuItem BuildDictionaryViewer(bool isName)
        {
            var test = isName ? Properties.Resources.OpenNameDictionaryWindow : Properties.Resources.OpenDictionary;
            var addToDictionary = new MenuItem
            {
                Header = isName ? Properties.Resources.OpenNameDictionary : Properties.Resources.OpenDictionary, Icon = BuildIcon("AddDictionary"), ToolTip = BuildToolTip(test), Style = FindResource("menuItem") as Style
            };
            if (isName)
                addToDictionary.Click += EditNameDicionary;
            else
                addToDictionary.Click += EditDicionary;

            return addToDictionary;
        }

        private MenuItem BuildAddToDictionary(bool isName)
        {
            var text = isName ? Properties.Resources.EntryIsAddedToNames : Properties.Resources.EntryIsAddedToDictionary;
            var addToDictionary = new MenuItem
            {
                Header = GetHeader(Box.CaretPosition, isName), Icon = BuildIcon("bookOpenEdit.png"), Command = EditingCommands.IgnoreSpellingError, ToolTip = BuildToolTip(text), CommandTarget = Box, Style = FindResource("menuItem") as Style
            };

            addToDictionary.Click += (o, rea) => AddToDictionary(Box.CaretPosition, isName ? NameDict : CurrentTextIsEnglish()? CustomDictEnglish: CustomDict);
            return addToDictionary;
        }

        internal bool CurrentTextIsEnglish()
        {
            return CurrentText.Language.StartsWith("en");
        }


        private object GetHeader(TextPointer pointer, bool name)
        {
            var entry = Box.GetSpellingErrorRange(pointer);
            if (entry == null)
                return Properties.Resources.EntryIsAddedToDictionary;

            var text = name ? Properties.Resources.SpecialEntryAddedToNames : Properties.Resources.SpecialEntryAddedToDictionary;
            return string.Format(text, entry.Text);
        }

        private Separator BuildSeperator(string text)
        {
            return new Separator
            {
                Tag = text, Style = FindResource(string.IsNullOrEmpty(text) ? "shortsep" : "titlesep") as Style
            };
        }

        private void AddToDictionary(TextPointer pointer, string dict)
        {
            var entry = Box.GetSpellingErrorRange(pointer);

            if (entry == null)
                return;

            if (!File.Exists(dict))
            {
                var stream = File.Create(dict);
                stream.Close();
            }

            using (var streamWriter = new StreamWriter(dict, true, Encoding.Unicode))
            {
                streamWriter.WriteLine(entry.Text);
            }
        }

        private IEnumerable<Control> GetNameCommands(bool withSpaceAfter, bool withSpaceBefore)
        {
            var commands = new List<Control>();

            foreach (var characterName in CurrentText.Characters)
            {
                var men = new MenuItem
                {
                    Tag = $"{(withSpaceBefore ? " " : string.Empty)}{characterName.Name }{(withSpaceAfter ? " " : string.Empty)}",
                    Header = characterName.Name,
                    Style = FindResource("menuItem") as Style,
                    Icon = BuildIcon(GetIconName(characterName.Type)),
                    ToolTip = BuildToolTip(characterName.Description)

                };
                men.Click += AddName;

                commands.Add(men);
            }

            return commands;
        }

        private void AddName(object sender, RoutedEventArgs e)
        {
            var itm = (MenuItem) sender;
           InsertName((string)itm.Tag);
        }

        private IEnumerable<Control> GetStandardCommands()
        {
            var standardCommands = new List<Control>();

            var item = new MenuItem
            {
                Command = ApplicationCommands.Cut, Icon = BuildIcon("CutIcon"), IsEnabled = !CurrentText.ReadOnly, Style = FindResource("menuItem") as Style
            };
            standardCommands.Add(item);

            item = new MenuItem
            {
                Command = ApplicationCommands.Copy, Icon = BuildIcon("CopyIcon"), Style = FindResource("menuItem") as Style
            };
            standardCommands.Add(item);

            item = new MenuItem
            {
                Command = ApplicationCommands.Paste, Icon = BuildIcon("PasteIcon"), ToolTip = BuildToolTip(), IsEnabled = !CurrentText.ReadOnly, Style = FindResource("menuItem") as Style
            };
            standardCommands.Add(item);

            try
            {
                if (!Clipboard.ContainsText()) return standardCommands;

                standardCommands.Add(BuildSeperator(string.Empty));

                item = new MenuItem
                {
                    Command = OwnPasteCommand.Paste, Icon = BuildIcon("PasteNonStyleIcon"), CommandParameter = Box, IsEnabled = !CurrentText.ReadOnly, Style = FindResource("menuItem") as Style, ToolTip = BuildToolTip(string.Format(Properties.Resources.PastePreview, Clipboard.GetText()))
                };
                standardCommands.Add(item);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }

            return standardCommands;
        }

        private object BuildIcon(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            if (s.IndexOf(".", StringComparison.Ordinal) == -1)
                return FindResource(s) as Brush;

            var br = Utilities.GetImageBrushFromSource(s);

            return br;
        }

        public void CloseIt()
        {
            var observableCollection = TextFiles.Where(GetUnsavedFiles).ToList();
            if (observableCollection.Any())
                try
                {
                    var allowSave = Settings.Default.SaveAutomatical ||
                                    QuestionBox.ShowMessage(this,
                                        Properties.Resources.SomeFilesChanged,
                                        Properties.Resources.SaveChanged, false) == MessageBoxResult.Yes;
                    if(allowSave)
                        if (!SaveAllFiles(observableCollection))
                        {
                            if (QuestionBox.ShowMessage(this, Properties.Resources.SaveError, Properties.Resources.CloseAnyway, false) == MessageBoxResult.No)
                                return;
                        }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            UpdateOpenFileList();

            Settings.Default.LastFontName = CurrentFontFamily == null ? null : CurrentFontFamily.Family.FamilyNames.FirstOrDefault().Value;
            Settings.Default.LastFontSize = CurrentFontSize;
            //Settings.Default.CheckSpelling = SpellCheckEnabled;
            Settings.Default.TopMost = IsTopMost;
            Settings.Default.Styles = ApplyableStyles;

            if (WindowState == WindowState.Normal)
            {
                Settings.Default.LastSize = new Size(ActualWidth, ActualHeight);
                Settings.Default.StartPosition = new Point(Left, Top);
            }
            Settings.Default.FontStylePopupLocation = new Point(StylePopup2.HorizontalOffset, StylePopup2.VerticalOffset);
            Settings.Default.FontStylePopupIsAttached = PopupIsAttachedToMainWindow;
            Settings.Default.UseBlackBackground = UseBlackBackground;
            Settings.Default.ShowToolbar = ShowToolbar;
            Settings.Default.ShowNames = ShowNameList;

            if(ShowImagePopup)
            {
                Settings.Default.ImagePopupLocation = new Point(ImagePopup.HorizontalOffset,ImagePopup.VerticalOffset);
                Settings.Default.Watermark = Utilities.ConvertImageToByteArray(imageForPopup.Source);
                Settings.Default.WatermarkOpacity = imageForPopup.Opacity;
                Settings.Default.WatermarkSize  = new Size(ImagePopup.Width, ImagePopup.Height);
                Settings.Default.FlipImage = FlipImage;

            }
            else
            {
                Settings.Default.Watermark = null;
            }
            Settings.Default.Save();

            Application.Current.Shutdown();
        }

        private void SaveImageInSettings()
        {
            if (ShowImagePopup)
            {
                Settings.Default.ImagePopupLocation = new Point(ImagePopup.HorizontalOffset, ImagePopup.VerticalOffset);
                Settings.Default.Watermark = Utilities.ConvertImageToByteArray(imageForPopup.Source);
                Settings.Default.WatermarkOpacity = imageForPopup.Opacity;
                Settings.Default.WatermarkSize = new Size(ImagePopup.Width, ImagePopup.Height);
                Settings.Default.FlipImage = FlipImage;
            }
            else
            {
                Settings.Default.Watermark = null;
            }

            Settings.Default.Save();
        }

        private static bool GetUnsavedFiles(TextFile te)
        {
            if (te.ReadOnly) return false;
            if (string.IsNullOrEmpty(te.Filepath) && string.IsNullOrEmpty(te.Document.GetText())) return false;

            return te.IsChanged;
        }

        private bool SaveAllFiles(IEnumerable<TextFile> observableCollection)
        {
            var err = false;
            var issaved = true;
            foreach (var file in observableCollection)
            {
                try
                {
                    bool saveas;
                    issaved &= file.Save(file.Filepath, out saveas);
                    if (saveas)
                        SaveAndReload(file);
                }
                catch (Exception e)
                {
                    err = true;
                    AddSaveException(e);
                }
            }
            return !err & issaved;
        }

        private void SaveAndReload(TextFile file)
        {
            SaveFileAs(file);
            ReloadFile(file);
        }

        private void CheckText(object sender, TextCompositionEventArgs e)
        {
            var check = Utilities.IsAllowedText(e.Text);
            e.Handled = !check;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            LoadFileWithDialog();
        }

        public static void SetBusy()
        {
            UiServices.SetBusyState();
        }

        private void LoadFileWithDialog()
        {
            var fname =GetFileNameWithOwnDialog();
           
            if (!fname.Any()) return;

            SetBusy();
            Settings.Default.DefaultFolder = Path.GetDirectoryName(fname.First());

            foreach (var fileName in fname)
            {
                TextFile file;
                var foo = LoadFile(fileName, out file);
                if (foo)
                {
                    if (CurrentText != null && !CurrentText.IsChanged && string.IsNullOrEmpty(CurrentText.Filepath))
                    {
                        var index = TextFiles.IndexOf(CurrentText);
                        TextFiles.RemoveAt(index);
                        TextFiles.Insert(index, file);
                        ShowFile(file);
                    }
                    else
                    {
                        TextFiles.Add(file);
                        ShowFile(file);
                    }
                    file.IsChanged = false;
                }
            }
            UpdateOpenFileList();
            Settings.Default.Save();
        }

        private IEnumerable<string> GetFileNameWithOwnDialog()
        {
            var dlg = new FileOpener{InitialDirectory = Settings.Default.InitialDirectory};
            if (dlg.ShowDialog() == true && dlg.Result == MessageBoxResult.OK && dlg.Filenames.Any())
            {
                Settings.Default.InitialDirectory = Path.GetDirectoryName(dlg.Filenames.First());
                Settings.Default.Save();
                return dlg.Filenames;
            }

            return Enumerable.Empty<string>();
        }

        private void UpdateOpenFileList()
        {
            if (Settings.Default.OpenFilenames == null)
                Settings.Default.OpenFilenames = new OpenFiles();
            Settings.Default.OpenFilenames.Clear();

            foreach (var textFile in TextFiles.Where(elem => !string.IsNullOrEmpty(elem.Filepath)))
            {
                Settings.Default.OpenFilenames.Add(textFile.Filepath);
            }
        }

        private bool LoadFile(string fileName, out TextFile file)
        {
            file = null;

            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return false;

            if (TextFiles.Any(elem => elem.Filepath != null && elem.Filepath.Equals(fileName)))
            {
                MessageBox.ShowMessage(this, string.Format(Properties.Resources.FileIsOpen, fileName), Properties.Resources.Doubleing, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                file = TextFile.Load(fileName);
                if (!CheckPassword(file))
                    return false;

                AddInformation(string.Format(Properties.Resources.FileIsOpened, Path.GetFileName(fileName)),Properties.Resources.Diverse);

                file.Document.UpdateLanguage(file.Language);
                var updated = file.CheckAndChangeFonts();

                if (updated)
                    AddInformation(string.Format(Properties.Resources.FontIsUpdated, Path.GetFileName(fileName)), Properties.Resources.Diverse);
            }
            catch (Exception exception)
            {
                AddException(exception, string.Format(Properties.Resources.LoadError, Path.GetFileName(fileName)));
                return false;
            }

            return true;
        }

        private bool CheckPassword(TextFile file)
        {
            if (!Settings.Default.AskForPassword || string.IsNullOrEmpty(file.Password))
                return true;

            var dlg = new PasswordInput
            {
                Owner = this, Filename = file.Display, CheckPassword = true, PasswordDecrpyted = file.Password, PasswordQuestion = file.PasswordQuestion
            };

            return (dlg.ShowDialog() == true && dlg.Result == MessageBoxResult.OK);
        }

        private void SaveAllAutomatic(IEnumerable<TextFile> textFiles)
        {
            Dispatcher.Invoke(() =>
            {
                _erro = false;
                foreach (var file in textFiles)
                {
                    try
                    {
                        bool saveas;
                        file.Save(file.Filepath, out saveas);
                        if (saveas)
                            SaveAndReload(file);
                    }
                    catch (Exception e)
                    {
                        _erro = true;
                        AddSaveException(e);
                    }
                }
                if (_erro == false)
                    SaveErrorProvider.Visibility = Visibility.Hidden;
            });
        }

        private bool SaveFile(TextFile currentText)
        {
            if (!currentText.IsChanged && !currentText.ReadOnly)
                return true;

            var reload = false;
            try
            {
                var filename = currentText.Filepath;

                if (string.IsNullOrEmpty(filename))
                {
                    var defaultName = GetDefaultName(Box.GetText());

                    var dlg = new SaveFileDialog
                    {
                        FileName = defaultName, DefaultExt = "etf", Filter =Properties.Resources.FileFilterLarge, InitialDirectory = Settings.Default.DefaultFolder, CheckFileExists = false
                    };
                    if (dlg.ShowDialog() == false) return true;

                    SetBusy();

                    filename = dlg.FileName;

                    if (CheckAndRestart(filename, currentText, false, out reload)) return true;

                    CurrentText.Filepath = filename;

                    if (!string.IsNullOrEmpty(filename))
                        Settings.Default.DefaultFolder = Path.GetDirectoryName(dlg.FileName);
                }
                SetBusy();

                if (string.IsNullOrEmpty(filename)) return true;

                var isSaved = false;
                try
                {
                    bool saveas;
                    isSaved = currentText.Save(filename, out saveas);
                    if (saveas)
                        SaveAndReload(currentText);
                }
                catch (Exception e)
                {
                    AddSaveException(e);
                }

                if (reload)
                {
                    TextFiles.RemoveAll(t => t.Filepath.Equals(filename));
                    LoadAndShowFile(filename);
                    NotifySave(currentText, false, filename);
                    return true;
                }

                UpdateOpenFiles(filename);

                NotifySave(currentText, false, filename);
                CurrentText.IsChanged = !isSaved;
            }
            catch (Exception exception)
            {
                AddException(exception);
                return false;
            }

            return true;
        }

        private void NotifySave()
        {
            //AnimateSaveProvider();
        }

        private void NotifySave(TextFile currentText, bool automatic, string filename)
        {
            //AnimateSaveProvider();
            var fileName = CheckRightSaveFormat(currentText, automatic, filename);
            AddMessage(automatic ? string.Format(Properties.Resources.AutomaticSavedMessage, fileName) : string.Format(Properties.Resources.FileISSaved, fileName), Properties.Resources.Saving);
        }

        private bool CheckAndRestart(string filename, TextFile currentText, bool saveAs, out bool reload)
        {
            reload = false;
            var exist = TextFiles.FirstOrDefault(t => t.Filepath != null && t.Filepath.Equals(filename));
            if (exist == null) return false;

            if (CurrentText.Filepath != null && CurrentText.Filepath.Equals(filename)) return false;

            var save = new SaveMessage {Owner = this, Filename = Path.GetFileName(filename)};
            if (save.ShowDialog() == false) return true;

            var res = save.Result;
            if (res == MessageBoxResult.Yes)
            {
                reload = true;
                return false;
            }
            if (res == MessageBoxResult.No)
            {
                if (saveAs)
                    SaveFileAs(currentText);
                else
                    SaveFile(currentText);
            }

            return true;
        }

        private string CheckRightSaveFormat(TextFile currentText, bool automatic, string filename)
        {
            var fileName = Path.GetFileName(currentText.Filepath);

            if (!automatic && filename.EndsWith(".rtxt") && currentText.Watermark != null)
            {
                MessageBox.ShowMessage(this, Properties.Resources.WatermarkOnlyWithEtf, Properties.Resources.Saving, MessageBoxImage.Information);
            }
            return fileName;
        }

        private static void UpdateOpenFiles(string filename)
        {
            if (Settings.Default.OpenFilenames == null)
                Settings.Default.OpenFilenames = new OpenFiles();
            Settings.Default.OpenFilenames.Add(filename);
            Settings.Default.Save();
        }

        private void SaveFileAs(TextFile currentText)
        {
            var reload = false;
            var helpName = currentText.Filepath;
            try
            {
                var filename = string.Empty;

                if (string.IsNullOrEmpty(filename))
                {
                    var defaultName = GetDefaultName(Box.GetText());

                    var dlg = new SaveFileDialog
                    {
                        InitialDirectory = Settings.Default.DefaultFolder, FileName = defaultName, DefaultExt = "etf", Filter = Properties.Resources.FileFilterLarge, CheckFileExists = false
                    };
                    if (dlg.ShowDialog() != false)
                    {
                        filename = dlg.FileName;
                    }
                    if (!string.IsNullOrEmpty(filename))
                    {
                        Settings.Default.DefaultFolder = Path.GetDirectoryName(dlg.FileName);
                        Settings.Default.Save();
                    }
                    if (CheckAndRestart(filename, currentText, true, out reload)) return;
                }

                if (string.IsNullOrEmpty(filename))
                    return;

                if (filename.EndsWith(".rtxt") && currentText.Watermark != null)
                {
                    var res = QuestionBox.ShowMessage(this, Properties.Resources.WatermarkNotSupportedMessage, Properties.Resources.SaveQuestion);
                    switch (res)
                    {
                        case MessageBoxResult.No:
                            SaveFileAs(currentText);
                            return;
                        case MessageBoxResult.Cancel:
                            return;
                    }
                }

                var issaved = false;
                try
                {
                    bool saveas;
                    issaved = currentText.Save(filename, out saveas);
                    if (saveas)
                        SaveAndReload(currentText);
                }
                catch (Exception e)
                {
                    AddSaveException(e);
                }

                if (Settings.Default.OpenFilenames == null)
                    Settings.Default.OpenFilenames = new OpenFiles();
                Settings.Default.OpenFilenames.Add(filename);
                Settings.Default.Save();

                AnimateSaveProvider();
                var fileName = Path.GetFileName(filename);
                AddMessage(string.Format(Properties.Resources.FileISSaved, fileName), Properties.Resources.Saving);

                if (reload)
                {
                    var remove = TextFiles.FirstOrDefault(t => t.Filepath.Equals(filename));
                    if (remove == null)
                        throw new Exception("Das darf nicht passieren!");
                    TextFiles.Remove(remove);
                }

                if (!helpName.Equals(filename))
                    LoadAndShowFile(filename);

                CurrentText.IsChanged = !issaved;
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        public static string GetDefaultName(string boxText)
        {
            try
            {
                var text = string.Empty;
                if (!string.IsNullOrEmpty(boxText))
                {
                    var index = -1;
                    for (var i = 0; i < 5; i++)
                    {
                        index = boxText.IndexOfWhitespaceOrNewline(index + 1);
                        if (index == -1)
                        {
                            text = boxText;
                            break;
                        }
                        var indexOf = boxText.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                        if (indexOf != -1 && indexOf >= index)
                            text = boxText.Substring(0, index);
                    }
                    return string.IsNullOrEmpty(text) ? Properties.Resources.UnknowTxt : BringIntoRightForm(text);
                }
            }
            catch (Exception)
            {
                return Properties.Resources.UnknowTxt;
            }
            return Properties.Resources.UnknowTxt;
        }

        private static string BringIntoRightForm(string text)
        {
            var rgx = new Regex("[^a-zA-Z0-9ÄÖÜäöüß -]");
            var bringIntoRightForm = rgx.Replace(text, "");
            bringIntoRightForm = bringIntoRightForm.ToTitleCase();

            return bringIntoRightForm.Replace(" ", "");
        }

        private void SaveCurrentFile(object sender, RoutedEventArgs e)
        {
            UpdateCurrentText();
            SaveFile(CurrentText);
        }

        private void UpdateCurrentText()
        {
            Dispatcher.Invoke(() =>
            {
                if (CurrentText != null)
                    CurrentText.Document = Box.Document;
            });
        }

        private void SaveCurrentFileAs(object sender, RoutedEventArgs e)
        {
            UpdateCurrentText();
            SaveFileAs(CurrentText);
        }

        private void Textchanged(object sender, TextChangedEventArgs e)
        {
            if (!((RichTextBox) sender).IsFocused)
                return;

            if (_disableTextChange)
            {
                _disableTextChange = false;
                return;
            }

            CurrentText.IsChanged = true;
            if (SearchAndReplaceControl.Visibility == Visibility.Visible)
            {
                SearchAndReplaceControl.FlowDocument = Box.Document;
            }
            UpdateCurrentLanguage(e);
        }

        private void UpdateCurrentLanguage(TextChangedEventArgs e)
        {
            var changeList = e.Changes.ToList();
            if (changeList.Count > 0)
            {
                foreach (var change in changeList)
                {
                    TextPointer start = null;
                    TextPointer end = null;
                    if (change.AddedLength > 0)
                    {
                        start = Box.Document.ContentStart.GetPositionAtOffset(change.Offset);
                        end = Box.Document.ContentStart.GetPositionAtOffset(change.Offset + change.AddedLength);
                    }
                    else
                    {
                        int startOffset = Math.Max(change.Offset - change.RemovedLength, 0);
                        start = Box.Document.ContentStart.GetPositionAtOffset(startOffset);
                        end = Box.Document.ContentStart.GetPositionAtOffset(change.Offset);
                    }

                    if (start != null && end != null)
                    {
                        var range = new TextRange(start, end);
                        range.ApplyPropertyValue(FrameworkElement.LanguageProperty, CurrentText.Document.Language);
                    }
                }
            }
        }

        private bool AskForSave(TextFile file = null)
        {
            UpdateCurrentText();

            if (file == null && TextFiles.All(elem => !elem.IsChanged))
                return false;

            var textFiles = file == null ? TextFiles.Where(elem => elem.IsChanged) : new List<TextFile>(new[] {file});
            foreach (var textFile in textFiles)
            {
                var result = QuestionBox.ShowMessage(this, string.Format(Properties.Resources.ChangesInFile, textFile.Display), Properties.Resources.SaveQuestion);

                if (result == MessageBoxResult.Yes)
                {
                    SaveFile(textFile);
                }
                if (result == MessageBoxResult.Cancel)
                    return true;
            }

            return false;
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb) sender).Tag as MainWindow;
            if (win == null) return;

            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;


            win.ImagePopup.VerticalOffset += e.VerticalChange;
            win.ImagePopup.HorizontalOffset += e.HorizontalChange;

            if (!win.PopupIsAttachedToMainWindow) return;

            win.StylePopup2.VerticalOffset += e.VerticalChange;
            win.StylePopup2.HorizontalOffset += e.HorizontalChange;

        }

        private void DragDelta(object sender, DragDeltaEventArgs e)
        {
            var reducedWidth = e.HorizontalChange;
            var windowRect = new Rect(Left, Top, ActualWidth, ActualHeight);
            var maxReducedWidth = Math.Max(0, ActualWidth - MinWidth);
            reducedWidth = Math.Min(reducedWidth, maxReducedWidth);
            windowRect.X += reducedWidth;
            windowRect.Width -= reducedWidth;
            this.SetWindowVisualRect(windowRect);
        }

        private void DragDelta2(object sender, DragDeltaEventArgs e)
        {
            var reducedHeight = e.VerticalChange;
            var reducedWidth = e.HorizontalChange;
            var maxReducedHeight = Math.Max(0, ActualHeight - MinHeight);
            var maxReducedWidth = Math.Max(0, ActualWidth - MinWidth);
            reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
            reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
            Width = Math.Max(ActualWidth + reducedWidth, MinWidth);
            Height = Math.Max(ActualHeight + reducedHeight, MinHeight);
        }

        private void RaiseDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var window = ((Thumb) sender).Tag as MainWindow;
            if (window == null || !window.PopupIsAttachedToMainWindow) return;
            window.ShowStylePopupIfAllowed();
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var window = ((Thumb) sender).Tag as MainWindow;
            if (window == null || !window.PopupIsAttachedToMainWindow) return;
            window.HideStylePopup();
        }

        private void HandleKeysPreview(object sender, KeyEventArgs e)
        {
            HandleKey(e);
        }

        private void HandleKey(KeyEventArgs e)
        {
            try
            {
                var inte = (int) e.Key;
                if (e.Key == Key.F2)
                {
                    var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

                    var firstOrDefault = CurrentText.DefaultStyle;
                    if (firstOrDefault != null)
                        ApplyStyle(firstOrDefault, selectionRange);
                    else
                    {
                        MessageBox.ShowMessage(this,
                            Properties.Resources.NoDefaultStyle,
                            Properties.Resources.NoDefaultStyleTitle);
                    }
                }
                if (inte >= 34 && inte <= 43 && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ConvertToSimpleQuote(inte - 34);
                    e.Handled = true;
                    return;
                }

                if (inte >= 34 && inte <= 43 && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                {
                    ConvertToSimpleQuoteTextOnly(inte - 34);
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.I && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    EditingCommands.IgnoreSpellingError.Execute(null, Box);

                if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ChangeFontWithDialog();
                    e.Handled = true;
                }

                //if (e.Key.IsSeperator())
                //{
                //    if (!CheckWordUpper())
                //        UpdatePopup();
                //}

                if (e.Key == Key.V && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    PasteTextAsTextOnly();

                if (e.Key == Key.F8)
                {
                    ConvertToSimpleQuote(false);
                    return;
                }

                if (e.Key == Key.F6)
                {
                    ConvertToSimpleQuote(true);
                    return;
                }

                if (e.Key == Key.F7)
                {
                    var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

                    var firstOrDefault = CurrentText.Styles.Styles.FirstOrDefault(elem => elem.HasCaption && IsLanguageElement(elem)) ?? ApplyableStyles.Styles.FirstOrDefault(elem => elem.HasCaption && IsLanguageElement(elem));

                    ApplyStyle(firstOrDefault, selectionRange);
                    return;
                }

                if (e.Key == Key.F3 && SearchAndReplaceControl.Visibility == Visibility.Visible)
                {
                    SearchAndReplaceControl.SearchNextElement();
                    return;
                }

                if (e.Key == Key.F4)
                {
                    ConvertToSimpleSingleQuote(false);
                }

                if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ShowSearchControl(false);
                    e.Handled = true;
                }

                if (e.Key == Key.H && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    ShowSearchControl(false, true);
                    e.Handled = true;
                }

                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    UpdateCurrentText();
                    SaveFile(CurrentText);
                    e.Handled = true;
                }

                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    UpdateCurrentText();
                    SaveAll();
                    e.Handled = true;
                }

                if (e.Key == Key.I && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    EditingCommands.ToggleItalic.Execute(null, Box.Document);
                    e.Handled = true;
                }

                if (e.Key == Key.B && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    EditingCommands.ToggleBold.Execute(null, Box.Document);
                    e.Handled = true;
                }

                if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    EditingCommands.ToggleUnderline.Execute(null, Box.Document);
                    e.Handled = true;
                }

                if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    AddSelectedNameToDocument();
                    e.Handled = true;
                }

                if (e.Key == Key.N && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                {
                    MakeTheFile();
                    e.Handled = true;
                }

                if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    LoadFileWithDialog();
                    e.Handled = true;
                }

                if (e.Key == Key.S && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                {
                    UpdateCurrentText();
                    SaveFileAs(CurrentText);
                    e.Handled = true;
                }

                if (e.Key == Key.Insert)
                {
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    var text = Box.GetXamlString();
                    XamlMessageBox.ShowMessage(this, text, "Xaml Format", MessageBoxImage.Information);
                }

                if (e.Key == Key.E && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    PrintText();
                    e.Handled = true;
                }

                if (e.Key == Key.C && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                {
                    CopyStyle();
                    e.Handled = true;
                }

                if (((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt)) && (e.Key == Key.OemQuestion))
                {
                    string key;
                    var ranges = new TextRange(Box.Selection.End, Box.Selection.End.GetPositionAtOffset(-1, LogicalDirection.Backward));
                    if (ranges.Text.Equals(" ") || ranges.Text.Equals(""))
                    {
                        key = CurrentText.SingleOpeningQuote;
                    }
                    else
                    {
                        key = CurrentText.SingleClosingQuote;
                    }

                    e.Handled = true;
                    Box.Selection.Text = key;
                    Box.Selection.Select(Box.Selection.End, Box.Selection.End);
                }
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private static bool IsLanguageElement(ComplexStyle elem)
        {
            return elem.Caption.Equals("Sprache", StringComparison.OrdinalIgnoreCase) || elem.Caption.Equals("Speech", StringComparison.OrdinalIgnoreCase);
        }

        private void ShowSearchControl(bool switchIt, bool focusReplace = false)
        {
            SearchAndReplaceControl.FocusReplace = focusReplace;

            SearchAndReplaceControl.SwitchVisibility(switchIt);

            SearchAndReplaceControl.FlowDocument = Box.Document;
            SearchAndReplaceControl.ResetManager();
            SearchAndReplaceControl.SearcBox.Text = Box.Selection.Text;
            //Box.SpellCheck.CustomDictionaries.Clear();
            //Box.SpellCheck.CustomDictionaries.Add(new Uri(CustomDict));
        }

        private void PrintText()
        {
            List<Exception> exceptions;

            var ps = new PrintSettings
            {
                FontSizeModifier = CurrentText.IncFontWhenPrinting, Document = CurrentText.Document.CopyWithTickness(new Thickness(0)), UseBlackAndWhite = CurrentText.UseBlackAndWhite, Watermark = CurrentText.Watermark, UseWatermark = CurrentText.UseWatermark, UseCharacters = CurrentText.UseCharacters, ShowPageNumber = CurrentText.ShowPageNumber, PageCountElement = CurrentText.PageCountElement, From = -1, Till = -1, FontFamily = CurrentText.DefaultStyle?.FontFamily ?? CurrentText.Styles?.Styles.FirstOrDefault()?.FontFamily ?? Box.FontFamily, FontSize = CurrentText.DefaultStyle?.FontSize ?? CurrentText.Styles?.Styles.FirstOrDefault()?.FontSize ?? Box.FontSize, BackgroundBrush = CurrentText.PrintBackground, UseOldNumbering = CurrentText.UseOldNumbering
            };

            var printTextWithDialog = PdfHelper.PrintTextWithDialog(Box, out exceptions, GetColors(), ref ps, Path.GetFileNameWithoutExtension(CurrentText.Filepath), CurrentText.Characters);

            if (!Equals(ps.PageCountElement, CurrentText.PageCountElement))
            {
                CurrentText.PageCountElement = ps.PageCountElement;
                CurrentText.IsChanged = true;
            }

            if (!Equals(CurrentText.PrintBackground, ps.BackgroundBrush))
            {
                CurrentText.PrintBackground = ps.BackgroundBrush;
                CurrentText.IsChanged = true;
            }

            if (!Equals(CurrentText.UseBlackAndWhite, ps.UseBlackAndWhite))
            {
                CurrentText.UseBlackAndWhite = ps.UseBlackAndWhite;
                CurrentText.IsChanged = true;
            }

            if (!Equals(CurrentText.UseOldNumbering, ps.UseOldNumbering))
            {
                CurrentText.UseOldNumbering = ps.UseOldNumbering;
                CurrentText.IsChanged = true;
            }

            if (!Equals(CurrentText.UseWatermark, ps.UseWatermark))
            {
                CurrentText.UseWatermark = ps.UseWatermark;
                CurrentText.IsChanged = true;
            }

            if (!Equals(CurrentText.UseCharacters, ps.UseCharacters))
            {
                CurrentText.UseCharacters = ps.UseCharacters;
                CurrentText.IsChanged = true;
            }

            if (!Equals(CurrentText.ShowPageNumber, ps.ShowPageNumber))
            {
                CurrentText.ShowPageNumber = ps.ShowPageNumber;
                CurrentText.IsChanged = true;
            }

            if (!Equals(CurrentText.IncFontWhenPrinting, ps.FontSizeModifier))
            {
                CurrentText.IncFontWhenPrinting = ps.FontSizeModifier;
                CurrentText.IsChanged = true;
            }

            if (!printTextWithDialog || exceptions == null || !exceptions.Any())
                return;

            foreach (var exception in exceptions)
            {
                AddException(exception);
            }
        }

        private void UpdatePopup()
        {
            CountDown.StartCountdown(10);
        }

        private bool CheckWordUpper()
        {
            var start = Box.CaretPosition;
            TextPointer sp;
            TextPointer ep;
            var t = GetWordAtPointer(start, out sp, out ep);

            if (string.IsNullOrEmpty(t) || t.IsCorrectLettering())
                return true;

            AutoCorrectElement = new AutoCorrectElement
            {
                OriginalValue = t, CorrectedValue = t.ToTitleCase(), StartPointer = sp
            };

            Box.Selection.Select(sp, ep);
            Box.Selection.Text = t.ToTitleCase();
            Box.Selection.Select(ep, ep);
            AutoCorrectElement.EndPointer = sp.GetPositionAtOffset(t.Length);
            return false;
        }

        private string GetWordAtPointer(TextPointer textPointer, out TextPointer startPointer, out TextPointer endPointer)
        {
            return string.Join(string.Empty, GetWordCharactersBefore(textPointer, out startPointer), GetWordCharactersAfter(textPointer, out endPointer));
        }

        private string GetWordCharactersBefore(TextPointer textPointer, out TextPointer startPointer)
        {
            var backwards = textPointer.GetTextInRun(LogicalDirection.Backward);
            var wordCharactersBeforePointer = new string(backwards.Reverse().TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).Reverse().ToArray());
            startPointer = textPointer.GetPositionAtOffset(wordCharactersBeforePointer.Length*-1, LogicalDirection.Backward);
            return wordCharactersBeforePointer;
        }

        private string GetWordCharactersAfter(TextPointer textPointer, out TextPointer endPointer)
        {
            var fowards = textPointer.GetTextInRun(LogicalDirection.Forward);
            var wordCharactersAfterPointer = new string(fowards.TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).ToArray());
            endPointer = textPointer.GetPositionAtOffset(wordCharactersAfterPointer.Length, LogicalDirection.Forward);

            return wordCharactersAfterPointer;
        }

        public void AddException(Exception exception, string header = null)
        {
            try
            {
                if (!CanAddMessage)
                    return;
                MoveLast();
                ErrorMessages.Add(new ErrorMessageItem(exception, header));
                FocusTextbox();
                AnimateErrorProvider();
            }
            catch (Exception e)
            {
                XamlMessageBox.ShowErrorMessage(this, e, header); // Hier "echte Meldung ausgeben"
            }
        }

        public void AddSaveException(Exception exception, string header = null)
        {
            try
            {
                if (!CanAddMessage)
                    return;
                MoveLast();
                ErrorMessages.Add(new ErrorMessageItem(exception, header));
                FocusTextbox();
                SaveErrorProvider.Visibility = Visibility.Visible;
            }
            catch (Exception e)
            {
                XamlMessageBox.ShowErrorMessage(this, e, header); // Hier "echte Meldung ausgeben"
            }
        }

        public void AddInformation(string text, string groupname)
        {
            AddMessage(text, groupname, ErrorSeverity.Information);
        }

        public void AddMessage(string text, string groupname, ErrorSeverity severity = ErrorSeverity.None)
        {
            try
            {
                if (!CanAddMessage)
                    return;
                MoveLast();
                ErrorMessages.Add(new ErrorMessageItem(text, severity, groupname));
                FocusTextbox();
            }
            catch (Exception e)
            {
                XamlMessageBox.ShowErrorMessage(this, e); // Hier "echte Meldung ausgeben"
            }
        }

        private void MoveLast()
        {
            var last = ErrorMessages.LastOrDefault();
            if (last == null)
                return;
            last.DisplayGroupName = last.GroupName;
        }

        private void AnimateErrorProvider()
        {
            if (_isErrorRunning)
                return;
            ErrorProvider.Visibility = Visibility.Visible;
            if (_errorBoard == null)
            {
                InitErrorBoard();
            }
            if (_errorBoard == null) return;
            _isErrorRunning = true;
            _errorBoard.Begin(this, true);
        }

        private void AnimateSaveProvider()
        {
            Dispatcher.Invoke(() =>
            {
                if (_isSaveRunning)
                    return;
                SaveProvider.Visibility = Visibility.Visible;
                if (_saveBoard == null)
                {
                    InitSaveBoard();
                }
                if (_saveBoard == null) return;
                _isSaveRunning = true;
                _saveBoard.Begin(this, true);
            });
        }

        private void InitErrorBoard()
        {
            var animatedBrush = new SolidColorBrush(Black);
            RegisterName("AnimatedErrorBrush", animatedBrush);
            ErrorProvider.Background = animatedBrush;
            var colorAnimation = new ColorAnimationUsingKeyFrames {Duration = TimeSpan.FromSeconds(0.6)};
            var red = DiverseUtilities.ColorFromString("#ffd31f1f");
            colorAnimation.KeyFrames.Add(new LinearColorKeyFrame(red, // Target value (KeyValue)
                KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3))) // KeyTime
                );
            colorAnimation.KeyFrames.Add(new LinearColorKeyFrame(Black, // Target value (KeyValue)
                KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6))) // KeyTime
                );
            colorAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(3));

            Storyboard.SetTargetName(colorAnimation, "AnimatedErrorBrush");
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
            _errorBoard = new Storyboard();
            // Create a storyboard to apply the animation.
            _errorBoard.Children.Add(colorAnimation);
            _errorBoard.Completed += ErrorBoardCompleted;
        }

        private void InitSaveBoard()
        {
            var animatedBrush = new SolidColorBrush(Black);
            RegisterName("AnimatedSaveBrush", animatedBrush);
            SaveProvider.Background = animatedBrush;
            var colorAnimation = new ColorAnimationUsingKeyFrames {Duration = TimeSpan.FromSeconds(0.6)};
            var red = (Color) FindResource("DarkerTitleColor");

            colorAnimation.KeyFrames.Add(new LinearColorKeyFrame(red, // Target value (KeyValue)
                KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3))) // KeyTime
                );
            colorAnimation.KeyFrames.Add(new LinearColorKeyFrame(Black, // Target value (KeyValue)
                KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6))) // KeyTime
                );
            colorAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(3));

            Storyboard.SetTargetName(colorAnimation, "AnimatedSaveBrush");
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
            _saveBoard = new Storyboard();
            // Create a storyboard to apply the animation.
            _saveBoard.Children.Add(colorAnimation);
            _saveBoard.Completed += SaveBoardCompleted;
        }

        private void SaveBoardCompleted(object sender, EventArgs e)
        {
            _isSaveRunning = false;
            SaveProvider.Visibility = Visibility.Hidden;
        }

        private void ErrorBoardCompleted(object sender, EventArgs e)
        {
            _isErrorRunning = false;
            ErrorProvider.Visibility = Visibility.Hidden;
        }

        private void MakeNewFile(object sender, RoutedEventArgs e)
        {
            newPopup.IsOpen = true;
        }

        private void TextClicked(object sender, RoutedEventArgs e)
        {
            var evt = e as NameClickedRoutedEventArgs;
            if (evt == null) return;
            var test = Keyboard.Modifiers == ModifierKeys.Control ? " ": string.Empty;

            InsertName(evt.ClickedText + test);
        }

        private void InsertName(string text)
        {
            if (Box.Selection.IsEmpty)
                Box.CaretPosition = Box.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);

            if (Box.Selection.IsEmpty)
                Box?.CaretPosition?.InsertTextInRun(text);
            else
            {
                Box.Selection.Text = text;

                Box.CaretPosition = Box?.CaretPosition?.GetPositionAtOffset(0, LogicalDirection.Forward);
            }
            Box.Focus();
        }

        public static readonly DependencyProperty ShowNameListProperty = DependencyProperty.Register("ShowNameList", typeof (bool), typeof (MainWindow), new PropertyMetadata(false));

        public bool ShowNameList
        {
            get { return (bool) GetValue(ShowNameListProperty); }
            set { SetValue(ShowNameListProperty, value); }
        }

        private void MakeTheFile(ComplexStyles styles = null, TextFile reference = null)
        {
            try
            {
                var range = Box.Selection;
                var fam = reference == null || reference.DefaultStyle == null ? range.GetPropertyValue(FontFamilyProperty) as FontFamily : reference.DefaultStyle.FontFamily;
                var size = reference == null || reference.DefaultStyle == null ? (double) range.GetPropertyValue(FontSizeProperty) : reference.DefaultStyle.FontSize;

                var textFile = new TextFile {Document = new FlowDocument()};
                textFile.CaretOffset = textFile.Document.ContentEnd.GetOffsetToPosition(textFile.Document.ContentStart);
                TextFiles.Add(textFile);
                ShowFile(textFile);
                SetStartFont(size, fam, textFile.Document);
                if (styles != null)
                    CurrentText.Styles = styles;
                if (reference != null)
                {
                    if (reference.Watermark != null)
                        CurrentText.Watermark = reference.Watermark;
                    if (reference.PageCountElement != null)
                        CurrentText.PageCountElement = reference.PageCountElement;

                    CurrentText.PrintBackground = reference.PrintBackground;
                    CurrentText.ClosingQuote = reference.ClosingQuote;
                    CurrentText.OpeningQuote = reference.OpeningQuote;
                    CurrentText.SingleClosingQuote = reference.SingleClosingQuote;
                    CurrentText.SingleOpeningQuote = reference.SingleOpeningQuote;
                    //CurrentText.DefaultStyle = reference.DefaultStyle;
                }
                //CurrentText.CreationDate = DateTime.Now;
                CurrentText.IsChanged = false;
                AddInformation(Properties.Resources.NewFileCreated, Properties.Resources.Diverse);
                newPopup.IsOpen = false;
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void SetStartFont(double size, FontFamily fam, FlowDocument document)
        {
            try
            {
                document.FontSize = size;
                document.FontFamily = fam;
                var selection = new TextRange(document.ContentStart, document.ContentEnd);
                selection.ApplyPropertyValue(FontFamilyProperty, fam);
                selection.ApplyPropertyValue(FontSizeProperty, size);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }


        private void SetFontSize(double value, bool ignoreCancel = false)
        {
            try
            {
                var holeText = new TextRange(Box.Document.ContentStart, Box.Document.ContentEnd);
                if (string.IsNullOrEmpty(holeText.Text) || holeText.Text.Equals("\r\n"))
                {
                    Box.FontSize = Math.Max(7, value);
                }
                else
                    Box.Selection.ApplyPropertyValue(FontSizeProperty, value);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void ConvertToSimpleQuote(bool noStyle)
        {
            var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);
            selectionRange.Text = CurrentText.OpeningQuote + selectionRange.Text + CurrentText.ClosingQuote;

            var positionAtOffset = Box.Selection.Start.GetPositionAtOffset(1, LogicalDirection.Forward);
            var textPointer = Box.Selection.End.GetPositionAtOffset(-1, LogicalDirection.Backward);
            if (positionAtOffset != null && textPointer != null)
                Box.Selection.Select(positionAtOffset, textPointer);
            if (noStyle) return;

            selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

            var firstOrDefault = CurrentText.Styles.Styles.FirstOrDefault(elem => elem.HasCaption && IsLanguageElement(elem)) ?? ApplyableStyles.Styles.FirstOrDefault(elem => elem.HasCaption && IsLanguageElement(elem));

            ApplyStyle(firstOrDefault, selectionRange);
        }

        private void ConvertToSimpleQuote(int inte)
        {
            var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);
            selectionRange.Text = CurrentText.OpeningQuote + selectionRange.Text + CurrentText.ClosingQuote;

            var positionAtOffset = Box.Selection.Start.GetPositionAtOffset(1, LogicalDirection.Forward);
            var textPointer = Box.Selection.End.GetPositionAtOffset(-1, LogicalDirection.Backward);
            if (positionAtOffset != null && textPointer != null)
                Box.Selection.Select(positionAtOffset, textPointer);

            selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

            if (inte == 0)
                inte = 10;

            var firstOrDefault = GetStyleForNumber(inte);

            ApplyStyle(firstOrDefault, selectionRange);
        }

        private ComplexStyle GetStyleForNumber(int inte)
        {
            return
                CurrentText.Styles.Styles.FirstOrDefault(
                    elem =>
                        IsFittingStyleForNumber(inte, elem)) ??
                ApplyableStyles.Styles.FirstOrDefault(
                    elem =>
                        IsFittingStyleForNumber(inte, elem));
        }

        private static bool IsFittingStyleForNumber(int inte, ComplexStyle elem)
        {
            return elem.HasCaption && elem.Caption.EndsWith(inte.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private void ConvertToSimpleQuoteTextOnly(int inte)
        {
            var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

            if (inte == 0)
                inte = 10;

            var firstOrDefault = GetStyleForNumber(inte);

            ApplyStyle(firstOrDefault, selectionRange);
        }

        private void ConvertToSimpleSingleQuote(bool noStyle)
        {
            var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);
            selectionRange.Text = CurrentText.SingleOpeningQuote + selectionRange.Text + CurrentText.SingleClosingQuote;

            if (noStyle) return;

            var positionAtOffset = Box.Selection.Start.GetPositionAtOffset(1, LogicalDirection.Forward);
            var textPointer = Box.Selection.End.GetPositionAtOffset(-1, LogicalDirection.Backward);
            if (positionAtOffset != null && textPointer != null)
                Box.Selection.Select(positionAtOffset, textPointer);
        }

        private void UpdateInfos(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

                CurrentFontFamily = FontFamilies.FirstOrDefault(elem => elem.Family.Equals(selectionRange.GetPropertyValue(FlowDocument.FontFamilyProperty) as FontFamily));
                Debug.WriteLine(selectionRange.GetPropertyValue(FlowDocument.FontSizeProperty));
                var test = selectionRange.GetPropertyValue(FlowDocument.FontSizeProperty);

                if (test != DependencyProperty.UnsetValue)
                    CurrentFontSize = (double) test;

                CheckListings();
                CheckAlignment(selectionRange);
                CheckStyle(selectionRange);
                if (CurrentText != null && !CurrentText.ReadOnly)
                    CurrentText.CaretOffset = Box.Document.ContentStart.GetOffsetToPosition(Box.CaretPosition);

                var currentStyle = FlowDocumentExtensions.GetCurrentStyle(selectionRange, CurrentFontSize);

                if (CurrentText == null) return;

                ComplexStyle existing = CurrentText.Styles.Styles.FirstOrDefault(elem => elem.Equals(currentStyle));
                if (!ComplexStyle.Equals(CurrentStyle, existing))
                    CurrentStyle = existing;
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void CheckStyle(TextRange selectionRange)
        {
            boldBtn.IsChecked = FontWeights.Bold.Equals(selectionRange.GetPropertyValue(FontWeightProperty));

            italicBtn.IsChecked = FontStyles.Italic.Equals(selectionRange.GetPropertyValue(FontStyleProperty));

            underlineBtn.IsChecked = TextDecorations.Underline.Equals(selectionRange.GetPropertyValue(Inline.TextDecorationsProperty));

            strikeoutBtn.IsChecked = TextDecorations.Strikethrough.Equals(selectionRange.GetPropertyValue(Inline.TextDecorationsProperty));
        }

        private void CheckListings()
        {
            buttetBtn.IsChecked = numberingBtn.IsChecked = false;

            try
            {
                var curCaret = Box.CaretPosition;
                var bl = Box.Document.Blocks.FirstOrDefault(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1);

                var list = bl as List;
                if (list != null)
                {
                    if (list.MarkerStyle == TextMarkerStyle.Decimal)
                        numberingBtn.IsChecked = true;
                    else
                        buttetBtn.IsChecked = true;
                }
            }
            catch (Exception e)
            {
                AddException(e);
            }
        }

        private void CheckAlignment(TextRange selectionRange)
        {
            try
            {
                leftBtn.IsChecked = rightBtn.IsChecked = centerBtn.IsChecked = justifyBtn.IsChecked = false;

                if ((TextAlignment) selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty) == TextAlignment.Left)
                {
                    leftBtn.IsChecked = true;
                }

                if ((TextAlignment) selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty) == TextAlignment.Center)
                {
                    centerBtn.IsChecked = true;
                }

                if ((TextAlignment) selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty) == TextAlignment.Right)
                {
                    rightBtn.IsChecked = true;
                }
                if ((TextAlignment) selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty) == TextAlignment.Justify)
                {
                    justifyBtn.IsChecked = true;
                }
            }
            catch
            {
            }
        }

        private void UpdateOtherElements(object sender, RoutedEventArgs e)
        {
            CurrentText.IsChanged = true;
            leftBtn.IsChecked = rightBtn.IsChecked = centerBtn.IsChecked == justifyBtn.IsChecked;
            var btn = (ToggleButton) sender;

            leftBtn.IsChecked = btn.Equals(leftBtn);
            rightBtn.IsChecked = btn.Equals(rightBtn);
            centerBtn.IsChecked = btn.Equals(centerBtn);
            justifyBtn.IsChecked = btn.Equals(justifyBtn);
            FocusTextbox();
        }

        private void Changes(object sender, RoutedEventArgs e)
        {
            CurrentText.IsChanged = true;
            FocusTextbox();
        }

        private void Changes(object sender, EventArgs e)
        {
            CurrentText.IsChanged = true;
            FocusTextbox();
        }

        private void StrikeTextOut(object sender, RoutedEventArgs e)
        {
            try
            {
                TextDecorationCollection tdc;
                try
                {
                    tdc = (TextDecorationCollection) Box.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                    if (tdc == null || !tdc.Equals(TextDecorations.Strikethrough))
                    {
                        tdc = TextDecorations.Strikethrough;
                    }
                    else
                    {
                        tdc = new TextDecorationCollection();
                    }
                }
                catch
                {
                    tdc = new TextDecorationCollection();
                    strikeoutBtn.IsChecked = false;
                }
                Box.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, tdc);
                FocusTextbox();
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        public static ObservableCollection<Color> GetColors()
        {
            var colors = new ObservableCollection<Color>();
            if (string.IsNullOrEmpty(Settings.Default.ColorList))
                return colors;
            foreach (var s in Settings.Default.ColorList.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var convertFromString = ColorConverter.ConvertFromString(s);
                if (convertFromString != null)
                    colors.Add((Color) convertFromString);
            }

            return colors;
        }

        public static void StoreColors(IEnumerable<Color> colors)
        {
            var str = colors.Aggregate(string.Empty, (current, color) => current + (color.ToHexColor() + "|"));
            Settings.Default.ColorList = str;
            Settings.Default.Save();
        }

        private void ChangesTextColor(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new ColorWindow
                {
                    Owner = this, Title = Properties.Resources.ChangeForeColor, PredefinedColors = GetColors()
                };
                try
                {
                    var brush = (SolidColorBrush) Box.Selection.GetPropertyValue(FlowDocument.ForegroundProperty);
                    dlg.StartColor = brush != null ? brush.Color : Black;
                }
                catch
                {
                    dlg.StartColor = Black;
                }
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (dlg.ShowDialog() == true && dlg.Result != MessageBoxResult.Cancel)
                {
                    Box.Selection.ApplyPropertyValue(FlowDocument.ForegroundProperty, new SolidColorBrush(dlg.SelectedColor));
                    StoreColors(dlg.PredefinedColors);
                }
                FocusTextbox();
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void ReloadFile(TextFile file)
        {
            var index = TextFiles.IndexOf(file);
            TextFiles.RemoveAt(index);
            TextFile newFile = null;
            var loaded = LoadFile(file.Filepath, out newFile);
            if (!loaded || newFile == null)
            {
                TextFiles.Insert(index, file);
                ShowFile(file);
                file.IsChanged = false;
                return;
            }

            TextFiles.Insert(index, newFile);
            ShowFile(newFile);
            newFile.IsChanged = false;
        }

        private void ChangesTextBackgroundColor(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorWindow
            {
                Owner = this, Title = Properties.Resources.ChangeBackColor, PredefinedColors = GetColors()
            };
            try
            {
                var brush = (SolidColorBrush) Box.Selection.GetPropertyValue(FlowDocument.BackgroundProperty);
                dlg.StartColor = brush != null ? brush.Color : Colors.Transparent;
            }
            catch
            {
                dlg.StartColor = Black;
            }
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (dlg.ShowDialog() == true && dlg.Result != MessageBoxResult.Cancel)
            {
                Box.Selection.ApplyPropertyValue(FlowDocument.BackgroundProperty, new SolidColorBrush(dlg.SelectedColor));
                StoreColors(dlg.PredefinedColors);
            }
            FocusTextbox();
        }

        private void InsertSpecialCharacter(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.Start);

                var family = selectionRange.GetPropertyValue(FlowDocument.FontFamilyProperty) as FontFamily;
                var element = FontFamilies.FirstOrDefault(elem => elem.Family.Equals(family));
                var dlg = new SymbolWindow {Owner = this, SelectedFamily = element, DefaultFamily = element};

                if (dlg.ShowDialog() == true && dlg.Result != MessageBoxResult.Cancel)
                {
                    CurrentSymbol = dlg.Symbol;
                    if (dlg.SelectedFamily != null)
                        CurrentSymbolFont = dlg.SelectedFamily.Family;
                    Settings.Default.CurrentSymbolFont = CurrentSymbolFont.FamilyNames.First().Value;
                    Settings.Default.Symbol = CurrentSymbol;
                    Settings.Default.Save();
                    InsertCurrentSymbol();
                }
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void InsertCurrentSymbol()
        {
            Box.Selection.Text = CurrentSymbol;
            CurrentFontFamily = FontFamilies.FirstOrDefault(elem => elem.Family.Equals(CurrentSymbolFont));
            var old = Box.Selection.GetPropertyValue(FlowDocument.FontFamilyProperty).ToString();
            Box.Selection.ApplyPropertyValue(FontFamilyProperty, CurrentSymbolFont);
            Box.Selection.Select(Box.Selection.End, Box.Selection.End);
            Box.Selection.ApplyPropertyValue(FontFamilyProperty, old);
            CurrentText.IsChanged = true;
        }

        private void CopyStyle(object sender, RoutedEventArgs e)
        {
            CopyStyle();
        }

        private void CopyStyle()
        {
            AddStyleTo(ApplyableStyles);
        }

        private void AddStyleTo(ComplexStyles complexStyles, ComplexStyle applyableStyle = null)
        {
            try
            {
                if (applyableStyle == null)
                {
                    var range = new TextRange(Box.Selection.Start, Box.Selection.End);

                    if (CheckValues(range))
                    {
                       return;
                    }

                    applyableStyle = FlowDocumentExtensions.GetCurrentStyle(range, CurrentFontSize);
                }

                if (!complexStyles.Styles.Contains(applyableStyle))
                {
                    AddComplexStyle(complexStyles, applyableStyle);
                }
                else
                {
                    var first = complexStyles.Styles.FirstOrDefault(elem => elem.Equals(applyableStyle));
                    if (first == null)
                        return;
                    var res = QuestionBox.ShowMessage(this, string.IsNullOrEmpty(first.Caption) ? Properties.Resources.StyleAddedWithoutName : string.Format(Properties.Resources.StyleAddedWithName, first.Caption), Properties.Resources.Rename+"?", false);
                    if (res == MessageBoxResult.Yes)
                    {
                        RenameElement(complexStyles, first);
                    }
                }
            }
            catch
            {
                MessageBox.ShowMessage(this, Properties.Resources.ErrorWhileStyleCopy, Properties.Resources.Error, MessageBoxImage.Error);
            }
            CurrentStyle = applyableStyle;
        }

        private void RenameElement(ComplexStyles styles, ComplexStyle applyableStyle)
        {
            bool isSpeak;
            string cap;
            var input = AskForComplexStyleTitle(styles, out cap, out isSpeak);

            if (input)
            {
                applyableStyle.Caption = cap;
            }
        }

        private void AddComplexStyle(ComplexStyles styles, ComplexStyle applyableStyle)
        {
            string cap;
            bool isSpeak;
            var input = AskForComplexStyleTitle(styles, out cap, out isSpeak);

            if (input)
            {
                var style = styles.Styles.FirstOrDefault(elem => elem.Caption.Equals(cap));

                HandleStyles(styles, applyableStyle, style, cap, isSpeak);
            }
        }

        private bool AskForComplexStyleTitle(ComplexStyles styles, out string captionText, out bool isSpeak)
        {
            var input = new TextInput
            {
                Owner = this, Watermark = Properties.Resources.StyleTitle, Title = Properties.Resources.ReplaceStyle+": " + styles.Title
            };
            input.ShowDialog();
            captionText = input.UseAsSpeek ? Properties.Resources.Language : input.Text;
            isSpeak = input.UseAsSpeek;
            return input.Result == MessageBoxResult.OK;
        }

        private void HandleStyles(ComplexStyles styles, ComplexStyle applyableStyle, ComplexStyle style, string text, bool ignoreMesage = false)
        {
            applyableStyle.Caption = text;

            var res = style == null ? MessageBoxResult.None : ignoreMesage ? MessageBoxResult.Yes : QuestionBox.ShowMessage(this, Properties.Resources.StyleAlreadyExists, Properties.Resources.ReplaceStyle+"?");

            switch (res)
            {
                case MessageBoxResult.None:
                    //if (!styles.HasItems)
                    //    CurrentText.DefaultStyle = applyableStyle;
                    styles.Styles.Add(applyableStyle);

                    break;

                case MessageBoxResult.Yes:
                    styles.Styles.Remove(style);
                    //if (!styles.HasItems)
                    //    CurrentText.DefaultStyle = applyableStyle;
                    styles.Styles.Add(applyableStyle);

                    break;
                case MessageBoxResult.No:
                    AddComplexStyle(styles, applyableStyle);
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void ApplyStyle(object sender, RoutedEventArgs e)
        {
            var applyableStyle = ((FrameworkElement) sender).Tag as ComplexStyle;

            if (applyableStyle == null)
                return;
            ApplyStyle(applyableStyle);
        }

        private void ApplyStyle(ComplexStyle applyableStyle, TextRange range = null)
        {
            if (applyableStyle == null)
                return;
            try
            {
                if (range == null)
                    range = Box.Selection;
                range.ClearAllProperties();
                range.ApplyPropertyValue(FontFamilyProperty, applyableStyle.FontFamily);
                range.ApplyPropertyValue(FontSizeProperty, applyableStyle.FontSize);
                range.ApplyPropertyValue(FlowDocument.BackgroundProperty, applyableStyle.BackgroundBrush);
                range.ApplyPropertyValue(FlowDocument.ForegroundProperty, applyableStyle.ForegroundBrush);
                range.ApplyPropertyValue(FontStretchProperty, applyableStyle.Stretch);
                range.ApplyPropertyValue(FontWeightProperty, applyableStyle.IsBold ? FontWeights.Bold : FontWeights.Regular);
                range.ApplyPropertyValue(FontStyleProperty, applyableStyle.IsItalic ? FontStyles.Italic : FontStyles.Normal);
                if (applyableStyle.IsUnderlined)
                    range.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                if (applyableStyle.IsStrikedOut)
                    range.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);

                CurrentText.IsChanged = true;
                Box.Focus();
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        public void InsertImage()
        {
            var dlg = new ImagePicker {Owner = this, CanDeleteImage = false};

            // Process open file dialog box results
            if (dlg.ShowDialog() == true && dlg.Result != MessageBoxResult.Cancel)
            {
                var image = new Image {Source = dlg.Image};
                if (image.Source != null)
                {
                    image.Width = dlg.ImageWidth.GetSizeValue();
                    image.Height = dlg.ImageHeight.GetSizeValue();
                    image.Opacity = dlg.ImageOpacity;
                    RenderOptions.SetBitmapScalingMode(image, dlg.ImageScale);

                    var img = new Span();
                    img.Inlines.Add(image);
                    InsertSpanAtCurrentCaret(img);
                }
            }
        }

        private void InsertSpanAtCurrentCaret(Span img)
        {
            var p = Box.CaretPosition.Paragraph;
            if (p == null)
            {
                p = new Paragraph();
                p.Inlines.Add(img);
                Box.Document.Blocks.Add(p);
                return;
            }
            if (p.Inlines.Count == 0)
            {
                p.Inlines.Add(img);
                return;
            }
            try
            {
                var tp = Box.CaretPosition;
                var first = tp.GetTextInRun(LogicalDirection.Backward);
                //tp2 is at the start of this Run.
                var tp2 = tp.GetNextContextPosition(LogicalDirection.Backward);
                var r = new TextRange(Box.Document.ContentStart, Box.Document.ContentEnd);
                if (r.IsEmpty)
                {
                    if (tp.Paragraph != null) tp.Paragraph.Inlines.Add(img);
                }
                else if (tp.Paragraph != null)
                    foreach (var inline in tp.Paragraph.Inlines)
                    {
                        //There can be more then one Inline inside a Paragraph. Let's find the one containing tp by comparing its start position with tp2.
                        if (tp2 != null && inline.ContentStart.GetOffsetToPosition(tp2) <= 0)
                        {
                            //I've only considered the situation where you want to insert the image inside a Run. You must consider other scenarios to make your solution better.
                            var run1 = inline as Run;
                            if (run1 != null)
                            {
                                var run = run1;
                                //Split the Run to two parts, insert the image between them, and delete the original one.
                                var second = run.Text.Substring(first.Length, run.Text.Length - first.Length);
                                tp.Paragraph.Inlines.InsertBefore(run1, BuildRun(first, run1));
                                tp.Paragraph.Inlines.InsertBefore(run1, img);
                                tp.Paragraph.Inlines.InsertAfter(run1, BuildRun(second, run1));
                                tp.Paragraph.Inlines.Remove(run1);
                            }
                            break;
                        }
                    }
                Box.Selection.Text = " ";
                Box.CaretPosition = tp.GetNextContextPosition(LogicalDirection.Forward);
            }
            catch (FileNotFoundException exception)
            {
                AddException(exception);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void ExportToPdf(object sender, RoutedEventArgs e)
        {
            PrintText();
        }

        private void CanExecuteRerollCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = !string.IsNullOrEmpty(Clipboard.GetText());
            }
            catch (Exception exc)
            {
                AddException(exc);
            }
        }

        private void ExecuteRerollCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PasteTextAsTextOnly();
            e.Handled = true;
        }

        private void PasteTextAsTextOnly()
        {
            try
            {
                var text = Clipboard.GetText();
                Box.Selection.Text = text;
                Box.Selection.Select(Box.Selection.End, Box.Selection.End);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }

        private void ExportXaml(object sender, RoutedEventArgs e)
        {
            var text = Box.GetXamlString();
            XamlMessageBox.ShowMessage(this, text, Properties.Resources.ShowXAML, MessageBoxImage.Information);
        }

        private void InsertAnImage(object sender, RoutedEventArgs e)
        {
            InsertImage();
        }

        private void ShowOrActivateWindow(object sender, RoutedEventArgs e)
        {
            ShowErrorWindow();
        }

        private void OnShowErrorWindow(object sender, MouseButtonEventArgs e)
        {
            ShowErrorWindow();
        }

        private void ShowErrorWindow()
        {
            if (ErrorWindow != null)
            {
                ErrorWindow.Activate();
                return;
            }
            ErrorWindow = new ErrorWindow2();
            ErrorWindow.Show();
        }

        public void FocusTextbox()
        {
            Box.Focus();
        }

        private void FocusTextbox(object sender, RoutedEventArgs e)
        {
            FocusTextbox();
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollviewer = e.OriginalSource as ScrollViewer;
            if (scrollviewer == null || IgnoreFocus)
            {
                IgnoreFocus = false;
                return;
            }

            if (CurrentText != null && !_dontDateUp && CurrentText.ReadOnly)
                CurrentText.ScrollOffset = scrollviewer.VerticalOffset;

            FocusTextbox();
        }

        private void UpdateView(object sender, RoutedEventArgs e)
        {
            IgnoreFocus = true;
            var args = e as MatchEventArgs;
            if (args == null)
                return;

            Box.IsReadOnly = true;
            Box.Focus();

            if (args.Range != null)
                Box.Selection.Select(args.Range.Start, args.Range.End);
            else
                Box.Selection.Select(Box.Selection.End, Box.Selection.End);

            if (args.FromReplace)
                SearchAndReplaceControl.ReplaceBox.Focus();
            else
                SearchAndReplaceControl.SearcBox.Focus();
            Box.IsReadOnly = false;
        }

        private void SwitchSearchBar(object sender, RoutedEventArgs e)
        {
            ShowSearchControl(true);
        }

        private void EditDicionary(object sender, RoutedEventArgs e)
        {
            var dlg = new DictionaryEditor {Owner = this, Dictionary = CurrentTextIsEnglish()? CustomDictEnglish: CustomDict, Title = Properties.Resources.EditDictionary};

            if (dlg.ShowDialog() != true || dlg.Result != MessageBoxResult.OK) return;
            if (QuestionBox.ShowMessage(this, Properties.Resources.RestartForChanges, Properties.Resources.Restart +"?") == MessageBoxResult.Yes)
                RestartApplication();
        }

        private void EditNameDicionary(object sender, RoutedEventArgs e)
        {
            var dlg = new DictionaryEditor {Owner = this, Dictionary = NameDict, Title = Properties.Resources.EditNameList};

            if (dlg.ShowDialog() != true || dlg.Result != MessageBoxResult.OK) return;
            if (QuestionBox.ShowMessage(this, Properties.Resources.RestartForNameChanges, Properties.Resources.Restart+"?") == MessageBoxResult.Yes)
                RestartApplication();
        }

        public string NameDict { get; set; }

        private static void RestartApplication()
        {
            Application.Current.Exit += (s, arg) => { Process.Start(Application.ResourceAssembly.Location, "-n"); };
            Application.Current.Shutdown();
        }

        private void UpdateFile(object sender, RoutedEventArgs e)
        {
            var textFile = ((Button) sender).Tag as TextFile;

            if (textFile == null || Equals(CurrentText, textFile)) return;

            if (Settings.Default.AskForPasswordOnTabChange && !string.IsNullOrEmpty(textFile.Password))
            {
                if (!CheckPassword(textFile))
                    return;
            }
            _disableTextChange = true;
            SetBusy();
            ShowFile(textFile);
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            CloseFileFromButton(sender, e);
        }

        private void CloseAllButThis(object sender, RoutedEventArgs e)
        {
            var exception = ((MenuItem) sender).Tag as TextFile;
            if (exception == null) return;

            var textFiles = TextFiles.Where(elem => !elem.Equals(exception)).ToList();
            foreach (var text in textFiles)
            {
                if (!text.IsChanged || (string.IsNullOrEmpty(text.Filepath) && string.IsNullOrWhiteSpace(text.Document.GetText())))
                {
                    TextFiles.Remove(text);
                }
                else
                {
                    if (!SaveFile(text))
                        continue;
                    TextFiles.Remove(text);
                }
            }
        }

        private void CloseFileFromButton(object sender, RoutedEventArgs e)
        {
            var text = ((Button) sender).Tag as TextFile;
            if (text == null)
                return;
            var current = text.Equals(CurrentText);
            var index = TextFiles.IndexOf(text);
            e.Handled = true;
            if (!text.IsChanged || (string.IsNullOrEmpty(text.Filepath) && string.IsNullOrWhiteSpace(text.Document.GetText())))
            {
                TextFiles.Remove(text);
            }
            else
            {
                if (!SaveFile(text))
                    return;
                TextFiles.Remove(text);
            }

            if (!TextFiles.Any())
            {
                var newTextFIle = GetNewTextFIle();
                TextFiles.Add(newTextFIle);
            }
            if (current)
            {
                var file = TextFiles[Math.Min(index, TextFiles.Count - 1)];
                ShowFile(file);
                CurrentText.IsChanged = false;
            }
        }

        private void SaveAllFiles(object sender, RoutedEventArgs e)
        {
            SaveAll();
        }

        private void SaveAll()
        {
            foreach (var textFile in TextFiles.Where(elem => elem.IsChanged))
            {
                SaveFile(textFile);
            }
        }

        private void RemoveStyleFromList(object sender, RoutedEventArgs e)
        {
            var style = ((Button) sender).Tag as ComplexStyle;

            RemoveStyleFromList(style, ApplyableStyles);
        }

        private bool RemoveStyleFromList(ComplexStyle style, ComplexStyles applyableStyle)
        {
            if (style == null || QuestionBox.ShowMessage(this, Properties.Resources.StyleRemoveQuestionTitle, Properties.Resources.DeleteTitle, false) == MessageBoxResult.No)
                return false;
            applyableStyle.Styles.Remove(style);
            return true;
        }

        private void ClearStyles2(object sender, RoutedEventArgs e)
        {
            if (QuestionBox.ShowMessage(this, Properties.Resources.StyleRemoveQuestion, Properties.Resources.DeleteTitle, false) != MessageBoxResult.Yes)
                return;

            CurrentText.Styles.Styles.Clear();
        }

        private void CloseOnMiddle(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
                return;
            CloseFileFromButton(sender, e);
        }

        private void DeleteFromDisk(object sender, RoutedEventArgs e)
        {
            if (QuestionBox.ShowMessage(this, Properties.Resources.FileDeleteQuestion, Properties.Resources.DeleteTitle, false) != MessageBoxResult.Yes)
                return;

            var text = ((MenuItem) sender).Tag as TextFile;

            if (text == null)
                return;
            if (!CheckPassword(text)) return;

            var current = text.Equals(CurrentText);
            var index = TextFiles.IndexOf(text);
            e.Handled = true;

            TextFiles.Remove(text);

            if (!string.IsNullOrEmpty(text.Filepath) && File.Exists(text.Filepath))
                File.Delete(text.Filepath);

            if (!TextFiles.Any())
            {
                var newTextFIle = GetNewTextFIle();
                TextFiles.Add(newTextFIle);
            }
            if (current)
            {
                var file = TextFiles[Math.Min(index, TextFiles.Count - 1)];
                ShowFile(file);
                CurrentText.IsChanged = false;
            }
        }

        private void ChangeWatermark(object sender, RoutedEventArgs e)
        {
            var dlg = new ImagePicker
            {
                Owner = this,
                CanEditScalingMode = false,
                Image = CurrentText.Watermark?.ImageSource,
                ImageWidth = CurrentText.Watermark?.Size.Width ?? 100,
                ImageHeight = CurrentText.Watermark?.Size.Height ?? 100,
                ImageOpacity = CurrentText.Watermark?.Opacity ?? 1d,
                Title = Properties.Resources.UpdateWatermark
            };

            // Process open file dialog box results
            if (dlg.ShowDialog() == true && dlg.Result != MessageBoxResult.Cancel)
            {
                if(CurrentText.Watermark ==  null)
                    CurrentText.Watermark = new Watermark();
                CurrentText.Watermark.ImageSource = dlg.Image;
                if (CurrentText.Watermark != null)
                {
                    CurrentText.Watermark.Size = new Size(dlg.ImageWidth.GetSizeValue(), dlg.ImageHeight.GetSizeValue());
                    CurrentText.Watermark.Opacity = dlg.ImageOpacity;
                    CurrentText.IsChanged = true;
                }
            }
        }

        private void ChangeSideImage(object sender, RoutedEventArgs e)
        {
            var dlg = new ImagePicker
            {
                Owner = this,
                CanEditScalingMode = false,
                Image = imageForPopup.Source,
                ImageWidth = imageForPopup.Width,
                ImageHeight = imageForPopup.Height,
                ImageOpacity = imageForPopup.Opacity,
                Title = Properties.Resources.UpdateSideImage
            };

            // Process open file dialog box results
            if (dlg.ShowDialog() == true && dlg.Result != MessageBoxResult.Cancel)
            {
                imageForPopup.Source = dlg.Image;
                if (imageForPopup.Source != null)
                {
                    ImagePopup.Width = imageForPopup.Width = dlg.ImageWidth.GetSizeValue();
                    ImagePopup.Height = imageForPopup.Height =dlg.ImageHeight.GetSizeValue();
                    imageForPopup.Opacity = dlg.ImageOpacity;
                    ShowImagePopup = true;
                }
                else
                    ShowImagePopup = false;

                FlipImage = false;
                SaveImageInSettings();
            }
        }

        private void ChangeFontWithDialog(object sender = null, RoutedEventArgs e = null)
        {
            var range = Box.Selection;

            var fam = range.GetPropertyValue(FontFamilyProperty) as FontFamily;
            var size = range.GetPropertyValue(FontSizeProperty) as double?;
            var style = range.GetPropertyValue(FontStyleProperty) as FontStyle?;
            var weight = range.GetPropertyValue(FontWeightProperty) as FontWeight?;
            var stretch = range.GetPropertyValue(FontStretchProperty) as FontStretch?;
            var deco = range.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
            var fore = range.GetPropertyValue(TextElement.ForegroundProperty) as Brush;
            var back = range.GetPropertyValue(TextElement.BackgroundProperty) as Brush;

            var dlg = new FontStyleWindow(fore, back)
            {
                Owner = this, TheFontFamily = FontFamilies.FirstOrDefault(elem => elem.Family.Equals(fam)) ?? FontFamilies.FirstOrDefault(), DisplayedText = range.Text, TheTextDecoration = deco, TheFontSize = size == null || !size.IsRealNumber() ? 12 : size.Value, TheFontStyle = style == null ? FontStyles.Normal : style.Value, TheFontWeight = weight == null ? FontWeights.Normal : weight.Value, TextForground = fore, TextBackground = back ?? Brushes.Transparent, TheFontStretch = stretch == null ? FontStretches.Normal : stretch.Value, PredefinedColors = GetColors()
            };

            dlg.ShowDialog();

            if (dlg.Result != MessageBoxResult.OK)
                return;

            range.ApplyPropertyValue(FontFamilyProperty, dlg.TheFontFamily.Family);
            range.ApplyPropertyValue(FontSizeProperty, dlg.TheFontSize);
            range.ApplyPropertyValue(FontWeightProperty, dlg.TheFontWeight);
            range.ApplyPropertyValue(FontStyleProperty, dlg.TheFontStyle);
            range.ApplyPropertyValue(Inline.TextDecorationsProperty, dlg.TheTextDecoration);
            range.ApplyPropertyValue(TextElement.BackgroundProperty, dlg.TextBackground);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, dlg.TextForground);
            range.ApplyPropertyValue(FontStretchProperty, dlg.FontStretch);
            CurrentText.IsChanged = true;
        }

        private void ChangeCurrentToThis(object sender, RoutedEventArgs e)
        {
            var range = new TextRange(Box.Selection.Start, Box.Selection.End);
            var test = range.GetPropertyValue(FlowDocument.FontSizeProperty);

            if (CheckValues(range)) return;

            var applyableStyle = new ComplexStyle
            {
                FontFamily = range.GetPropertyValue(FontFamilyProperty) as FontFamily, FontSize = test != DependencyProperty.UnsetValue ? (double) test : CurrentFontSize, IsBold = range.GetPropertyValue(FontWeightProperty).Equals(FontWeights.Bold), IsItalic = range.GetPropertyValue(FontStyleProperty).Equals(FontStyles.Italic), IsCondenced = range.GetPropertyValue(FontStretchProperty).Equals(FontStretches.Condensed), IsUnderlined = range.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Underline), IsStrikedOut = range.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Strikethrough), BackgroundBrush = range.GetPropertyValue(FlowDocument.BackgroundProperty) as SolidColorBrush, ForegroundBrush = range.GetPropertyValue(FlowDocument.ForegroundProperty) as SolidColorBrush
            };

            Box.Document.CheckAndChangeFonts(applyableStyle, ((MenuItem) sender).Tag as ComplexStyle);
            CurrentText.IsChanged = true;
        }

        private static bool CheckValues(TextRange range)
        {
            var propertyValue = range.GetPropertyValue(FontFamilyProperty);
            if (propertyValue == null || propertyValue.Equals(DependencyProperty.UnsetValue))
                return true;
            propertyValue = range.GetPropertyValue(FlowDocument.FontSizeProperty);
            if (propertyValue == null || propertyValue.Equals(DependencyProperty.UnsetValue))
                return true;
            propertyValue = range.GetPropertyValue(FlowDocument.BackgroundProperty);
            if (propertyValue != null && propertyValue.Equals(DependencyProperty.UnsetValue))
                return true;
            propertyValue = range.GetPropertyValue(FlowDocument.ForegroundProperty);
            if (propertyValue != null && propertyValue.Equals(DependencyProperty.UnsetValue))
                return true;
            return false;
        }

        private void ChangeThisToSelected(object sender, RoutedEventArgs e)
        {
            var style = ((MenuItem) sender).Tag as ComplexStyle;

            if (style == null) return;
            UpdateStyle(CurrentText.Styles, style);
        }

        private void ChangeThisToSelected2(object sender, RoutedEventArgs e)
        {
            var style = ((MenuItem) sender).Tag as ComplexStyle;

            if (style == null) return;
            UpdateStyle(ApplyableStyles, style);
        }

        private void UpdateStyle(ComplexStyles stylelist, ComplexStyle style)
        {
            var range = new TextRange(Box.Selection.Start, Box.Selection.End);

            if (CheckValues(range)) return;

            var test = range.GetPropertyValue(FlowDocument.FontSizeProperty);

            var applyableStyle = new ComplexStyle
            {
                FontFamily = range.GetPropertyValue(FontFamilyProperty) as FontFamily, FontSize = test != DependencyProperty.UnsetValue ? (double) test : CurrentFontSize, IsBold = range.GetPropertyValue(FontWeightProperty).Equals(FontWeights.Bold), IsItalic = range.GetPropertyValue(FontStyleProperty).Equals(FontStyles.Italic), IsCondenced = range.GetPropertyValue(FontStretchProperty).Equals(FontStretches.Condensed), IsUnderlined = range.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Underline), IsStrikedOut = range.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Strikethrough), BackgroundBrush = range.GetPropertyValue(FlowDocument.BackgroundProperty) as SolidColorBrush, ForegroundBrush = range.GetPropertyValue(FlowDocument.ForegroundProperty) as SolidColorBrush, Caption = style.Caption
            };

            ReplaceStyleWith(stylelist, style, applyableStyle, false);
        }

        private void ReplaceStyleWith(ComplexStyles styleList, ComplexStyle oldStyle, ComplexStyle newStyle, bool ask)
        {
            var existing = styleList.Styles.FirstOrDefault(elem => elem.Equals(newStyle));
            if (existing != null)
            {
                MessageBox.ShowMessage(this, string.IsNullOrEmpty(existing.Caption) ? Properties.Resources.StyleExistsWithoutName : string.Format(Properties.Resources.StyleExists, existing.Caption), "", MessageBoxImage.Exclamation);
                return;
            }

            var help = oldStyle.Copy();
            styleList.Styles.Remove(oldStyle);
            styleList.Styles.Add(newStyle);

            if (!ask || QuestionBox.ShowMessage(this, Properties.Resources.UpdateElements, Properties.Resources.Replace+"?", false) == MessageBoxResult.Yes)
            {
                SetBusy();
                Box.Document.CheckAndChangeFonts(help, newStyle);
                CurrentText.IsChanged = true;
            }
        }

        private void CopyStyle2(object sender, RoutedEventArgs e)
        {
            AddStyleTo(CurrentText.Styles);
        }

        private void RemoveStyleFromList2(object sender, RoutedEventArgs e)
        {
            var style = ((Button) sender).Tag as ComplexStyle;
            var removed = RemoveStyleFromList(style, CurrentText.Styles);
            if (!removed || QuestionBox.ShowMessage(this, Properties.Resources.RomoveUsageAsWell, Properties.Resources.RemovedStyles+"?", false) != MessageBoxResult.Yes) return;
            CurrentText.Document.CheckAndChangeFonts(style, CurrentText.DefaultStyle);
            var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

            CurrentStyle = FlowDocumentExtensions.GetCurrentStyle(selectionRange, CurrentFontSize);
        }

        private void AddToApplyable(object sender, RoutedEventArgs e)
        {
            var style = ((MenuItem) sender).Tag as ComplexStyle;
            if (style == null) return;

            AddStyleWithoutAsk(ApplyableStyles, style);
        }

        private void AddToCurrentStyles(object sender, RoutedEventArgs e)
        {
            var style = ((MenuItem) sender).Tag as ComplexStyle;
            if (style == null) return;

            AddStyleWithoutAsk(CurrentText.Styles, style);
        }

        private static void AddStyleWithoutAsk(ComplexStyles applyableStyles, ComplexStyle style)
        {
            if (!applyableStyles.Styles.Contains(style))
            {
                applyableStyles.Styles.Add(style);
            }
        }

        private void CopyGlobals(object sender, RoutedEventArgs e)
        {
            foreach (var applyableStyle in ApplyableStyles.Styles)
            {
                if (!CurrentText.Styles.Styles.Contains(applyableStyle))
                {
                    CurrentText.Styles.Styles.Add(applyableStyle);
                }
            }
        }

        private void MakeEmpty(object sender, RoutedEventArgs e)
        {
            MakeTheFile();
        }

        private void MakeEmptyCurrent(object sender, RoutedEventArgs e)
        {
            MakeTheFile(CurrentText.Styles, CurrentText);
        }

        private void MakeEmptyReference(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                FileName = Properties.Resources.Unknown, DefaultExt = "etf", InitialDirectory = Settings.Default.DefaultFolder, Filter = Properties.Resources.FileFilterSmall, CheckFileExists = true, Multiselect = false
            };
            if (dlg.ShowDialog() != true)
                return;

            var existing = TextFiles.FirstOrDefault(tf => tf.Filepath != null && tf.Filepath.Equals(dlg.FileName));
            if (existing == null)
            {
                if (!LoadFile(dlg.FileName, out existing))
                    return;
            }
            if (existing == null) return;

            MakeTheFile(existing.Styles, existing);
        }

        private void ExportGlobalStyle(object sender, RoutedEventArgs e)
        {
            ExportStyle(ApplyableStyles);
        }

        private void ExportCurrentStyle(object sender, RoutedEventArgs e)
        {
            ExportStyle(CurrentText.Styles);
        }

        public void ExportStyle(ComplexStyles style)
        {
            style.Export();
        }

        private void MakeEmptyStyle(object sender, RoutedEventArgs e)
        {
            var style = ComplexStyles.Import();
            MakeTheFile(style);
        }

        private void ImportCurrentStyle(object sender, RoutedEventArgs e)
        {
            var styles = ComplexStyles.Import();
            CurrentText.Styles = styles;
        }

        private void ImportGlobalStyle(object sender, RoutedEventArgs e)
        {
            var styles = ComplexStyles.Import();
            ApplyableStyles = styles;
        }

        private void InsertLastSpecialCharacter(object sender, RoutedEventArgs e)
        {
            InsertCurrentSymbol();
            Box.Focus();
        }

        private void EditCurrentLocal(object sender, RoutedEventArgs e)
        {
            var style = ((MenuItem) sender).Tag as ComplexStyle;
            if (style == null) return;

            ChangeStyleWithDialog(CurrentText.Styles, style, false);
        }

        private void EditCurrentGlobal(object sender, RoutedEventArgs e)
        {
            var style = ((MenuItem) sender).Tag as ComplexStyle;
            if (style == null) return;

            ChangeStyleWithDialog(ApplyableStyles, style, true);
        }

        private void ChangeStyleWithDialog(ComplexStyles styles, ComplexStyle oldStyle, bool ask)
        {
            var dlg = new FontStyleWindow(oldStyle.ForegroundBrush, oldStyle.BackgroundBrush)
            {
                Owner = this, TheFontFamily = Utilities.PossibleFonts.FirstOrDefault(elem => elem.Family.Equals(oldStyle.FontFamily)), TheTextDecoration = oldStyle.Decoration, TheFontSize = oldStyle.FontSize, TheFontStyle = oldStyle.Style, TheFontWeight = oldStyle.Weight, TextForground = oldStyle.ForegroundBrush, TextBackground = oldStyle.BackgroundBrush ?? Brushes.Transparent, TheFontStretch = oldStyle.Stretch, PredefinedColors = GetColors()
            };

            dlg.ShowDialog();

            if (dlg.Result != MessageBoxResult.OK)
                return;
            var addstyle = new ComplexStyle
            {
                FontFamily = dlg.TheFontFamily == null ? oldStyle.FontFamily : dlg.TheFontFamily.Family, FontSize = dlg.TheFontSize, IsBold = dlg.TheFontWeight == FontWeights.Bold, IsItalic = dlg.TheFontStyle == FontStyles.Italic, IsUnderlined = TextDecorations.Underline.Equals(dlg.TheTextDecoration), IsStrikedOut = TextDecorations.Strikethrough.Equals(dlg.TheTextDecoration), BackgroundBrush = dlg.TextBackground, ForegroundBrush = dlg.TextForground, IsCondenced = dlg.TheFontStretch == FontStretches.Condensed, Caption = oldStyle.Caption
            };

            ReplaceStyleWith(styles, oldStyle, addstyle, ask);
          
        }

        public void HideStylePopup()
        {
            //StylePopup2.IsOpen = false;
        }

        public void ShowStylePopupIfAllowed()
        {
            //StylePopup2.IsOpen = ShowStylePopup;
        }

        private static void UpdatePopup(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void DragPopup(object sender, DragDeltaEventArgs e)
        {
            StylePopup2.VerticalOffset += e.VerticalChange;
            StylePopup2.HorizontalOffset += e.HorizontalChange;
        }

        private void DragPopup2(object sender, DragDeltaEventArgs e)
        {
            ImagePopup.VerticalOffset += e.VerticalChange;
            ImagePopup.HorizontalOffset += e.HorizontalChange;
        }

        private void SetChangePassword(object sender, RoutedEventArgs e)
        {
            var dlg = new PasswordInput
            {
                Owner = this, PasswordDecrpyted = CurrentText.Password, Filename = CurrentText.Display, PasswordQuestion = CurrentText.PasswordQuestion
            };

            if (dlg.ShowDialog() == true && dlg.Result == MessageBoxResult.OK)
            {
                CurrentText.Password = dlg.GetEncryptedPassword();
                CurrentText.PasswordQuestion = dlg.PasswordQuestion;
                CurrentText.IsChanged = true;
            }
        }

        private void UpdateQuotations(object sender, RoutedEventArgs e)
        {
            var fontf = Box.Selection.GetPropertyValue(FontFamilyProperty) as FontFamily;
            var fonts = (double) Box.Selection.GetPropertyValue(FontSizeProperty);
            var quotes = new QuoteWindow
            {
                FontFamily = fontf ?? CurrentFontFamily.Family, SingleClosingQuotation = CurrentText.SingleClosingQuote, SingleOpeningQuotation = CurrentText.SingleOpeningQuote, ClosingQuotation = CurrentText.ClosingQuote, OpeningQuotation = CurrentText.OpeningQuote, FontSize = Math.Max(fonts, 6), Owner = this
            };

            if (quotes.ShowDialog() == true && quotes.Result == MessageBoxResult.OK)
            {
                CurrentText.SingleClosingQuote = quotes.SingleClosingQuotation;
                CurrentText.SingleOpeningQuote = quotes.SingleOpeningQuotation;
                CurrentText.ClosingQuote = quotes.ClosingQuotation;
                CurrentText.OpeningQuote = quotes.OpeningQuotation;
            }
        }

        public void UpdateAutosave()
        {
            _t.Enabled = Settings.Default.SaveAutomatical;
            ShowSaveFile = !Settings.Default.SaveAutomatical;
        }

        private void RenameFile(object sender, RoutedEventArgs e)
        {
            var textFile = ((MenuItem) sender).Tag as TextFile;
            if (textFile == null) return;

            var dlg = new SimpleInput(textFile.Filepath) {Owner = this};
            if (dlg.ShowDialog() != true || dlg.Result == MessageBoxResult.Cancel) return;

            var buildFileName = dlg.BuildFileName(dlg.Text);

            if (string.Equals(textFile.Filepath, buildFileName)) return;

            File.Move(textFile.Filepath, buildFileName);
            textFile.Filepath = buildFileName;
            UpdateOpenFileList();
            Settings.Default.Save();
            SaveAllAutomatic(TextFiles);
        }

        private void ClearStyles3(object sender, RoutedEventArgs e)
        {
            var counter = 0;
            var counteradd = 0;
            SetBusy();
            var styles = CurrentText.Document.GetStyles(CurrentFontSize);
            foreach (var complexStyle in new List<ComplexStyle>(CurrentText.Styles.Styles))
            {
                if (complexStyle.Equals(CurrentText.DefaultStyle)) continue;
                if (!styles.Any(elem => elem.Equals(complexStyle)))
                {
                    CurrentText.Styles.Styles.Remove(complexStyle);
                    counter++;
                }
            }

            var complexStyles = styles.Where(elem => FamilyExist(elem.FontFamily));
            foreach (var complexStyle in complexStyles)
            {
                var any = CurrentText.Styles.Styles.Any(elem => elem.Equals(complexStyle));
                if (!any)
                {
                    counteradd++;
                    complexStyle.Caption = string.Format("XXX_Neuer Stil {0:00}", counteradd);
                    CurrentText.Styles.Styles.Add(complexStyle);
                }
            }

            MessageBox.ShowMessage(this, counter == 0 && counteradd == 0 ? Properties.Resources.AllStylesUpToDate : string.Format(Properties.Resources.StyleUpdateInfo, counter, counteradd), "Entfernte Stile");
        }

        private bool FamilyExist(FontFamily fontFamily)
        {
            var familyExist = Utilities.PossibleFonts.Any(elem => elem.Family.Equals(fontFamily));

            return familyExist;
        }

        private void EmptyStyle(object sender, RoutedEventArgs e)
        {
            var style = ((MenuItem) sender).Tag as ComplexStyle;
            if (style == null) return;

            CurrentText.Document.CheckAndChangeFonts(style, CurrentText.DefaultStyle);
            var selectionRange = new TextRange(Box.Selection.Start, Box.Selection.End);

            CurrentStyle = FlowDocumentExtensions.GetCurrentStyle(selectionRange, CurrentFontSize);
        }

        private void OpenAbout(object sender, RoutedEventArgs e)
        {
            new About {Owner = this}.ShowDialog();
        }

        private void SetDefaultStyle(object sender, RoutedEventArgs e)
        {
            var sty =((MenuItem) sender).Tag as ComplexStyle;
            if (sty == null) return;
            sty.IsDefault = true;
            CurrentText.ClearDefaultWithout(sty);
            Box.FontFamily = sty.FontFamily;
        }

        private void CountStylePositions(object sender, RoutedEventArgs e)
        {
            var sty = ((MenuItem) sender).Tag as ComplexStyle;
            if (sty == null) return;
            TextPointer p = null;
            UiServices.SetBusyState();

            var count = CurrentText.Document.CountStyle(sty, 0d, ref p, false);
            MessageBox.ShowMessage(this, string.Format(Properties.Resources.StyleUsageCount, sty.Caption, count), Properties.Resources.Occurance);
        }

        private void GoToFirstCall(object sender, RoutedEventArgs e)
        {
            var sty = ((MenuItem) sender).Tag as ComplexStyle;
            if (sty == null) return;

            UiServices.SetBusyState();
            TextPointer p = null;
            CurrentText.Document.CountStyle(sty, 0d, ref p, true);
            if (p != null)
            {
                Box.Selection.Select(p, p);
                Box.Focus();
            }
        }

        private void AddNewFile(object sender, MouseButtonEventArgs e)
        {
        }

        private const int _MARGIN = 25;

        private void ChangeRightIndent(object sender, RoutedEventArgs e)
        {
            var value = (int) ((Button) sender).Tag*_MARGIN;
            var curCaret = Box.CaretPosition.Paragraph;
            if (curCaret != null)
                curCaret.SetValue(Block.MarginProperty, new Thickness(curCaret.Margin.Left, curCaret.Margin.Top, Math.Max(0, curCaret.Margin.Right + value), curCaret.Margin.Bottom));
            Changes(sender, e);
        }

        private void ChangeLeftIndent(object sender, RoutedEventArgs e)
        {
            var value = (int) ((Button) sender).Tag*_MARGIN;
            var curCaret = Box.CaretPosition.Paragraph;
            if (curCaret != null)
                curCaret.SetValue(Block.MarginProperty, new Thickness(Math.Max(0, curCaret.Margin.Left + value), curCaret.Margin.Top, curCaret.Margin.Right, curCaret.Margin.Bottom));
            Changes(sender, e);
        }

        private void FlipTheImage(object sender, RoutedEventArgs e)
        {
            FlipImage = !FlipImage;
        }

        private void RemoveImage(object sender, RoutedEventArgs e)
        {
            ShowImagePopup = false;
            imageForPopup.Source = null;
            SaveImageInSettings();
        }

        private void InsertQuoteSymbol(object sender, RoutedEventArgs e)
        {
            var symbol =((Button)sender).Content as string;

            Box.Selection.Text = symbol;
            Box.Selection.Select(Box.Selection.End, Box.Selection.End);
            Box.Focus();
            CurrentText.IsChanged = true;
        }

        private void ExportRtf(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = Path.GetFileNameWithoutExtension(CurrentText.Filepath),
                DefaultExt = "rtf",
                Filter = "Rich Text Files|*.rtf"
            };
            if (dlg.ShowDialog(this) == true)
            {
                TextFile.SaveFile(dlg.FileName,CurrentText.Document);
            }
        }

        private void UpdateLineHight(object sender, RoutedEventArgs e)
        {

            var curCaret = Box.CaretPosition.Paragraph;
            curCaret.LineHeight = 40d;
            
        }

        private void ChangeLineHeight(object sender, RoutedEventArgs e)
        {
            var lineHeight = CurrentText.Document.Blocks.FirstBlock.LineHeight;
            LineHightDouble.Value = lineHeight.Equals(Double.NaN) ? 1 : lineHeight;
            lineHightPopup.IsOpen = true;

        }

        private void SetAutomatical(object sender, RoutedEventArgs e)
        {
            _lineHeight = Double.NaN;
            lineHightPopup.IsOpen = false;
            foreach (var block in CurrentText.Document.Blocks.Where(elem => !elem.BreakPageBefore))
            {
                block.LineHeight = Double.NaN;
            }
            Changes(sender, e);

        }

        private double _lineHeight;

        private void UpdateIdent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(lineHightPopup.IsOpen)
                _lineHeight = ((DoubleUpDown) sender).Value ?? Double.NaN;
        }

        private void UpdateBlocks(object sender, EventArgs e)
        {
            if (CurrentText.Document.Blocks.FirstBlock.LineHeight.Equals(_lineHeight)) return;
            foreach (var block in CurrentText.Document.Blocks.Where(elem=>!elem.BreakPageBefore))
            {
                block.LineHeight = Math.Max(1,_lineHeight) ;
            }
            Changes(sender, e);
        }

        private void ClosePop(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                lineHightPopup.IsOpen = false;
        }

        private void SetLanguage(object sender, RoutedEventArgs e)
        {
            CurrentText.Language = (string) ((ToggleButton) sender).Tag;
            CurrentText.Document.UpdateLanguage(CurrentText.Language);
            languagePopup.IsOpen = false;
            CurrentText.IsChanged = true;
            // InputLanguageManager.SetInputLanguage(Box, CultureInfo.CreateSpecificCulture(CurrentText.Language));
        }

        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            languagePopup.IsOpen = true;
        }

        private void ShowLanguage(object sender, EventArgs e)
        {
            enBtn.IsChecked = CurrentText.Language.IndexOf("en",StringComparison.InvariantCultureIgnoreCase)!= -1;
            deBtn.IsChecked = CurrentText.Language.IndexOf("de", StringComparison.InvariantCultureIgnoreCase) != -1;
        }
    }
}