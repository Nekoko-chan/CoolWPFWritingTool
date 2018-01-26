using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ComplexWriter.Properties;
using ExtensionObjects;
using Writer.Data;
using Xceed.Wpf.Toolkit;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class PrintDialogHelper
    {
        public PrintDialogHelper(FlowDocument document, ObservableCollection<Character> characters,FontFamily family,double fsize, bool addCharacters) : this()
        {
            _originalDocument = document;
            _characters = characters;
            _family = family;
            _fsize = fsize;
            BuildDocument();
        }

        private void AppendCharactersToDocument()
        {

            if (_characters == null || !_characters.Any()) return;

            var width = CalculateWidth(_characters,_family,_fsize);
            var width2 = Document.PageWidth - width - 190;

            Document.Blocks.Add(
             new Paragraph(new Run(Properties.Resources.Characters)
             {
                 FontFamily = _family,
                 FontSize = _fsize + 10,
                 FontWeight = FontWeights.Bold
             })
             { BreakPageBefore = true });

            var enumerable = Enum.GetValues(typeof(NameType));

            var list = enumerable.Cast<NameType>().OrderByDescending(elem=>elem);

            foreach (var value in list)
            {
                var ch = _characters.Where(elem => elem.Type.Equals(value));
                if(!ch.Any()) continue;

                var tabl = BuildCharacterTypeTable(width, width2,ch);
                Document.Blocks.Add(tabl);
            }
            
        }

        private Table BuildCharacterTypeTable(double width, double width2, IEnumerable<Character> characters)
        {    
            var title = new EnumDescriptionTypeConverter(typeof(NameType)).ConvertTo(null,new CultureInfo(Settings.Default.Language),characters.First().Type,typeof(string));

            Document.Blocks.Add(
                new Paragraph(new Run((string)title)
                {
                    FontFamily = _family,
                    FontSize = _fsize + 5
                }));
            var tabl = new Table
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                FontFamily = _family,
                FontSize = _fsize
            };
            var anyCharacterHasImage =  characters.Any(elem=>elem.Image!= null);

            var realWidth = width2;
            if(anyCharacterHasImage)
                realWidth-=80;
            tabl.Columns.Add(new TableColumn {Width = new GridLength(width + 20)});
            
            if (anyCharacterHasImage)
                tabl.Columns.Add(new TableColumn { Width = new GridLength(80) });

            tabl.Columns.Add(new TableColumn {Width = new GridLength(realWidth)});

            var rowgroup = new TableRowGroup();
            tabl.RowGroups.Add(rowgroup);


            var count = 0;
            MakeRow("Name", "Beschreibung", Brushes.Black, Brushes.White, rowgroup,anyCharacterHasImage,null);
            foreach (var character in characters.OrderBy(elem=>$"{(elem.IsMain?"00":"99")}_{elem.Name}"))
            {
                var bg = count%2 == 0 ? Brushes.White : Brushes.GhostWhite;
                count++;
                MakeRow(character.Name, character.Description, bg, Brushes.Black, rowgroup,anyCharacterHasImage,character.Image);
            }
            return tabl;
        }

        private static void MakeRow(string name, string description, SolidColorBrush bg, SolidColorBrush fg, TableRowGroup rowgroup,bool buildImageCell,BitmapSource image)
        {
            var cell1 = new TableCell(new Paragraph(new Run(name)))
            {
                Background = bg,
                Foreground = fg,
                Padding = new Thickness(5, 3, 5, 3)
            };
            var row = new TableRow();
            row.Cells.Add(cell1);

            if (buildImageCell)
            {
                TableCell celltable;
                if (image == null)
                    celltable =  new TableCell
                    {
                        Background = bg,
                        Foreground = fg,
                        Padding = new Thickness(5, 3, 5, 3)
                    };
                else
                {
                    var ima = new Image {Source = image,Width = 80, Height = 80, StretchDirection = StretchDirection.DownOnly};
                    var img = new Span();
                    img.Inlines.Add(ima);

                    celltable = new TableCell(new Paragraph(img))
                    {
                        Background = bg,
                        Foreground = fg,
                        Padding = new Thickness(5, 3, 5, 3)
                    };
                }
                row.Cells.Add(celltable );
            }

            var cell2 = new TableCell(new Paragraph(new Run(description)))
            {
                Background = bg,
                Foreground = fg,
                Padding = new Thickness(5, 3, 5, 3)
            };
            row.Cells.Add(cell2);

            rowgroup.Rows.Add(row);
        }

        private double CalculateWidth(ObservableCollection<Character> characters, FontFamily fam, double size)
        {
            return characters.Max(el => MeasureString(el.Name, fam,size).Width); 
        }

        public PrintDialogHelper()
        {
            InitializeComponent();
        }

        private readonly FlowDocument _originalDocument;
        private readonly ObservableCollection<Character> _characters;
        private readonly FontFamily _family;
        private readonly double _fsize;

        public int From
        {
            get { return GetLowestValue(); }
        }

        private int GetLowestValue()
        {
            if(fromUpDown.Value == null)
                return 0;
            return tillUpDown.Value == null ? fromUpDown.Value.Value-1 : Math.Min(fromUpDown.Value.Value, tillUpDown.Value.Value)-1;
        }

        public bool KeepOldNumbering
        {
            get { return OldNumbering.IsChecked == true; }
            set { OldNumbering.IsChecked = value; }
        }

        public int Till
        {
            get { return GetHighestValue(); }
        }

        private int GetHighestValue()
        {
            if (tillUpDown.Value == null)
                return PageViewer.PageViewer.PageCount-1;
            return fromUpDown.Value == null ? tillUpDown.Value.Value-1 : Math.Max(fromUpDown.Value.Value, tillUpDown.Value.Value)-1; 
        }


        public static readonly DependencyProperty DocumentProperty =
        DependencyProperty.Register("Document", typeof(FlowDocument), typeof(PrintDialogHelper),
            new PropertyMetadata(null,OnDocumentChanged));


        public FlowDocument Document
        {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public static readonly DependencyProperty IsBlackAndWhiteProperty =
       DependencyProperty.Register("IsBlackAndWhite", typeof(bool), typeof(PrintDialogHelper),
           new PropertyMetadata(false));


        public bool IsBlackAndWhite
        {
            get { return (bool)GetValue(IsBlackAndWhiteProperty); }
            set { SetValue(IsBlackAndWhiteProperty, value); }
        }

        public static readonly DependencyProperty UseWaterMarkProperty =
   DependencyProperty.Register("UseWaterMark", typeof(bool), typeof(PrintDialogHelper),
       new PropertyMetadata(true));

        public static readonly DependencyProperty CanUseWaterMarkProperty =
   DependencyProperty.Register("CanUseWaterMark", typeof(bool), typeof(PrintDialogHelper),
       new PropertyMetadata(false));


        public bool CanUseWaterMark
        {
            get { return (bool)GetValue(CanUseWaterMarkProperty); }
            set { SetValue(CanUseWaterMarkProperty, value); }
        }


        public bool UseWaterMark
        {
            get { return (bool)GetValue(UseWaterMarkProperty); }
            set { SetValue(UseWaterMarkProperty, value); }
        }

        public static readonly DependencyProperty UseCharactersProperty =
 DependencyProperty.Register("UseCharacters", typeof(bool), typeof(PrintDialogHelper),
     new PropertyMetadata(true, UseCharactersUpdated));

        private static void UseCharactersUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var helper = (PrintDialogHelper) d;

            helper.BuildDocument();
        }

        public void BuildDocument()
        {
            Document = _originalDocument.CopyWithTickness(new Thickness(0));
            if (UseCharacters)
                AppendCharactersToDocument();
            Document.ChangeFontSize(FontSizeInc);
        }


        public static readonly DependencyProperty CanUseCharactersProperty =
   DependencyProperty.Register("CanUseCharacters", typeof(bool), typeof(PrintDialogHelper),
       new PropertyMetadata(false));


        public bool CanUseCharacters
        {
            get { return (bool)GetValue(CanUseCharactersProperty); }
            set { SetValue(CanUseCharactersProperty, value); }
        }


        public bool UseCharacters
        {
            get { return (bool)GetValue(UseCharactersProperty); }
            set { SetValue(UseCharactersProperty, value); }
        }

        public static readonly DependencyProperty ShowPageCountProperty =
   DependencyProperty.Register("ShowPageCount", typeof(bool), typeof(PrintDialogHelper),
       new PropertyMetadata(true));


        public bool ShowPageCount
        {
            get { return (bool)GetValue(ShowPageCountProperty); }
            set { SetValue(ShowPageCountProperty, value); }
        }

        public static readonly DependencyProperty PageCountElementProperty =
   DependencyProperty.Register("PageCountElement", typeof(PageCountElement), typeof(PrintDialogHelper),
       new PropertyMetadata(null));

        public PageCountElement PageCountElement
        {
            get { return (PageCountElement)GetValue(PageCountElementProperty); }
            set { SetValue(PageCountElementProperty, value); }
        }


        public static readonly DependencyProperty ColorsProperty =
     DependencyProperty.Register("PredefinedColors", typeof(ObservableCollection<Color>), typeof(PrintDialogHelper), new PropertyMetadata(new ObservableCollection<Color>()));

        public ObservableCollection<Color> PredefinedColors
        {
            get { return (ObservableCollection<Color>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
   DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(PrintDialogHelper),
       new PropertyMetadata(null));
        
        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public int FontSizeInc { get; set; }

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dlg = d as PrintDialogHelper;

            if (dlg == null)
                return;

            dlg.Document.PageHeight = 1122.72;
            dlg.Document.PageWidth = 793.28;
            dlg.Document.PagePadding = PdfHelper.PagePadding;
            dlg.Document.ColumnGap = 0;
            dlg.Document.ColumnWidth = 793.28;
   
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            if (win == null)
                return;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button) sender).Tag;
        }


        private void CloseWithTagSave(object sender, RoutedEventArgs e)
        {
            if(QuestionBox.ShowMessage(this,Properties.Resources.SaveOnlyQuestion,Properties.Resources.ReallyClosing,false) == MessageBoxResult.Yes)
                Result = (MessageBoxResult)((Button)sender).Tag;
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            if (FontSizeInc == 0) return;

            FontIncBlock.Text = string.Format(Properties.Resources.DeviationInFontSize,0);
            BuildDocument();
        }

        private void IncFont(object sender, RoutedEventArgs e)
        {
            FontSizeInc++;
            BuildDocument();
            FontIncBlock.Text = FontSizeInc == 0 ? string.Empty:string.Format(Properties.Resources.DeviationInFontSize, FontSizeInc);
        }

        private void DecFont(object sender, RoutedEventArgs e)
        {
            FontSizeInc--;
            BuildDocument();
            FontIncBlock.Text = FontSizeInc == 0 ? string.Empty : string.Format(Properties.Resources.DeviationInFontSize, FontSizeInc);
        }

        private void SetOption(object sender, RoutedEventArgs e)
        {
            var pageCountElement = new PageCountElement
            {
                FontFamily = PageCountElement.FontFamily,
                ForgroundBrush = PageCountElement.ForgroundBrush,
                UseLeadingZero = PageCountElement.UseLeadingZero,
                FontSize = PageCountElement.FontSize
            };

            var dlg = new PageCountSettings{PageCountElement = pageCountElement, Owner = this, PredefinedColors = PredefinedColors};

            if (dlg.ShowDialog() == false || dlg.Result == MessageBoxResult.Cancel) return;
            
            PageCountElement = dlg.PageCountElement;

        }

        private Size MeasureString(string candidate, FontFamily family, double fontsize)
        {
            var formattedText = new FormattedText(
                candidate,
                new CultureInfo(Settings.Default.Language),
                FlowDirection.LeftToRight,
                new Typeface(family,FontStyles.Normal, FontWeights.Normal, FontStretches.SemiCondensed),
                fontsize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void CheckText(object sender, TextCompositionEventArgs e)
        {
            var check = Utilities.IsAllowedText(e.Text);
            e.Handled = !check;
        }

        private void SetInitialValueFrom(object sender, DependencyPropertyChangedEventArgs e)
        {
            var inte =((IntegerUpDown) sender);
            if(inte.IsHitTestVisible)
            {
                inte.Value = 1;
                inte.Maximum = PageViewer.PageViewer.PageCount;
            }
            else
            {
                inte.Value = null;
            }
        }

        private void SetInitialValueTill(object sender, DependencyPropertyChangedEventArgs e)
        {
            var inte = ((IntegerUpDown)sender);
            if (inte.IsHitTestVisible)
            {
                inte.Value = PageViewer.PageViewer.PageCount;
                inte.Maximum = PageViewer.PageViewer.PageCount;
            }
            else
            {
                inte.Value = null;
            }
        }

        private void ChangeBackground(object sender, RoutedEventArgs e)
        {
            var dlg = new PrintBackgroundDialog(BackgroundBrush){Owner = this,PredefinedColors = MainWindow.GetColors()};

            if (dlg.ShowDialog() == true && dlg.Result == MessageBoxResult.OK)
                BackgroundBrush = dlg.ResultBrush;
        }


        private void ResetFontInc(object sender, RoutedEventArgs e)
        {
            FontSizeInc = 0;
            BuildDocument();
            FontIncBlock.Text = "";
        }
    }

}
