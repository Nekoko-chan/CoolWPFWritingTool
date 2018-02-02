using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using Writer.Data;
using Button = System.Windows.Controls.Button;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class FileExporter
    {
        public FileExporter()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.InitialDirectory))
                BaseFolder = Properties.Settings.Default.InitialDirectory;
        }

        /// <summary>
        /// DependencyProperty for 'BaseFolder'
        /// </summary>
        public static readonly DependencyProperty BaseFolderProperty =
        DependencyProperty.Register("BaseFolder", typeof(string), typeof(FileExporter), new UIPropertyMetadata(string.Empty, Loadfilesformsource));

        /// <summary>
        /// 
        /// </summary>
        public string BaseFolder
        {
            get { return (string)GetValue(BaseFolderProperty); }
            set { SetValue(BaseFolderProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'ConverterItems'
        /// </summary>
        public static readonly DependencyProperty ConverterItemsProperty =
        DependencyProperty.Register("ConverterItems", typeof(ObservableCollection<ExportStepInfo>), typeof(FileExporter), new UIPropertyMetadata(null, Addhooks));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ExportStepInfo> ConverterItems
        {
            get { return (ObservableCollection<ExportStepInfo>)GetValue(ConverterItemsProperty); }
            set { SetValue(ConverterItemsProperty, value); }
        }

        private static void Addhooks(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dObj = d as FileExporter;
            if (dObj == null) return;

            ((ObservableCollection<ExportStepInfo>)e.NewValue).CollectionChanged += dObj.FileExporter_CollectionChanged;

        }

        internal void FileExporter_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           viewer.ScrollToEnd();
        }





        /// <summary>
        /// DependencyProperty for 'HasBoldTitle'
        /// </summary>
        public static readonly DependencyProperty HasBoldTitleProperty =
        DependencyProperty.Register("HasBoldTitle", typeof(bool), typeof(FileExporter), new UIPropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool HasBoldTitle
        {
            get { return (bool)GetValue(HasBoldTitleProperty); }
            set { SetValue(HasBoldTitleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'HasUnderlinedTitle'
        /// </summary>
        public static readonly DependencyProperty HasUnderlinedTitleProperty =
        DependencyProperty.Register("HasUnderlinedTitle", typeof(bool), typeof(FileExporter), new UIPropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for 'TextFontSize'
        /// </summary>
        public static readonly DependencyProperty TextFontSizeProperty =
        DependencyProperty.Register("TextFontSize", typeof(double), typeof(FileExporter), new UIPropertyMetadata(15.25d));

        /// <summary>
        /// 
        /// </summary>
        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'TitleFontSize'
        /// </summary>
        public static readonly DependencyProperty TitleFontSizeProperty =
        DependencyProperty.Register("TitleFontSize", typeof(double), typeof(FileExporter), new UIPropertyMetadata(24.25d));

        /// <summary>
        /// 
        /// </summary>
        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }



        /// <summary>
        /// 
        /// </summary>
        public bool HasUnderlinedTitle
        {
            get { return (bool)GetValue(HasUnderlinedTitleProperty); }
            set { SetValue(HasUnderlinedTitleProperty, value); }
        }



        private static void Loadfilesformsource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dObj = d as FileExporter;
            dObj?.ScanForFiles((string)e.NewValue);
        }

        private void ScanForFiles(string folder)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;
            var files = GetFilenames(folder);

            TextFileItems = new ObservableCollection<TextFileItem>(files.Select(elem => new TextFileItem { Filename = elem, IsChecked = false }));
        }

        /// <summary>
        /// DependencyProperty for 'TextFileItems'
        /// </summary>
        public static readonly DependencyProperty TextFileItemsProperty =
        DependencyProperty.Register("TextFileItems", typeof(ObservableCollection<TextFileItem>), typeof(FileExporter), new UIPropertyMetadata(new ObservableCollection<TextFileItem>()));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TextFileItem> TextFileItems
        {
            get { return (ObservableCollection<TextFileItem>)GetValue(TextFileItemsProperty); }
            set { SetValue(TextFileItemsProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'MainVisibility'
        /// </summary>
        public static readonly DependencyProperty MainVisibilityProperty =
        DependencyProperty.Register("MainVisibility", typeof(Visibility), typeof(FileExporter), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public Visibility MainVisibility
        {
            get { return (Visibility)GetValue(MainVisibilityProperty); }
            set { SetValue(MainVisibilityProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'InfoVisibility'
        /// </summary>
        public static readonly DependencyProperty InfoVisibilityProperty =
        DependencyProperty.Register("InfoVisibility", typeof(Visibility), typeof(FileExporter), new UIPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility InfoVisibility
        {
            get { return (Visibility)GetValue(InfoVisibilityProperty); }
            set { SetValue(InfoVisibilityProperty, value); }
        }



        private IEnumerable<string> GetFilenames(IEnumerable<string> selecteddirectories)
        {
            return selecteddirectories.Concat(selecteddirectories.SelectMany(GetFilenames));
        }

        private IEnumerable<string> GetFilenames(string selectedPath)
        {
            var files = Directory.GetFiles(selectedPath).Where(e => e.EndsWith("etf"));
            if (Directory.GetDirectories(selectedPath).Any())
                return files.Concat(GetFilenames(Directory.GetDirectories(selectedPath)));
            return files;
        }

        private void ChooseBaseFolder(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog { SelectedPath = Properties.Settings.Default.InitialDirectory };

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            BaseFolder = dlg.SelectedPath;
            Properties.Settings.Default.InitialDirectory = dlg.SelectedPath;
            Properties.Settings.Default.Save();
        }

        private void CheckAll(object sender, RoutedEventArgs e)
        {
            CheckAllElements(true);
        }

        private void CheckAllElements(bool b)
        {
            foreach (var textFileItem in TextFileItems)
            {
                textFileItem.IsChecked = b;
            }
        }

        private void UncheckAll(object sender, RoutedEventArgs e)
        {
            CheckAllElements(false);
        }

        private void UpdateChildFiles(object sender, RoutedEventArgs e)
        {
            var checke = ((System.Windows.Controls.CheckBox)sender).Tag as string;

            foreach (var source in TextFileItems.Where(elem => elem.Folder.Equals(checke)))
            {
                source.IsChecked = ((System.Windows.Controls.CheckBox)sender).IsChecked == true;
            }
        }

        private BackgroundWorker _worker;

        private void ToSingleFile(object sender, RoutedEventArgs e)
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += MoWorkerDoWork;
            _worker.ProgressChanged += MoWorkerProgressChanged;
            _worker.RunWorkerCompleted += m_oWorker_RunWorkerCompleted;
            _worker.WorkerReportsProgress = true;

            var dlg = new SaveFileDialog
            {
                FileName = "exports",
                DefaultExt = "rtf",
                Filter = "Rich Text Files|*.rtf"
            };
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            MainVisibility = Visibility.Collapsed;
            InfoVisibility = Visibility.Visible;

            var info = new WorkerPreferences
            {
                Filename = dlg.FileName,
                Items = TextFileItems.Where(elem=>elem.IsChecked),
                TitleSize = TitleFontSize,
                TextSize = TextFontSize,
                UseBold = HasBoldTitle,
                UseUnderline = HasUnderlinedTitle
            };
            ConverterItems= new ObservableCollection<ExportStepInfo>();

            _worker.RunWorkerAsync(info);
        }

        private void MoWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var info = (WorkerPreferences) e.Argument;
            var filename = info.Filename;
            var document = new FlowDocument();
            
            var counter = 0;
            var index = 0;

            var failcounter = 0;

            var length = info.Items.Count(elem => elem.IsChecked);

            foreach (var source in info.Items)
            {
                try
                {
                    var file = TextFile.Load(source.Filename);

                    file.Document.UpdateBreaks();

                    var title = string.IsNullOrWhiteSpace(file.Title)
                        ? Path.GetFileNameWithoutExtension(source.Filename)
                        : file.Title;

                    document = document.AddFlowDocument(file.Document, title, counter != 0, info.TextSize, info.TitleSize, info.UseUnderline,info.UseBold);

                    index++;
                    counter++;

                    var doub2 = ((double)index/length)*100;
                    _worker.ReportProgress((int)doub2, new ExportStepInfo { Filename = Path.GetFileNameWithoutExtension(source.Filename), Success = true });
                }
                catch 
                {
                    failcounter++;
                    var doub = ((double)index / length) * 100;
                    _worker.ReportProgress((int)doub, new ExportStepInfo { Filename = Path.GetFileNameWithoutExtension(source.Filename), Success = false });
                    index++;
                }
            }
            document.SaveAsRtfFile(info.Filename);

            Process.Start(info.Filename);
            _worker.ReportProgress(-10, new ExportStepInfo { Filename = string.Format(Properties.Resources.CompleteMessage,failcounter), Success = failcounter == 0 });
        }

        private void MoWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var elemen = (ExportStepInfo)e.UserState;
                elemen.Percentage = e.ProgressPercentage;
                ConverterItems.Add(elemen);
            });
        }

        private void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _worker = null;
        }

        private void CheckText(object sender, TextCompositionEventArgs e)
        {
            var check = Utilities.IsAllowedText(e.Text);
            e.Handled = !check;
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult) ((Button) sender).Tag;
        }

        public void ActivateOk()
        {
            Result = MessageBoxResult.OK;
        }

        struct ExportInfo
        {
            public FlowDocument Document { get; set; }
            public string Filename { get; set; }
        }

        struct  WorkerPreferences
        {
            public IEnumerable<TextFileItem> Items { get; set; } 
            public double TextSize { get; set; }
            public double TitleSize { get; set; }

            public bool UseBold { get; set; }
            public bool UseUnderline { get; set; }
            public string Filename { get; set; }
        }
      

    }

    public struct ExportStepInfo
    {
        public string Filename { get; set; }
        public bool Success { get; set; }

        public int Percentage { get; set; }
    }
}
