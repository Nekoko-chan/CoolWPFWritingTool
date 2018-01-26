using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ComplexWriter.Properties;
using Microsoft.Win32;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class FileCombiner
    {
        public FileCombiner()
        {
            InitializeComponent();

            var names = GetFilenames();
            Box.AppendText(_baseFolder+" ("+names.Count()+")"+Environment.NewLine);

            var newdir = _baseFolder + "_Copy";
            if (!Directory.Exists(newdir))
                Directory.CreateDirectory(newdir);
            ConvertFiles(names);
        }

        private string _baseFolder;

        private IEnumerable<string> GetFilenames()
        {
            var dlg = new FolderBrowserDialog {SelectedPath = Settings.Default.DefaultFolder};

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return Enumerable.Empty<string>();

            _baseFolder = dlg.SelectedPath;
            return GetFilenames(dlg.SelectedPath);

        }

        private IEnumerable<string> GetFilenames(IEnumerable<string> selecteddirectories)
        {
            return selecteddirectories.SelectMany(GetFilenames);
        }

        private IEnumerable<string> GetFilenames(string selectedPath)
        {
            var files = Directory.GetFiles(selectedPath).Where(e => e.EndsWith("etf"));
            if (Directory.GetDirectories(selectedPath).Any())
                return files.Concat(GetFilenames(Directory.GetDirectories(selectedPath)));
            return files;
        }


        public void ConvertFiles(IEnumerable<string>files)
        {
            foreach (var file in files)
            {
                ConvertFile(file);
    
            }
        }

        public void ConvertFile(string filename)
        {
            try
            {
                MessageListener.Instance.ReceiveMessage($"{Path.GetFileName(filename)} copied");
                var newRoot = _baseFolder + "_Copy";
                var newPath = newRoot + filename.Substring(_baseFolder.Length);
                Box.AppendText(newPath + Environment.NewLine);

                if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));

                var file = TextFile.Load(filename);

                var newFile = CreateWriterFileFromTextfile(file);
                bool output;
                newFile.Save(newPath, out output);
            }
            catch (Exception e)
            {
                Box.AppendText($"{filename} not copied because {e.Message}{Environment.NewLine}");
            }

        }

        private static Writer.Data.TextFile CreateWriterFileFromTextfile(TextFile file)
        {
            var writerFileFromThis = new Writer.Data.TextFile
            {
                Colors = file.Colors,
                Tags = file.Tags,
                CaretOffset = file.CaretOffset,
                CharacterNames = file.CharacterNames,
                Characters = CopyCharacter(file.Characters),
                ClosingQuote = file.ClosingQuote,
                Document = file.Document,
                Filepath = file.Filepath,
                IncFontWhenPrinting = file.IncFontWhenPrinting,
                IsChangeString = file.IsChangeString,
                IsExtended = file.IsExtended,
                OpeningQuote = file.OpeningQuote,
                PageCountElement = CopyPageCountElement(file.PageCountElement),
                Password = file.Password,
                PasswordQuestion = file.PasswordQuestion,
                PrintBackground = file.PrintBackground,
                PrintBackgroundString = file.PrintBackgroundString,
                QuotationButtonState = CopyButtonState(file.QuotationButtonState),
                ReadOnly = file.ReadOnly,
                ScrollOffset = file.ScrollOffset,
                ShowPageNumber = file.ShowPageNumber,
                SingleClosingQuote = file.SingleClosingQuote,
                SingleOpeningQuote = file.SingleOpeningQuote,
                SpellCheckEnabled = file.SpellCheckEnabled,Styles = CopyStyles(file.Styles),
                UseBlackAndWhite = file.UseBlackAndWhite,
                UseCharacters = file.UseCharacters,
                UseOldNumbering = file.UseOldNumbering,
                UseWatermark = file.UseWatermark,
                Watermark = CopyWatermark(file.Watermark)
            };


            return writerFileFromThis;
        }

        private static Writer.Data.Watermark CopyWatermark(Watermark watermark)
        {
            if (watermark == null) return null;
            return new Writer.Data.Watermark
            {
                ImageSource = watermark.ImageSource,
                Opacity = watermark.Opacity,
                Size = watermark.Size
            };
        }

        private static Writer.Data.ComplexStyles CopyStyles(ComplexStyles styles)
        {
            if(styles == null) return null;
            var styls = new Writer.Data.ComplexStyles {Title = styles.Title};
            foreach (var news in styles.Styles.Select(CopyComplexStyle))
            {
                styls.Styles.Add(news);
            }
            return styls;
        }

        private static Writer.Data.ButtonState CopyButtonState(ButtonState quotationButtonState)
        {
            var text = quotationButtonState.ToString();

            Writer.Data.ButtonState ret;

            return Enum.TryParse(text, true, out ret) ? ret : Writer.Data.ButtonState.State1;
        }

        private static Writer.Data.PageCountElement CopyPageCountElement(PageCountElement pageCountElement)
        {
            if (pageCountElement == null) return null;

            return new Writer.Data.PageCountElement
            {
                FontFamily = pageCountElement.FontFamily,
                FontSize = pageCountElement.FontSize,
                ForgroundBrush = pageCountElement.ForgroundBrush,
                ForgroundBrushString = pageCountElement.ForgroundBrushString,
                UseLeadingZero = pageCountElement.UseLeadingZero
            };
        }

        private static Writer.Data.ComplexStyle CopyComplexStyle(ComplexStyle defaultStyle)
        {
            if (defaultStyle == null) return null;
            var conv = new ComplexStyleConverter();

            var str = conv.ConvertTo(defaultStyle, typeof (string));

            var conv2 = new Writer.Data.ComplexStyleConverter();
            return conv2.ConvertFrom(str) as Writer.Data.ComplexStyle;
        }

        private static ObservableCollection<Writer.Data.Character> CopyCharacter(ObservableCollection<Character> characters)
        {
            if (characters == null) return null;
            var obs = new ObservableCollection<Writer.Data.Character>();
            foreach (var character in characters)
            {
                var chara = new Writer.Data.Character
                {
                    Description = character.Description,
                    ImageBuffer = character.ImageBuffer,
                    IsMain = character.IsMain,
                    Name = character.Name,
                    Type = GetTypeValue(character.Type)
                };
                obs.Add(chara);
            }
            return obs;
        }

        private static Writer.Data.NameType GetTypeValue(NameType type)
        {
            var text = type.ToString();

            Writer.Data.NameType ret;

            return Enum.TryParse(text, true, out ret) ? ret : Writer.Data.NameType.Unset;
        }
    }
}
