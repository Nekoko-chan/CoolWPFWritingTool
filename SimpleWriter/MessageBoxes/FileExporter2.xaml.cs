using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using Writer.Data;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileExporter : Window
    {
        public FileExporter()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.DefaultExportPath))
                BaseFolder = Properties.Settings.Default.DefaultExportPath;
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

        private static void Loadfilesformsource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dObj = d as FileExporter;
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
        DependencyProperty.Register("TextFileItems", typeof(ObservableCollection<TextFileItem>), typeof(FileExporter), new UIPropertyMetadata(new ObservableCollection<TextFileItem>()));

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
            var dlg = new FolderBrowserDialog { SelectedPath = Properties.Settings.Default.DefaultExportPath };

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            BaseFolder = dlg.SelectedPath;
            Properties.Settings.Default.DefaultExportPath= dlg.SelectedPath;
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
            var checke = ((System.Windows.Controls.CheckBox) sender).Tag  as TextFileItem;

            if (checke == null || !checke.IsFolder) return;

            foreach (var source in TextFileItems.Where(elem=>elem.Filename.StartsWith(checke.Filename)))
            {
                source.IsChecked = checke.IsChecked;
            }

        }

        private void ToSingleFile(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
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
                if(!string.IsNullOrEmpty(file.Title))
                {
                    var para = new Paragraph(new Run(file.Title));
                    if (file?.Document?.Blocks.FirstBlock == null)
                    {
                        sb.AppendLine(source.Filename);
                        continue;
                    }
                    file.Document.Blocks.InsertBefore(file.Document.Blocks.FirstBlock,para);
                }
                document = document.AddFlowDocument(file.Document);

            }

            TextFile.SaveFile(dlg.FileName,document);

            var message = string.IsNullOrEmpty(sb.ToString()) ? Properties.Resources.ExportBookSuccess : string.Format(Properties.Resources.ExportBookSuccess,sb);

            MessageBoxes.MessageBox.ShowMessage(this,message,string.Empty);
        }
    }
}
