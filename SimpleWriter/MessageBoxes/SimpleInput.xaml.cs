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
using Microsoft.Win32;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class SimpleInput
    {
        public SimpleInput()
        {
            InitializeComponent();
        }

        private string _text;
        public SimpleInput(string text)
        {
            _text = text;
            InitializeComponent();
            ContentBox.Text = Path.GetFileNameWithoutExtension(_text);
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        public string Text { get { return ContentBox.Text; } }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = (MessageBoxResult)((Button) sender).Tag;
            Result = messageBoxResult;
        }

        public static readonly DependencyProperty IsUseableProperty =
          DependencyProperty.Register("IsUseable", typeof(bool), typeof(SimpleInput),
              new PropertyMetadata(true));

        public bool IsUseable
        {
            get { return (bool)GetValue(IsUseableProperty); }
            set { SetValue(IsUseableProperty, value); }
        }
        

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            ContentBox.Focus();
        }

        private void ContentBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Result = MessageBoxResult.OK;
                e.Handled = true;
            }
        }

        private void UpdateText(object sender, TextChangedEventArgs e)
        {
            IsUseable = !string.IsNullOrEmpty(_text) && (ContentBox.Text.Equals(Path.GetFileNameWithoutExtension(_text)) || !File.Exists(BuildFileName(ContentBox.Text)));
        }

        public string BuildFileName(string text)
        {
            var path = System.IO.Path.GetDirectoryName(_text);
            var ext = System.IO.Path.GetExtension(_text);
            return System.IO.Path.Combine(path, text +  ext);
        }
    }
}
