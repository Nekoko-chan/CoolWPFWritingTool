using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FontOrganizer.Properties;
using Path = System.IO.Path;

namespace FontOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tbx.Text = EXAMPLE;
        }

        public static readonly DependencyProperty FontFamiliesProperty =
          DependencyProperty.Register("FontFamilies", typeof(ObservableCollection<FontElement>), typeof(MainWindow),
              new PropertyMetadata(null));

        public ObservableCollection<FontElement> FontFamilies
        {
            get { return (ObservableCollection<FontElement>)GetValue(FontFamiliesProperty); }
            set { SetValue(FontFamiliesProperty, value); }
        }

        public static readonly DependencyProperty PossibleDirectoriesProperty =
         DependencyProperty.Register("PossibleDirectories", typeof(ObservableCollection<string>), typeof(MainWindow),
             new PropertyMetadata(null));

        public ObservableCollection<string> PossibleDirectories
        {
            get { return (ObservableCollection<string>)GetValue(PossibleDirectoriesProperty); }
            set { SetValue(PossibleDirectoriesProperty, value); }
        }


        public string _fontpath;

        private void OpenFontFolder(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (!string.IsNullOrEmpty(Settings.Default.LastDirectory))
                dialog.SelectedPath = Settings.Default.LastDirectory;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.Cancel)
            {
                Application.Current.Shutdown();
                return;
            }

            _fontpath = dialog.SelectedPath;
            Settings.Default.LastDirectory = _fontpath;
            Settings.Default.Save();


            var fonts = LoadFontsFromDirectory(_fontpath);
            if ( !fonts.Any())
            {
                Application.Current.Shutdown();
                return;
            }
            FontFamilies = new ObservableCollection<FontElement>(fonts.Values);

            LoadPossibleDirectories();


        }

        private void LoadPossibleDirectories()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (!string.IsNullOrEmpty(Settings.Default.LastMainDirectory))
                dialog.SelectedPath = Settings.Default.LastMainDirectory;
            else
                if (!string.IsNullOrEmpty(Settings.Default.LastDirectory))
                    dialog.SelectedPath = Settings.Default.LastDirectory;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();



            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                Application.Current.Shutdown();
                return;
            }

            Settings.Default.LastMainDirectory = dialog.SelectedPath;
            Settings.Default.Save();

            var dirs = Directory.GetDirectories(dialog.SelectedPath).Where(elem => !string.Equals(elem, _fontpath));
            PossibleDirectories = new ObservableCollection<string>(dirs);
        }

        public Dictionary<string, FontElement> LoadFontsFromDirectory(string path)
        {
            Dictionary<string, FontElement> foundFonts = new Dictionary<string, FontElement>();

            if (!Directory.Exists(path)) throw new Exception("directory doesnt exist");

            foreach (FileInfo fi in new DirectoryInfo(path).GetFiles("*.ttf"))
            {
                PrivateFontCollection fileFonts = new PrivateFontCollection();
                fileFonts.AddFontFile(fi.FullName);
                if (!foundFonts.ContainsKey(fileFonts.Families[0].Name))
                {
                    //add the font only if this fontfamily doesnt exist yet
                    FontFamily family = new FontFamily(String.Format("file:///{0}#{1}", fi.FullName, fileFonts.Families[0].Name));

                    var element = new FontElement
                    {
                        Family = family,
                        Name = fileFonts.Families[0].Name,
                        Pathes = new ObservableCollection<string>(new[] { fi.FullName })
                    };
                    foundFonts.Add(fileFonts.Families[0].Name, element);
                }
                else
                {
                    foundFonts[fileFonts.Families[0].Name].Pathes.Add(fi.FullName);
                }
            }

            return foundFonts;
        }

        private void MoveToDestination(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = ((Button) sender).Tag as string;
                if (string.IsNullOrEmpty(path)) return;

                var selectedItems = ListBox.SelectedItems.OfType<FontElement>().ToList();
                foreach (var selectedItem in selectedItems)
                {
                    foreach (var pathe in selectedItem.Pathes)
                    {
                        var fileName = Path.GetFileName(pathe);
                        if (string.IsNullOrEmpty(fileName)) continue;

                        File.Move(pathe, Path.Combine(path,fileName));
                    }
                    FontFamilies.Remove(selectedItem);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void DeleteSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                var elements = ListBox.SelectedItems.OfType<FontElement>().SelectMany(elem=>elem.Pathes).ToList();
                if (
                    MessageBox.Show(this, string.Format("Would you really like to delete {0} file(s) permanently?",elements.Count), "Delete", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.No) return;

                foreach (var fontElement in elements)
                {
                    File.Delete(fontElement);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public const string EXAMPLE = @"This is an example text
ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ
abcdefghijklmnopqrstuvwxyzäöüß
»main quotes« 
›main single quotes‹
""secondary quotes""
'secondary single quotes'";

        private void ResetText(object sender, RoutedEventArgs e)
        {
            tbx.Text = EXAMPLE;
        }
    }

    public class FontElement
    {
        public FontFamily Family { get; set; }
        public string Name { get; set; }
        public ObservableCollection<string> Pathes { get; set; } 
    }

}
