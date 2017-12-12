using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CustomControls;
using ExtensionObjects;
using PdfSharp.Pdf.Filters;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class FontStyleWindow
    {

        private const string DefaultText = "abcABCäöüÄÖÜß123345679";

        private Brush _fore;
        private Brush _back;

        public FontStyleWindow(Brush fore, Brush back) : this()
        {
            _fore = fore;
            _back = back;
        }
            public FontStyleWindow()
        {
            InitializeComponent();
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            if (win == null) return;

            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }


        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            MainWindow.StoreColors(PredefinedColors.ToList());
            var messageBoxResult = (MessageBoxResult)((Button) sender).Tag;
            Result = messageBoxResult;
        }

        #region DependencyProperties
        public static readonly DependencyProperty TheFontFamilyProperty =
           DependencyProperty.Register("TheFontFamily", typeof(FontElement), typeof(FontStyleWindow),
               new PropertyMetadata(null));

        public FontElement TheFontFamily
        {
            get { return (FontElement)GetValue(TheFontFamilyProperty); }
            set { SetValue(TheFontFamilyProperty, value); }
        }

        public static readonly DependencyProperty TheFontStyleProperty =
           DependencyProperty.Register("TheFontStyle", typeof(FontStyle), typeof(FontStyleWindow),
               new PropertyMetadata(FontStyles.Normal));

        public FontStyle TheFontStyle
        {
            get { return (FontStyle)GetValue(TheFontStyleProperty); }
            set { SetValue(TheFontStyleProperty, value); }
        }

        public static readonly DependencyProperty TheFontStretchProperty =
           DependencyProperty.Register("TheFontStretch", typeof(FontStretch), typeof(FontStyleWindow),
               new PropertyMetadata(FontStretches.Normal));

        public FontStretch TheFontStretch
        {
            get { return (FontStretch)GetValue(TheFontStretchProperty); }
            set { SetValue(TheFontStretchProperty, value); }
        }

        public static readonly DependencyProperty TheFontSizeProperty =
           DependencyProperty.Register("TheFontSize", typeof(double), typeof(FontStyleWindow),
               new PropertyMetadata(12d));

        public double TheFontSize
        {
            get { return (double)GetValue(TheFontSizeProperty); }
            set { SetValue(TheFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TheFontWeightProperty =
           DependencyProperty.Register("TheFontWeight", typeof(FontWeight), typeof(FontStyleWindow),
               new PropertyMetadata(FontWeights.Normal));

        public FontWeight TheFontWeight
        {
            get { return (FontWeight)GetValue(TheFontWeightProperty); }
            set { SetValue(TheFontWeightProperty, value); }
        }


        public static readonly DependencyProperty PreviewTextProperty =
           DependencyProperty.Register("PreviewText", typeof(string), typeof(FontStyleWindow),
               new PropertyMetadata(DefaultText));

        public string PreviewText
        {
            get { return (string)GetValue(PreviewTextProperty); }
            set { SetValue(PreviewTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayedTextProperty =
           DependencyProperty.Register("DisplayedText", typeof(string), typeof(FontStyleWindow),
               new PropertyMetadata(DefaultText,UpdateText));

        private static void UpdateText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var main = (FontStyleWindow) d;

            if (e.NewValue is bool)
            {
                main.PreviewText = !(bool)e.NewValue && !string.IsNullOrWhiteSpace(main.DisplayedText) ? main.DisplayedText.Shorten(42) : DefaultText;
                return;
            }

             var str = e.NewValue as string;
            if(str != null)
                main.PreviewVisibility = string.IsNullOrWhiteSpace(str)
                   ? Visibility.Collapsed : Visibility.Visible;
        
             main.PreviewText = !main.ShowExample && !string.IsNullOrWhiteSpace(str) ? str.Shorten(32) : DefaultText;

        }

        public static readonly DependencyProperty ShowExampleProperty =
           DependencyProperty.Register("ShowExample", typeof(bool), typeof(FontStyleWindow),
               new PropertyMetadata(false, UpdateText));

        public bool ShowExample
        {
            get { return (bool)GetValue(ShowExampleProperty); }
            set { SetValue(ShowExampleProperty, value); }
        }

        public string DisplayedText
        {
            get { return (string)GetValue(DisplayedTextProperty); }
            set { SetValue(DisplayedTextProperty, value); }
        }

        public static readonly DependencyProperty ColorsProperty =
     DependencyProperty.Register("PredefinedColors", typeof(ObservableCollection<Color>), typeof(FontStyleWindow), new PropertyMetadata(new ObservableCollection<Color>()));

        public ObservableCollection<Color> PredefinedColors
        {
            get { return (ObservableCollection<Color>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }


        public static readonly DependencyProperty TheTextDecorationProperty =
           DependencyProperty.Register("TheTextDecoration", typeof(TextDecorationCollection), typeof(FontStyleWindow),
               new PropertyMetadata(null));

        public TextDecorationCollection TheTextDecoration
        {
            get { return (TextDecorationCollection)GetValue(TheTextDecorationProperty); }
            set { SetValue(TheTextDecorationProperty, value); }
        }

        public static readonly DependencyProperty TextForgroundProperty =
           DependencyProperty.Register("TextForground", typeof(Brush), typeof(FontStyleWindow),
               new PropertyMetadata(null));

        public Brush TextForground
        {
            get { return (Brush)GetValue(TextForgroundProperty); }
            set { SetValue(TextForgroundProperty, value); }
        }


        public static readonly DependencyProperty TextBackgroundProperty =
           DependencyProperty.Register("TextBackground", typeof(Brush), typeof(FontStyleWindow),
               new PropertyMetadata(Brushes.Transparent));

        public Brush TextBackground
        {
            get { return (Brush)GetValue(TextBackgroundProperty); }
            set { SetValue(TextBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PreviewVisibilityProperty =
          DependencyProperty.Register("PreviewVisibility", typeof(Visibility), typeof(FontStyleWindow),
              new PropertyMetadata(Visibility.Collapsed));

        public Visibility PreviewVisibility
        {
            get { return (Visibility)GetValue(PreviewVisibilityProperty); }
            set { SetValue(PreviewVisibilityProperty, value); }
        }


        #endregion

      

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            PredefinedColors = MainWindow.GetColors();  // Holt die Farben aus der Config
            FilterBox.Focus();
        }

        private void CheckText(object sender, TextCompositionEventArgs e)
        {
            var check = Utilities.IsAllowedText(e.Text);
            e.Handled = !check;
        }

        private void ScrollToElement(object sender, SelectionChangedEventArgs e)
        {
            var sdr = ((ListBox) sender);
            if (sdr.IsFocused)
                return;

            var item = sdr.SelectedItem;

            sdr.ScrollIntoView(item);

        }

        private void UpdateColor(object sender, RoutedEventArgs routedEventArgs)
        {
            SetColor(true, (Button)sender);
        }


        private void UpdateColor2(object sender, MouseButtonEventArgs e)
        {
            SetColor(false, (Button)sender);
        }

        private void UpdateColorWithRemoveCheck(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightShift))
            {
                PredefinedColors.Remove(((SolidColorBrush) ((Button) sender).Background).Color);
                return;
            }
            SetColor(true, (Button) sender);
        }

        private void SetColor(bool fore,Button bord)
        {
            var brush = bord.Background;
            if (fore)
                TextForground = brush;
            else
                TextBackground = brush;
        }

        private void AddPredefined(object sender, RoutedEventArgs e)
        {
            var cd = new ColorWindow {Owner = this,PredefinedColors = PredefinedColors,StartColor = Colors.Black};

            if (cd.ShowDialog() == true && cd.Result != MessageBoxResult.Cancel)
            {
                PredefinedColors = cd.PredefinedColors;
                PredefinedColors.Add(cd.SelectedColor);
                MainWindow.StoreColors(PredefinedColors.ToList());
            }
        }

        private void CheckDouble(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount==2)
            {
                e.Handled = true;
                var cd = new ColorWindow {Owner = this, ShowPredefined = false,StartColor = ((SolidColorBrush) ((Button) sender).Background).Color};

                if (cd.ShowDialog() == true && cd.Result != MessageBoxResult.Cancel)
                {
                    Replace(((SolidColorBrush) ((Button) sender).Background).Color, cd.SelectedColor);
                    MainWindow.StoreColors(PredefinedColors.ToList());
                    SetColor(e.ChangedButton== MouseButton.Left,(Button)sender);
                }
            }
            else
                SetColor(e.ChangedButton == MouseButton.Left, (Button)sender);

        }

        public void Replace(Color c1, Color c2)
        {
            foreach (var color in PredefinedColors.ToList().Where(c=>c.ToHexColor().Equals(c1.ToHexColor())))
            {
                var ind = PredefinedColors.IndexOf(color);
                PredefinedColors[ind] = c2;
            }
        }

        private void resetFore(object sender, RoutedEventArgs e)
        {
            TextForground = _fore;
        }

        private void resetBack(object sender, RoutedEventArgs e)
        {
            TextBackground = _back;

        }

        private void FilterFonts(object sender, TextChangedEventArgs e)
        {
            var view = FindResource("Fonts") as CollectionViewSource;
            if (view == null) return;
 
            view.GroupDescriptions.Clear();
            
            if (string.IsNullOrEmpty(FilterBox.Text))
            {
               view.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
               ExpandParentAfterTime();
            }

            view.View.Filter = FilterMy;
        }

        private void ExpandParentAfterTime()
        {
            var t = new Timer(300);
            t.Elapsed += t_Elapsed;
            t.Enabled = true;
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ((Timer)sender).Enabled = false;
                ExpandParent(TheFontFamily);               
            });
        }

        public void ExpandParent(FontElement item)
        {
            if(item== null) return;
            var collitem =
                box.ItemContainerGenerator.Items.OfType<CollectionViewGroup>()
                    .FirstOrDefault(elem => elem.Name.Equals(item.Group));
            if (collitem == null) return;
            var gi = box.ItemContainerGenerator.ContainerFromItem(collitem) as GroupItem;
            if (gi == null) return;
            var elemem = gi.GetVisualChild<UIElement>() as Expander;
            if (elemem != null)
                elemem.IsExpanded = true;

          

            var t = new Timer(100);
            t.Elapsed+=(e,s)=> Dispatcher.Invoke(() =>
            {
                ((Timer)e).Enabled = false;
                var gi2 = box.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (gi2 != null) gi2.BringIntoView();
            });
            t.Enabled = true;
        }

        private bool FilterMy(object o)
        {
            var elem = o as FontElement;
            return string.IsNullOrEmpty(FilterBox.Text) ||
                   (elem != null &&
                    elem.Family.FamilyNames.Any(
                        fn => fn.Value.IndexOf(FilterBox.Text, StringComparison.CurrentCultureIgnoreCase) != -1));
        }

        private void ShowFirst(object sender, RoutedEventArgs e)
        {
            ExpandParent(TheFontFamily);
        }
    }
}
