using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using SplashDemo;

namespace ComplexWriter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class App : Application
    {

        App()
        {
        }
         #region Constants and Fields
    
    public string FileName { get; set; }
    public bool AllowSecondInstance { get; set; }


    #endregion

    #region Methods

        protected override void OnStartup(StartupEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/splashscreen.png");
            image.EndInit();
            Splasher.Splash = new SplashScreen(image);
            Splasher.ShowSplash();

            if (_SingleInstance.IsFirstInstance || AllowSecondInstance)
            {
                _SingleInstance.Application = this;
                _SingleInstance.ArgumentsReceived += SingleInstanceParameter;
                _SingleInstance.ListenForArgumentsFromSuccessiveInstances();
                // Do your other app logic
            }
            else
            {
                // if there is an argument available, fire it
                if (e.Args.Length > 0)
                {
                    _SingleInstance.PassArgumentsToFirstInstance(e.Args[0]);
                }

                Environment.Exit(0);
            }
         
        }

        static void SingleInstanceParameter(object sender, SingleInstance.GenericEventArgs<string> e)
        {
            var singel = (SingleInstance) sender;

            try
            {
                var app = singel.Application;
                if (app == null) return;
                app.Dispatcher.Invoke(() =>
                {
                    var main = app.MainWindow as MainWindow;
                    if (main == null) return;

                    main.LoadAndShowFile(e.Data);
                    main.Activate();
                    main.Focus();
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

        }

        #endregion

        private static readonly SingleInstance _SingleInstance = new SingleInstance();


        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                App app = new App();
                var fname = !args.Any() ? string.Empty : args[0];
                if (fname.Equals("-n"))
                {
                    app.AllowSecondInstance = true;
                    fname = string.Empty;
                }

             
                MainWindow window = new MainWindow(fname);
                app.MainWindow = window;
                app.Run(window);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(),"Fehler",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
       
    }


    public class EventTest : EventWaitHandle
    {

        public string Filename { get; set; }

        public EventTest(bool initialState, EventResetMode mode, string name) : base(initialState, mode, name)
        {
        }
    }
 


}
