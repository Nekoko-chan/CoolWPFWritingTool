﻿using System;
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
    public partial class TextInput
    {
        public TextInput()
        {
            InitializeComponent();
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


        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(TextInput),
                new PropertyMetadata(""));

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty UseAsSpeekProperty =
          DependencyProperty.Register("UseAsSpeek", typeof(bool), typeof(TextInput),
              new PropertyMetadata(false, OnUseAsSpeekChanged));

        private static void OnUseAsSpeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue)
            {
                var contentBox = ((TextInput) d).ContentBox;
                contentBox.SelectAll();
                contentBox.Focus();
            }
        }

        public bool UseAsSpeek
        {
            get { return (bool)GetValue(UseAsSpeekProperty); }
            set { SetValue(UseAsSpeekProperty, value); }
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
    }
}
