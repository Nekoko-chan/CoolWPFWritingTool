using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ComplexWriter.MessageBoxes;
using ComplexWriter.Properties;

namespace ComplexWriter
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            //ani.StandardDuration = 200;
            //ani.ZipSource = "../Images/anim.zip";
            Loaded += (s, e) =>
                {
                    var elem = Template.FindName("close", this) as Button;
                    if(elem!= null)
                        elem.Focus();

                    Version = string.Format("{0} - \u00A9 2015",
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());


                    var path = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

                   path = path.Substring(0, path.LastIndexOf("\\"));

                    ConfigPath = path.Substring(path.LastIndexOf("\\")+1);

                };
        }


        public static readonly DependencyProperty VersionProperty =
         DependencyProperty.Register("Version", typeof(string), typeof(About), new PropertyMetadata(string.Empty));

        public string Version
        {
            get { return (string)GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value); }
        }

        public static readonly DependencyProperty ConfigPathProperty =
        DependencyProperty.Register("ConfigPath", typeof(string), typeof(About), new PropertyMetadata(string.Empty));

        public string ConfigPath
        {
            get { return (string)GetValue(ConfigPathProperty); }
            set { SetValue(ConfigPathProperty, value); }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void UpdateSaveSettings(object sender, RoutedEventArgs e)
        {
            var sd = new SecurityDialog
            {
                Owner = this,
                AllowEmptyPasswordQuestions = Settings.Default.AllowEmptyQuestions,
                AskPasswords = Settings.Default.AskForPassword,
                AskPasswordsOnTabChange = Settings.Default.AskForPasswordOnTabChange,
                SaveAutomatical =  Settings.Default.SaveAutomatical,
                HideQuestion =  Settings.Default.HideQuestion,
                UseEnglish =  Settings.Default.Language.StartsWith("en"),
                UseGerman = Settings.Default.Language.StartsWith("de"),
                AutoSaveInterval = Settings.Default.AutoSaveInterval,
                SaveWhenIdle = Settings.Default.AutoSaveOnBreak,
                NoSave = !Settings.Default.SaveAutomatical && !Settings.Default.AutoSaveOnBreak
            };

            if (sd.ShowDialog() != true || sd.Result != MessageBoxResult.OK) return;

            Settings.Default.AllowEmptyQuestions = sd.AllowEmptyPasswordQuestions;
            Settings.Default.AskForPassword = sd.AskPasswords;
            Settings.Default.AskForPasswordOnTabChange = sd.AskPasswordsOnTabChange;
            Settings.Default.HideQuestion = sd.HideQuestion;
            var language = sd.UseEnglish ? "en" : "de";
            if (Settings.Default.Language != language)
            {
                Settings.Default.Language = language;
                MessageBoxes.MessageBox.ShowMessage(this, Properties.Resources.RestartForLanguage, Properties.Resources.Restart,fontSize:15d);
            }
            if (sd.SaveWhenIdle != Settings.Default.AutoSaveOnBreak)
            {
                Settings.Default.AutoSaveOnBreak = sd.SaveWhenIdle;
                MainWindow.Global.UpdateAutosave();
            }
            if (sd.SaveAutomatical != Settings.Default.SaveAutomatical)
            {
                Settings.Default.SaveAutomatical = sd.SaveAutomatical;
                MainWindow.Global.UpdateAutosave();
            }
            if (!sd.AutoSaveInterval.Equals(Settings.Default.AutoSaveInterval))
            {
                Settings.Default.AutoSaveInterval = sd.AutoSaveInterval;
                MainWindow.Global.UpdateAutoSaveInterval();
            }
            Settings.Default.Save();
        }
    }
}
