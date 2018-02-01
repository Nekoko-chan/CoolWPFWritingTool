using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileExporter.Properties;
using System.Reflection;
using Writer.Data;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using RichTextBox = System.Windows.Controls.RichTextBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using System.Windows.Markup;

namespace FileExporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(Settings.Default.BaseFolder))
                BaseFolder = Settings.Default.BaseFolder;
        }


        /// <summary>
        /// DependencyProperty for 'BaseFolder'
        /// </summary>
        public static readonly DependencyProperty BaseFolderProperty =
        DependencyProperty.Register("BaseFolder", typeof(string), typeof(MainWindow), new UIPropertyMetadata(string.Empty, Loadfilesformsource));

        /// <summary>
        /// 
        /// </summary>
        public string BaseFolder
        {
            get { return (string)GetValue(BaseFolderProperty); }
            set { SetValue(BaseFolderProperty, value); }
        }

        private static void Loadfilesformsource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dObj = d as MainWindow;
            dObj?.ScanForFiles((string)e.NewValue);
        }

        private void ScanForFiles(string folder)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;
            var files = GetFilenames(folder);

            TextFileItems = new ObservableCollection<TextFileItem>(files.Select(elem => new TextFileItem {Filename = elem,IsChecked = false,IsFolder = Directory.Exists(elem)}));
        }

        /// <summary>
        /// DependencyProperty for 'TextFileItems'
        /// </summary>
        public static readonly DependencyProperty TextFileItemsProperty =
        DependencyProperty.Register("TextFileItems", typeof(ObservableCollection<TextFileItem>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<TextFileItem>()));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TextFileItem> TextFileItems
        {
            get { return (ObservableCollection<TextFileItem>)GetValue(TextFileItemsProperty); }
            set { SetValue(TextFileItemsProperty, value); }
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
            var dlg = new FolderBrowserDialog { SelectedPath = Settings.Default.BaseFolder };

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            BaseFolder = dlg.SelectedPath;
            Settings.Default.BaseFolder = dlg.SelectedPath;
            Settings.Default.Save();
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
            var checke = ((System.Windows.Controls.CheckBox) sender).Tag  as TextFileItem;

            if (checke == null || !checke.IsFolder) return;

            foreach (var source in TextFileItems.Where(elem=>elem.Filename.StartsWith(checke.Filename)))
            {
                source.IsChecked = checke.IsChecked;
            }

        }

        private void ToSingleFile(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "exports",
                DefaultExt = "rtf",
                Filter = "Rich Text Files|*.rtf"
            };
            if (dlg.ShowDialog(this) != true) return;

            var document = new FlowDocument();


            foreach (var source in TextFileItems.Where(elem=>elem.IsChecked &&!elem.IsFolder))
            {
                var file = TextFile.Load(source.Filename);
                if(file == null) continue;

                document = document.AddFlowDocument(file.Document);

            }

            //TextFile.SaveFile(dlg.FileName,document);

            var blubb = XamlWriter.Save(document);
            System.Diagnostics.Debug.WriteLine(blubb);
            var test = ConvertXamlToRtf(blubb);

            File.AppendAllText(dlg.FileName,test);


        }

        private static string ConvertXamlToRtf(string xamlContent)
        {
            var assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            var xamlRtfConverterType = assembly.GetType("System.Windows.Documents.XamlRtfConverter");
            var xamlRtfConverter = Activator.CreateInstance(xamlRtfConverterType, true);
            var convertXamlToRtf = xamlRtfConverterType.GetMethod("ConvertXamlToRtf", BindingFlags.Instance | BindingFlags.NonPublic);
            var rtfContent = (string)convertXamlToRtf.Invoke(xamlRtfConverter, new object[] { xamlContent });
            return rtfContent;
        }

        public static void SaveFile(string fileName, FlowDocument flowDocument)
        {
            if (fileName.EndsWith(".rtf"))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    //var textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                    //textRange.Save(fs, System.Windows.DataFormats.Rtf);
                }
            }
            else if (fileName.EndsWith(".rtxt"))
            {
                var format = System.Windows.DataFormats.XamlPackage;
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
    }
}
