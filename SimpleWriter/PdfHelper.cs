using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using ComplexWriter.MessageBoxes;
using ComplexWriter.Properties;
using ExtensionObjects;
using Microsoft.Win32;
using PdfSharp.Xps;
using Size = System.Windows.Size;

namespace ComplexWriter
{
    public class PdfHelper
    {
        public static bool PrintTextWithDialog(RichTextBox box, out List<Exception> exceptions, ObservableCollection<Color> predefColors,ref PrintSettings settings,string defaultFilename, ObservableCollection<Character> characters)
        {
            exceptions = new List<Exception>();

            var flowDocument = box.Document.CopyWithTickness(new Thickness(0));

            flowDocument.HidePageBreaker();
            

            var print = new PrintDialogHelper(flowDocument,characters,settings.FontFamily,settings.FontSize,settings.UseCharacters)
            {
                Owner = MainWindow.Global,
                CanUseWaterMark = settings.Watermark != null,
                CanUseCharacters = characters!= null && characters.Any(),
                PageCountElement = settings.PageCountElement,
                PredefinedColors = predefColors,
                BackgroundBrush = settings.BackgroundBrush,
                FontSizeInc = settings.FontSizeModifier,
                KeepOldNumbering = settings.UseOldNumbering,
                IsBlackAndWhite = settings.UseBlackAndWhite,
                UseWaterMark = settings.UseWatermark,
                UseCharacters = settings.UseCharacters,
                ShowPageCount = settings.ShowPageNumber
            };
            print.ShowDialog();

            if (print.Result == MessageBoxResult.Cancel)
                return false;

            settings.UseBlackAndWhite = print.IsBlackAndWhite;
            settings.ShowPageNumber = print.ShowPageCount;
            settings.UseOldNumbering = print.KeepOldNumbering;
            settings.UseWatermark = print.UseWaterMark;
            settings.UseCharacters = print.UseCharacters;
            settings.PageCountElement = print.PageCountElement;
            settings.BackgroundBrush = print.BackgroundBrush;
            settings.FontSizeModifier = print.FontSizeInc;

            if (print.Result == MessageBoxResult.Yes)
            {
                return true;
            }

            var defaultName = !string.IsNullOrEmpty(defaultFilename)? defaultFilename: MainWindow.GetDefaultName(box.GetText());
            string pdfFilename =GetFileName(defaultName);
            settings.PdfName = pdfFilename;
            settings.Document = print.Document;

            settings.From = print.From;
            settings.Till = print.Till;

            UiServices.SetBusyState();
            

            exceptions = PrintText(settings);
            settings.PageCountElement = print.PageCountElement;
            
            return !exceptions.Any();
        }

        private static List<Exception> PrintText(PrintSettings settings)
        {
            var exceptions = new List<Exception>();
            try
            {
                settings.Document = settings.Document.CopyWithTickness(new Thickness(0));
                if (settings.UseBlackAndWhite)
                {
                    var range = new TextRange(settings.Document.ContentStart, settings.Document.ContentEnd);
                    range.ApplyPropertyValue(FlowDocument.ForegroundProperty, Brushes.Black);
                    range.ApplyPropertyValue(FlowDocument.BackgroundProperty, Brushes.Transparent);
                }
               

                var tempFile = FlowDocumentToXps(settings, out exceptions);
                if (File.Exists(tempFile))
                {
                    PrintXpsAsPdf(tempFile, settings.PdfName, exceptions);
                }
                else
                {
                    exceptions.Add(new FileNotFoundException("Die zwischengespeicherte Datei konnte nicht gefunden werden"));
                }
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }

            return exceptions;
        }

