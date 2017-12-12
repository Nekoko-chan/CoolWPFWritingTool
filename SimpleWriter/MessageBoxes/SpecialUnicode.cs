using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ComplexWriter.MessageBoxes
{
    [ValueConversion(typeof(FontFamily), typeof(string))]
    public class SpecialUnicode : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UnicodeString;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static List<string> Quotations
        {
            get
            {
                var ret = new List<string>
                {
                        "\u0027",
                        "\u0022", 
                        "\u201A", 
                        "\u201E", 
                        "\u2018", 
                        "\u201C", 
                        "\u2019", 
                        "\u201D", 
                        "\u2039", 
                        "\u00AB", 
                        "\u203A", 
                        "\u00BB", 
                        "\u201B", 
                        "\u201F", 
                        "\u301D", 
                        "\u301E", 
                        "\u301F", 
                        "\u300C", 
                        "\u300D",
                        "\u300E", 
                        "\u300F"
                };
                return ret;
            }
        }

        public static List<string[]> Unicodes
        {
            get
            {
                var ret = new List<string[]>
                {
                    new[] {"\u2190", "Kleiner Pfeil links"},
                    new[] {"\u2191", "Kleiner Pfeil oben"},
                    new[] {"\u2192", "Kleiner Pfeil rechts"},
                    new[] {"\u2193", "Kleiner Pfeil unten"},
                    new[] {"\u2194", "Kleiner Pfeil links-rechts"},
                    new[] {"\u27A2", "3D Pfeil rechts"},
                    new[] {"\u21E6", "Dicker weißer Pfeil links"},
                    new[] {"\u21E8", "Dicker weißer Pfeil rechts"},
                    new[] {"\u21E7", "Dicker weißer Pfeil oben"},
                    new[] {"\u21E9", "Dicker weißer Pfeil unten"},
                    new[] {"\u21E0", "Gestrichelter Pfeil links"},
                    new[] {"\u21E2", "Gestrichelter Pfeil rechts"},
                    new[] {"\u27A8", "Dicker schwarzer Pfeil"},
                    new[] {"\u21B3", "Pfeil oben rechts"},
                    new[] {"\u2014", "Gedankenstrich"},
                    new[] {"\u2798", "Dicker Südost-Pfeil"},
                    new[] {"\u279A", "Dicker Nordost-Pfeil"},
                    new[] {"\u00A2", "Cent-Zeichen"},
                    new[] {"\u00A3", "Pfund-Zeichen"},
                    new[] {"\u00A5", "Yen-Zeiche"},
                    new[] {"\u2206", "Inkrement"},
                    new[] {"\u220F", "n-stelliges Produkt"},
                    new[] {"\u2211", "n-stellige Summe"},
                    new[] {"\u221A", "Wurzel aus"},
                    new[] {"\u221E", "Unendlich"},
                    new[] {"\u2248", "Fast gleich"},
                    new[] {"\u2260", "Ungleich"},
                    new[] {"\u2261", "Genau äquivalent"},
                    new[] {"\u2264", "Kleiner gleich"},
                    new[] {"\u2265", "Größer gleich"},
                    new[] {"\u00B1", "Plus/Minus"},
                    new[] {"\u2259", "Entspricht"},
                    new[] {"\u2297", "Multiplikation"},
                    new[] {"\u2030", "Pro-Mille Zeichen"},
                    new[] {"\u2031", "Pro-Zehntausend-Zeichen"},
                    new[] {"\u2227", "Logisches Und"},
                    new[] {"\u2228", "Logisches Oder"},
                    new[] {"\u263A", "Heller Smiley"},
                    new[] {"\u263B", "Dunkler Smiley"},
                    new[] {"\u263C", "Sonne mit Strahlen"},
                    new[] {"\u2640", "Weiblich"},
                    new[] {"\u2642", "Männlich"},
                    new[] {"\u2660", "Pik"},
                    new[] {"\u2663", "Kreuz"},
                    new[] {"\u2665", "Herz"},
                    new[] {"\u2666", "Karo"},
                    new[] {"\u266A", "Note 1"},
                    new[] {"\u266B", "Note 2"},
                    new[] {"\u263A", "Fröhlicher Smiley"},
                    new[] {"\u2639", "Trauriger Smiley"},
                    new[] {"\u271E", "Kreuz"},
                    new[] {"\u2721", "Davidstern"},
                    new[] {"\u2625", "Ankh"},
                    new[] {"\u262A", "Stern und Halbmond"},
                    new[] {"\u262F", "Ying und Yang"},
                    new[] {"\u25C6", "Kleiner Karo"},
                    new[] {"\u2756", "Vier Karos"},
                    new[] {"\u2022", "Ausgemalter Punkt"},
                    new[] {"\u25CB", "nicht ausgemalter punkt"},
                    new[] {"\u25C9", "\"Fischauge\""},
                    new[] {"\u25CE", "\"Bullseye\""},
                    new[] {"\u25AA", "Kleines schwarzes Quadrat"},
                    new[] {"\u2717", "Angekreuzt"},
                    new[] {"\u2713", "Haken"},
                    new[] {"\u2714", "Fetter Haken"},
                    new[] {"\u2612", "Feld angekreuzt"},
                    new[] {"\u2611", "Feld angehakt"},
                    new[] {"\u25A1", "Weißes Quadrat"},
                    new[] {"\u25A0", "Schwarzes Quadrat"},
                    new[] {"\u270F", "Bleistift"},
                    new[] {"\u231B", "Sanduhr"},
                    new[] {"\u25CF", "Schwarze Punkt"},
                    new[] {"\u2318", "\"Kulturstätte\""},
                    new[] {"\u203B", "Referenzzeichen"},
                    new[] {"\u2326", "Löschtaste nach links"},
                    new[] {"\u232B", "Löschtaste nach rechts"},
                    new[] {"\u266B", "Doppelte Achtelnote"},
                    new[] {"\u266C", "Doppelte Sechzehntelnote"},
                    new[] {"\u2602", "Schirm"},
                    new[] {"\u2620", "Totenkopf"},
                    new[] {"\u2740", "Weiße Blume"},
                    new[] {"\u03B1", "Alpha"},
                    new[] {"\u03B2", "Beta"},
                    new[] {"\u03B3", "Gamma"},
                    new[] {"\u03B4", "Delta"},
                    new[] {"\u03B5", "Epsilon"},
                    new[] {"\u03B6", "Zeta"},
                    new[] {"\u03B7", "Zeta"},
                    new[] {"\u03B8", "Theta"},
                    new[] {"\u03B9", "Iota"},
                    new[] {"\u03BA", "Kappa"},
                    new[] {"\u03BB", "Lamda"},
                    new[] {"\u03BC", "Mu"},
                    new[] {"\u03BD", "Nu"},
                    new[] {"\u03BE", "Xi"},
                    new[] {"\u03BF", "Omicron"},
                    new[] {"\u03C1", "Pi"},
                    new[] {"\u03C2", "Rho"},
                    new[] {"\u03C3", "Sigma"},
                    new[] {"\u03C4", "Tau"},
                    new[] {"\u03C5", "Upsilon"},
                    new[] {"\u03C6", "Phi"},
                    new[] {"\u03C7", "Chi"},
                    new[] {"\u03C8", "Psi"},
                    new[] {"\u03C9", "Omega"},
                    new[] {"\u2648", "Sternzeichen Widder"},
                    new[] {"\u2649", "Sternzeichen Stier"},
                    new[] {"\u264A", "Sternzeichen Zwillinge"},
                    new[] {"\u264B", "Sternzeichen Krebs"},
                    new[] {"\u264C", "Sternzeichen Löwe"},
                    new[] {"\u264D", "Sternzeichen Jungfrau"},
                    new[] {"\u264E", "Sternzeichen Waage"},
                    new[] {"\u264F", "Sternzeichen Skorpion"},
                    new[] {"\u2650", "Sternzeichen Schütze"},
                    new[] {"\u2651", "Sternzeichen Steinbock"},
                    new[] {"\u2652", "Sternzeichen Wassermann"},
                    new[] {"\u2653", "Sternzeichen Fische"},
                    new[] {"\u263D", "Zunehmender Mond"},
                    new[] {"\u263E", "Abnehmender Mond"},
                    new[] {"\u263F", "Merkur"},
                    new[] {"\u2641", "Erde"},
                    new[] {"\u2643", "Jupiter"},
                    new[] {"\u2644", "Saturn"},
                    new[] {"\u2645", "Uranus"},
                    new[] {"\u2646", "Neptun"},
                    new[] {"\u2647", "Plut0"},
                    new[] {"\u00BC", "Ein Viertel"},
                    new[] {"\u00BD", "Ein Halb"},
                    new[] {"\u2159", "Ein Sechstel"},
                    new[] {"\u00BE", "Drei Viertel"},
                    new[] {"\u00BF", "Umgedrehten Fragezeichen"},
                    new[] {"\u00E7", "Kleines c mit Cedille"},
                    new[] {"\u00B6", "Linienende"},
                    new[] {"\u00A9", "Copyright"},
                    new[] {"\u00AE", "Registierte Marke"},
                };

                return ret;
            }
        }

        public static List<string> UnicodesWithoutDecription
        {
            get
            {
                List<string[]> withDesc = Unicodes;
                return withDesc.Select(s => s[0]).ToList();
            }
        }

        public static string UnicodeString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (string s in UnicodesWithoutDecription)
                {
                    sb.Append(s + " ");
                }
                return sb.ToString();
            }
        }
    }
}
