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
using System.Windows.Input;
using System.Windows.Media;
using ComplexWriter.Properties;
using ExtensionObjects;
using Microsoft.Win32;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class DictionaryEditor
    {
        public DictionaryEditor()
        {
            InitializeComponent();
        }

        public string Symbol     { get; set; }

        public FontFamily DefaultFamily { get; set; }

        public static readonly DependencyProperty WordsProperty =
          DependencyProperty.Register("Words", typeof(ObservableCollection<DictionaryItem>), typeof(DictionaryEditor),
              new PropertyMetadata(new ObservableCollection<DictionaryItem>()));

        public ObservableCollection<DictionaryItem> Words
        {
            get { return (ObservableCollection<DictionaryItem>)GetValue(WordsProperty); }
            set { SetValue(WordsProperty, value); }
        }

        public string Dictionary { get; set; }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = (MessageBoxResult)((Button) sender).Tag;

            if (messageBoxResult == MessageBoxResult.OK)
            {
                SaveCustomDictionary();
            }

            Result = messageBoxResult;
        }

        private void SaveCustomDictionary()
        {

            using (var streamWriter = new StreamWriter(Dictionary, false, Encoding.Unicode))
            {
                foreach (var dictionaryItem in Words.Where(elem => !elem.IsForDeletion && !string.IsNullOrEmpty(elem.Value)))
                {
                    streamWriter.WriteLine(dictionaryItem.Value);
                }
            }
        }


        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            LoadDictionary();
            Width+=3;

            Panel itemsPanel = messageList.GetItemsPanel();
            itemsPanel.VerticalAlignment = VerticalAlignment.Top;
        }

        private void LoadDictionary()
        {
            Words.Clear();
            using (var reader = new StreamReader(Dictionary, Encoding.Default))
            {
                string readLine;
                while ((readLine = reader.ReadLine()) != null)
                {
                    if (!Words.Any(elem => elem.Value.Equals(readLine, StringComparison.InvariantCulture)))
                        Words.Add(new DictionaryItem { Value = readLine });
                }
            }
        }

        private void AddElement(object sender, RoutedEventArgs e)
        {
            Words.Add(new DictionaryItem());
        }

        private void MergeDictionary(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                FileName = "Dictionary.dic",
                DefaultExt = "dic",
                Filter = "Wörterbücher|*.dic",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                using (var reader = new StreamReader(dlg.FileName, Encoding.Unicode))
                {
                    string readLine;
                    while ((readLine = reader.ReadLine()) != null)
                    {
                        var firstOrDefault = Words.FirstOrDefault(elem => elem.Value.Equals(readLine));

                        if(firstOrDefault!= null)
                        {
                            firstOrDefault.IsForDeletion = false;
                            continue;
                        }
                        Words.Add(new DictionaryItem { Value = readLine });
                    }
                } 
            }
        }

        private void Update(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((Control)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            if (e.Key == Key.Escape)
            {
                ((TextBox) sender).Text = _buffer.Value;
                ((Control)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private DictionaryItem _buffer;
        private void Buffer(object sender, RoutedEventArgs e)
        {
            _buffer = ((TextBox) sender).Tag as DictionaryItem;
        }

        private void ClearList(object sender, RoutedEventArgs e)
        {
            foreach (var dictionaryItem in Words)
            {
                dictionaryItem.IsForDeletion = true;
            }
        }

        private void ExpandAll(object sender, RoutedEventArgs e)
        {
            SetIsExpanded(true);
        }

        private void CollapseAll(object sender, RoutedEventArgs e)
        {
            SetIsExpanded(false);

        }

        private void SetIsExpanded(bool b)
        {
            var control = messageList.GetItemsPanel();

            foreach (var child in control.Children.OfType<GroupItem>())
            {
                var exp = child.GetVisualChild<Expander>();
                if (exp == null)
                    continue;
                exp.IsExpanded = b;
            }

        }

        private void UnclearList(object sender, RoutedEventArgs e)
        {
            foreach (var dictionaryItem in Words)
            {
                dictionaryItem.IsForDeletion = false;
            }
        }

    }
}
