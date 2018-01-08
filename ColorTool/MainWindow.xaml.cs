using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ColorTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OpenFile();
        }

        private void OpenFile()
        {
            var open = new OpenFileDialog
            {
                FileName = "Colors",
                DefaultExt = "xaml",
                Filter = "Xaml-Files|*.xaml;"
            };

            if (open.ShowDialog() == true)
            {
                InitColors(open.FileName);
                foreach (var colorElement in Colors)
                {
                    Debug.WriteLine(colorElement.Key + ":" + colorElement.Color);
                }
            }
        }

        /// <summary>
        /// DependencyProperty for 'Colors'
        /// </summary>
        public static readonly DependencyProperty ColorsProperty =
        DependencyProperty.Register("Colors", typeof(ObservableCollection<ColorElement>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<ColorElement>()));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ColorElement> Colors
        {
            get { return (ObservableCollection<ColorElement>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }


        public void InitColors(string filename)
        {
            var dict = new ResourceDictionary() { Source = new Uri(filename, UriKind.Absolute) };

            Colors.Add(new ColorElement("BackColor", dict));
            Colors.Add(new ColorElement("AlternateBackColor", dict));
            Colors.Add(new ColorElement("OverColor", dict));
            Colors.Add(new ColorElement("DragColor", dict));
            Colors.Add(new ColorElement("TitleColor", dict));
            Colors.Add(new ColorElement("DarkerTitleColor", dict));
            Colors.Add(new ColorElement("SelectionColor", dict));
            Colors.Add(new ColorElement("SelectionColor2", dict));
            Colors.Add(new ColorElement("BackGradient1", dict));
            Colors.Add(new ColorElement("BackGradient2", dict));
            Colors.Add(new ColorElement("ButtonHighlight", dict));
            Colors.Add(new ColorElement("ButtonShadow", dict));
            Colors.Add(new ColorElement("ButtonLight", dict));
            Colors.Add(new ColorElement("ButtonFocus", dict));
            Colors.Add(new ColorElement("ButtonBorder", dict));
            Colors.Add(new ColorElement("RtfBrush", dict));
        }

        public void SaveData()
        {
            var save = new SaveFileDialog
            {
                FileName = "Colors",
                DefaultExt = "xaml",
                Filter = "Xaml-Files|*.xaml;",
            };

            if (save.ShowDialog() == true)
            {
                if (File.Exists(save.FileName))
                {
                    SaveOld(save.FileName);
                }
                string res;
                using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ColorTool.Resources.Colors.txt")))
                {
                    res = reader.ReadToEnd();
                }

                using (var streamWriter = new StreamWriter(save.FileName, false, Encoding.Unicode))
                {
                    streamWriter.Write(ReplaceElements(res));
                }
            }
        }

        private void SaveOld(string fileName)
        {
            var counter = 1;

            var name = $"{System.IO.Path.GetFileNameWithoutExtension(fileName)}_Sicherung{counter:00}.xaml";
            var newname = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileName), name);
            while (File.Exists(newname))
            {
                counter++;
                name = $"{System.IO.Path.GetFileNameWithoutExtension(fileName)}_Sicherung{counter:00}.xaml";
                newname = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileName), name);
            }
            File.Copy(fileName,newname);
        }

        private string ReplaceElements(string res)
        {
            var test = res;
            for (int i = 0; i < Colors.Count; i++)
            {
                var oldValue = "{" + i + "}";
                test = test.Replace(oldValue, Colors[i].Color.ToString());
            }
            return test;
        }

        private void UpdateColor(object sender, RoutedEventArgs e)
        {
            var colorelem = ((Button) sender).Tag as ColorElement;
            if (colorelem == null) return;

            var colorwd = new ColorWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                StartColor = colorelem.Color,
                PredefinedColors = new ObservableCollection<Color>(Colors.Select(elem => elem.Color)),
                SelectedColor = colorelem.Color,
                Title = colorelem.Key
            };

            if (colorwd.ShowDialog() != true || colorwd.Result == MessageBoxResult.Cancel) return;


            colorelem.Color = colorwd.SelectedColor;
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            SaveData();
        }
    }
}