        private static Watermark CheckGreyMark(Watermark mark, bool blackAndWhite)
        {
            if (mark == null|| mark.ImageSource == null) return null;

            if (!blackAndWhite) return mark;

            var newMark = new Watermark{Opacity = mark.Opacity,Size = mark.Size};
            byte[] markBytes;
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)mark.ImageSource));
                encoder.Save(stream);
               markBytes =stream.ToArray();
            }

            using (var stream = new MemoryStream(markBytes))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                var grayBitmapSource = new FormatConvertedBitmap();
                grayBitmapSource.BeginInit();
                grayBitmapSource.Source = bitmap;
                grayBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
                grayBitmapSource.EndInit();
                newMark.ImageSource = grayBitmapSource;
            }
            return newMark;
        }

        private static void PrintXpsAsPdf(string tempFile, string pdfFilename, List<Exception> exceptions)
        {
            try
            {
                XpsConverter.Convert(tempFile, pdfFilename, 0);
                Process.Start(pdfFilename);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            try
            {
                File.Delete(tempFile);
            }
            catch (AccessViolationException access)
            {
                exceptions.Add(access);
            }
        }

        public static Thickness PagePadding {get{return new Thickness(45, 50, 45, 45);}}

        private static string FlowDocumentToXps(PrintSettings settings, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();
            var tempFile = Path.Combine(Path.GetTempPath(), "Helper.xps");
            var tempFlow = settings.Document;//.CopyWithTickness(new Thickness(0));

            tempFlow.PagePadding = PagePadding;
           
            try
            {
                CreateXps(tempFlow, tempFile,settings);
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }

            return tempFile;
        }

        private static void CreateXps(FlowDocument tempFlow, string tempFile,PrintSettings settings)
        {
            //flowDocument, out exceptions, CheckGreyMark(mark, blackAndWhite), showNumber, face, blackAndWhite ? Brushes.Black : brush, useLeading, fontSize, from, till, useOldNumbering, blackAndWhite ? Brushes.Transparent : backBrush
            //
            var stream = new MemoryStream();

            using (Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
            {
                XpsDocument xpsDoc;
                using (xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                {
                    var rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                    DocumentPaginator paginator = ((IDocumentPaginatorSource) tempFlow).DocumentPaginator;
                    paginator = new DocumentPaginatorWrapper(paginator)
                    {
                        ContentPageSize = new Size(paginator.PageSize.Width, paginator.PageSize.Height),
                        ContentMargin = new Size(48, 48),
                        Mark = settings.UseWatermark ? CheckGreyMark(settings.Watermark,settings.UseBlackAndWhite):null,
                        FooterTypeface = new Typeface(settings.PageCountElement.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                        ForegroundBrush = settings.UseBlackAndWhite ? Brushes.Black : settings.PageCountElement.ForgroundBrush,
                        ShowNumbering = settings.ShowPageNumber,
                        UseLeadingZero =  settings.PageCountElement.UseLeadingZero,
                        FooterFontSize = settings.PageCountElement.FontSize,
                        FirstPritedPage = settings.From,
                        LastPrintedPage = settings.Till,
                        UseOldNumbering = settings.UseOldNumbering,
                        BackgroundBrush = settings.UseBlackAndWhite ? Brushes.White : settings.BackgroundBrush
                    };
                    ((DocumentPaginatorWrapper) paginator).ComputeValues();
                    rsm.SaveAsXaml(paginator);
                    rsm.Commit();
                }
            }
            stream.Position = 0;
            using (var fileStream = new FileStream(tempFile, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                stream.CopyTo(fileStream);
            }
        }
        
        private static string GetFileName(string defaultName)
        {

            var dlg = new SaveFileDialog
            {
                FileName = Path.GetFileNameWithoutExtension(defaultName),
                DefaultExt = "pdf",
                Filter = "pdfDateien|*.pdf",
                CheckFileExists = false
            };

            if (!string.IsNullOrEmpty(Settings.Default.DefaultExportPath))
                dlg.InitialDirectory = Settings.Default.DefaultExportPath;
            if (dlg.ShowDialog() == false)
            {
                return string.Empty;
            }
            Settings.Default.DefaultExportPath = Path.GetDirectoryName(dlg.FileName);
            Settings.Default.Save();

            return dlg.FileName;
        }

        
    }
}
