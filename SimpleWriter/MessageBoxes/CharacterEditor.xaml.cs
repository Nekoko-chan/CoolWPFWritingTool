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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ComplexWriter.Commands;
using ComplexWriter.Properties;
using Microsoft.Win32;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xamle
    /// </summary>
    public partial class CharacterEditor
    {
        public CharacterEditor()
        {
            InitializeComponent();
            Loaded += LoadWindow;
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = (MessageBoxResult)((Button) sender).Tag;
            Result = messageBoxResult;
        }
      

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            Box.AddHandler(ContextMenuOpeningEvent, new ContextMenuEventHandler(RichTextBox_ContextMenuOpening), true);
        }

        public static readonly DependencyProperty CharacterProperty =
        DependencyProperty.Register("Character", typeof(Character), typeof(CharacterEditor),
        new PropertyMetadata(null));
 

        public Character Character
        {
            get { return (Character)GetValue(CharacterProperty); }
            set { SetValue(CharacterProperty, value); }
        }

        public IEnumerable<Character> CharacterNames { get; set; }

        private void CanExecuteRerollCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = !string.IsNullOrEmpty(Clipboard.GetText());
            }
            catch (Exception exc)
            {
                MainWindow.Global.AddException(exc);
            }
        }

        private void ExecuteRerollCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PasteTextAsTextOnly();
            e.Handled = true;
        }

        private void PasteTextAsTextOnly()
        {
            try
            {
                string text = Clipboard.GetText();

                Box.SelectedText = text;
            }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }
        }

        private void RichTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            IEnumerable<Control> standard = GetStandardCommands();
            IEnumerable<Control> names = GetNameCommands(false, false);
            IEnumerable<Control> names2 = GetNameCommands(true, false);
            IEnumerable<Control> names3 = GetNameCommands(false, true);
            IEnumerable<Control> names4 = GetNameCommands(true,true);
            IList<Control> spell = GetSpellingSuggestions();
            ContextMenu.Items.Clear();

            ContextMenu.Items.Add(BuildSeperator(Properties.Resources.Function));

            foreach (Control menuItem in standard)
            {
                ContextMenu.Items.Add(menuItem);
            }
            if (spell.Any())
            {
                ContextMenu.Items.Add(BuildSeperator(Properties.Resources.CorrectText));
                foreach (Control menuItem in spell)
                {
                    ContextMenu.Items.Add(menuItem);
                }
            }
            if (names.Any())
            {
                ContextMenu.Items.Add(BuildSeperator(Properties.Resources.Names));
                var menu = new MenuItem { Header = Properties.Resources.Names, Icon = BuildIcon("IDIcon"), Style = MainWindow.Global.FindResource("menuItem") as Style };
                foreach (Control menuItem in names)
                {
                    menu.Items.Add(menuItem);
                }
                ContextMenu.Items.Add(menu);
                var menu2 = new MenuItem { Header = Properties.Resources.NameWithSpaceEnd, Icon = BuildIcon("IDIcon"), Style = MainWindow.Global.FindResource("menuItem") as Style };
                foreach (Control menuItem in names2)
                {
                    menu2.Items.Add(menuItem);
                }
                ContextMenu.Items.Add(menu2);

                var menu3 = new MenuItem { Header = Properties.Resources.NameWithSpaceStart, Icon = BuildIcon("IDIcon"), Style = MainWindow.Global.FindResource("menuItem") as Style };
                foreach (Control menuItem in names3)
                {
                    menu3.Items.Add(menuItem);
                }
                ContextMenu.Items.Add(menu3);

                var menu4 = new MenuItem { Header = Properties.Resources.NameWithSpaces, Icon = BuildIcon("IDIcon"), Style = MainWindow.Global.FindResource("menuItem") as Style };
                foreach (Control menuItem in names4)
                {
                    menu4.Items.Add(menuItem);
                }
                ContextMenu.Items.Add(menu4);
            }
        }


        private MenuItem BuildItem(ICommand command, string iconName, string header)
        {
            return new MenuItem
            {
                Command = command,
                Icon = BuildIcon(iconName),
                Header = header,
                Style = MainWindow.Global.FindResource("menuItem") as Style
            };
        }


        private MenuItem BuildItem(RoutedEventHandler handler, string iconName, string header)
        {
            var buildItem = new MenuItem
            {
                Icon = BuildIcon(iconName),
                Header = header,
                Style = MainWindow.Global.FindResource("menuItem") as Style
            };
            buildItem.Click += handler;
            return buildItem;
        }

        private IList<Control> GetSpellingSuggestions()
        {
            var spellingSuggestions = new List<Control>();
            SpellingError spellingError = Box.GetSpellingError(Box.CaretIndex);

            if (spellingError == null)
                return new List<Control>();

            var start = Box.GetSpellingErrorStart(Box.CaretIndex);
            var length = Box.GetSpellingErrorLength(Box.CaretIndex);
            string text = start < 0 || start + length > Box.Text.Length ? "" : Box.Text.Substring(start, length);

            List<string> suggestions = spellingError.Suggestions.ToList();

            if (suggestions.Count() <= 15)
            {
                spellingSuggestions.AddRange(suggestions.Select(str => new MenuItem
                {
                    Header = str,
                    Icon = BuildIcon("DictionaryEntry"),
                    Command = EditingCommands.CorrectSpellingError,
                    CommandParameter = str,
                    ToolTip = BuildToolTip(string.Format(Properties.Resources.ReplaceText, text, str)),
                    Style = MainWindow.Global.FindResource("menuItem") as Style,
                    CommandTarget = Box
                }));
            }
            else
            {
                var possibles = new MenuItem
                {
                    Header = Properties.Resources.PossibleReplacements,
                    Icon = BuildIcon("DictionaryEntry"),
                    Command = EditingCommands.IgnoreSpellingError,
                    CommandTarget = Box,
                    Style = MainWindow.Global.FindResource("menuItem") as Style
                };

                possibles.Items.AddRange(suggestions.Select(str => new MenuItem
                {
                    Header = str,
                    Icon = BuildIcon("DictionaryEntry"),
                    ToolTip = BuildToolTip(string.Format(Properties.Resources.ReplaceText, text, str)),
                    Command = EditingCommands.CorrectSpellingError,
                    CommandParameter = str,
                    Style = MainWindow.Global.FindResource("menuItem") as Style,
                    CommandTarget = Box
                }));
                spellingSuggestions.Add(possibles);
                possibles.Items.Add(BuildSeperator(string.Empty));

                MenuItem addToDictionaryBut = BuildAddToDictionary(false);
                possibles.Items.Add(addToDictionaryBut);

                MenuItem addToNameDictionaryBut = BuildAddToDictionary(true);
                possibles.Items.Add(addToNameDictionaryBut);


                possibles.Items.Add(BuildSeperator(string.Empty));
            }

            if (suggestions.Any())
                spellingSuggestions.Add(BuildSeperator(string.Empty));

            var ignoreAll = new MenuItem
            {
                Header = Properties.Resources.IgnorenAll,
                Icon = BuildIcon("IgnoreEntry"),
                Command = EditingCommands.IgnoreSpellingError,
                CommandTarget = Box,
                ToolTip =
                    BuildToolTip(
                        Properties.Resources.IgnoreAllHint),
                Style = MainWindow.Global.FindResource("menuItem") as Style
            };

            spellingSuggestions.Add(ignoreAll);

            MenuItem addToDictionary = BuildAddToDictionary(false);
            spellingSuggestions.Add(addToDictionary);

            MenuItem addToNameDictionary = BuildAddToDictionary(true);
            spellingSuggestions.Add(addToNameDictionary);

            spellingSuggestions.Add(BuildSeperator(string.Empty));
            
            return spellingSuggestions;
        }
     

        private MenuItem BuildAddToDictionary(bool isName)
        {
            var text = isName ? Properties.Resources.EntryIsAddedToNames : Properties.Resources.EntryIsAddedToDictionary;
            var addToDictionary = new MenuItem
            {
                Header =GetHeader(Box.CaretIndex, isName),
                Icon = BuildIcon("bookOpenEdit.png"),
                Command = EditingCommands.IgnoreSpellingError,
                ToolTip = BuildToolTip(text),
                CommandTarget = Box,
                Style = MainWindow.Global.FindResource("menuItem") as Style
            };


            addToDictionary.Click += (o, rea) => AddToDictionary(Box.CaretIndex, isName ? MainWindow.Global.NameDict :MainWindow.Global.CurrentTextIsEnglish()? MainWindow.Global.CustomDictEnglish: MainWindow.Global.CustomDict);
            return addToDictionary;
        }

        private object GetHeader(int index, bool name)
        {
            var start = Box.GetSpellingErrorStart(index);
            var length = Box.GetSpellingErrorLength(index);
            string text = start < 0 || start + length > Box.Text.Length ? "" : Box.Text.Substring(start, length);

            if (string.IsNullOrEmpty(text))
                return Properties.Resources.EntryIsAddedToDictionary;

            var ext = name ? Properties.Resources.SpecialEntryAddedToNames: Properties.Resources.SpecialEntryAddedToDictionary;
            return string.Format(ext, text, MainWindow.Global.TextLanguage);
        }

        private Separator BuildSeperator(string text)
        {
            return new Separator
            {
                Tag = text,
                Style = MainWindow.Global.FindResource(string.IsNullOrEmpty(text) ? "shortsep" : "titlesep") as Style
            };
        }

        private void AddToDictionary(int index, string dict)
        {
            
              var start = Box.GetSpellingErrorStart(index);
            var length = Box.GetSpellingErrorLength(index);
            string text = start < 0 || start + length > Box.Text.Length ? "" : Box.Text.Substring(start, length);

            if(string.IsNullOrEmpty(text))
                return;

            if (!File.Exists(dict))
            {
                var stream = File.Create(dict);
                stream.Close();
            }

            if(dict.Equals(MainWindow.Global.NameDict)&&!File.Exists(MainWindow.Global.NameDictUs))
            {
                var stream = File.Create(MainWindow.Global.NameDictUs);
                stream.Close();
            }

            using (var streamWriter = new StreamWriter(dict, true, Encoding.Unicode))
            {
                streamWriter.WriteLine(text);
            }

            if (dict.Equals(MainWindow.Global.NameDict))
            {
                using (var streamWriter = new StreamWriter(MainWindow.Global.NameDictUs, true, Encoding.Unicode))
                {
                    streamWriter.WriteLine(text);
                }
            }
        }



        private IEnumerable<Control> GetNameCommands(bool withSpaceAfter, bool withSpaceBefore)
        {
            var commands = new List<Control>();

            foreach (var characterName in CharacterNames)
            {
                var men = new MenuItem
                {
                    Tag = $"{(withSpaceBefore ? " " : string.Empty)}{characterName.Name }{(withSpaceAfter? " ":string.Empty)}",
                    Header = characterName.Name,
                    Style = MainWindow.Global.FindResource("menuItem") as Style,
                    Icon = BuildIcon(MainWindow.GetIconName(characterName.Type)),
                    ToolTip = BuildToolTip(characterName.Description)

                };
                    men.Click += AddName;

                commands.Add(men);
            }

            return commands;
        }

        private void AddName(object sender, RoutedEventArgs e)
        {
            var itm = (MenuItem)sender;
            Box.SelectedText = (string)itm.Tag;
            Box.SelectionStart += ((string)itm.Tag).Length;
        }
       

        private IEnumerable<Control> GetStandardCommands()
        {
            var standardCommands = new List<Control>();

            var item = new MenuItem
            {
                Command = ApplicationCommands.Cut,
                Icon = BuildIcon("CutIcon"),
                IsEnabled = true,
                Style = MainWindow.Global.FindResource("menuItem") as Style
            };
            standardCommands.Add(item);

            item = new MenuItem
            {
                Command = ApplicationCommands.Copy,
                Icon = BuildIcon("CopyIcon"),
                Style = MainWindow.Global.FindResource("menuItem") as Style
            };
            standardCommands.Add(item);

            item = new MenuItem
            {
                Command = ApplicationCommands.Paste,
                Icon = BuildIcon("PasteIcon"),
                IsEnabled = true,
                Style = MainWindow.Global.FindResource("menuItem") as Style
            };
            standardCommands.Add(item);

            try
            {
                if (!Clipboard.ContainsText()) return standardCommands;
                }
            catch (Exception exception)
            {
                MainWindow.Global.AddException(exception);
            }

            return standardCommands;
        }

        private object BuildIcon(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            if (s.IndexOf(".", StringComparison.Ordinal) == -1)
                return MainWindow.Global.FindResource(s) as Brush;

            object br = Utilities.GetImageBrushFromSource(s);

            return br;
        }


        private object BuildToolTip(string str)
        {
            var tip = new ToolTip
            {
                Style = MainWindow.Global.FindResource("tipTest") as Style,
                Content = str
            };

            return tip;
        }

        private void LoadNewPicture(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog
            {
                Filter = Properties.Resources.ImageFilter,
                DefaultExt = "png"
            };

            if(open.ShowDialog(this)==false) return;

            var fileStream =new FileStream(open.FileName, FileMode.Open, FileAccess.Read);

            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = fileStream;
            img.EndInit();

            Character.ImageBuffer = Utilities.ConvertImageToByteArray(img);
        }

        private void ClearPicture(object sender, RoutedEventArgs e)
        {
            Character.ImageBuffer = null;
        }
    }
}
