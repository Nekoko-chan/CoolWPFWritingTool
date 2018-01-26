using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ComplexWriter.Properties;
using Writer.Data;

namespace ComplexWriter
{
    /// <summary>
    /// Interaction logic for Testblock.xaml
    /// </summary>
    public partial class Testblock : Window
    {
        public Testblock()
        {
            InitializeComponent();
            FontElement firstOrDefault = Utilities.PossibleFonts.FirstOrDefault(
                elem => elem.Family.FamilyNames.Any(ele => ele.Value.Equals(Settings.Default.LastFontName)));
            if(firstOrDefault!= null)
            {
                block1.FontFamily = firstOrDefault.Family;
                block2.FontFamily = firstOrDefault.Family;
                block3.FontFamily = firstOrDefault.Family;
                block4.FontFamily = firstOrDefault.Family;
                block5.FontFamily = firstOrDefault.Family;
                block6.FontFamily = firstOrDefault.Family;
            }
            block1.FontSize = Settings.Default.LastFontSize;
            block2.FontSize = Settings.Default.LastFontSize;
            block3.FontSize = Settings.Default.LastFontSize;
            block4.FontSize = Settings.Default.LastFontSize;
            block5.FontSize = Settings.Default.LastFontSize;
            block6.FontSize = Settings.Default.LastFontSize;

        }
    }
}
