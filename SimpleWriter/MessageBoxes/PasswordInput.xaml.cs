using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ComplexWriter.Properties;
using ExtensionObjects;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class PasswordInput
    {
        public PasswordInput()
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

            if (CheckAndReturn(messageBoxResult)) return;
            Result = messageBoxResult;
        }

        private bool CheckAndReturn(MessageBoxResult messageBoxResult)
        {
            if (!CheckPassword && messageBoxResult == MessageBoxResult.Cancel) return false;
            if (CheckPassword && messageBoxResult == MessageBoxResult.OK && !GetPlainPassword().Equals(Password))
            {
                IsCorrectPassword = false;
                return true;
            }

            if (!CheckPassword && messageBoxResult == MessageBoxResult.OK && string.IsNullOrEmpty(Password)) return false;

            if (!CheckPassword && (!Settings.Default.AllowEmptyQuestions && Settings.Default.AskForPassword)&& string.IsNullOrEmpty(PasswordQuestion))
            {
                IsValidQuestion = false;
                return true;
            }

            if (!CheckPassword && !ShowPlainText && Password != GetPlainPassword())
            {
                return QuestionBox.ShowMessage(this, Properties.Resources.PasswordIsHidden, Properties.Resources.ApplyChanges, false)== MessageBoxResult.No;
            }


            return false;
        }


        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register("Filename", typeof(string), typeof(PasswordInput),
                new PropertyMetadata("",FilenameChanged));

        private static void FilenameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = d as PasswordInput;
            if (win == null) return;

            win.Message = win.CheckPassword ? 
                string.Format(Properties.Resources.FileIsProtected,win.Filename) 
                :
                String.Format(Properties.Resources.ChoosePassword,win.Filename);
        }

        public string Filename
        {
            get { return (string)GetValue(FilenameProperty); }
            set { SetValue(FilenameProperty, value); }
        }

        public static readonly DependencyProperty PasswordDecrpytedProperty =
            DependencyProperty.Register("PasswordDecrpytedDecrpyted", typeof(string), typeof(PasswordInput),
                new PropertyMetadata("",DecryptedPasswordChanged));

        private static void DecryptedPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = d as PasswordInput;
            if (win == null) return;

            if(!win.CheckPassword)
                win.Password =win.ContentPwdBox.Password= win.GetPlainPassword();
        }

        public string PasswordDecrpyted
        {
            get { return (string)GetValue(PasswordDecrpytedProperty); }
            set { SetValue(PasswordDecrpytedProperty, value); }
        }


        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordInput),
                new PropertyMetadata(""));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordQuestionProperty =
           DependencyProperty.Register("PasswordQuestion", typeof(string), typeof(PasswordInput),
               new PropertyMetadata("",UpdateQuestion));

        private static void UpdateQuestion(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = d as PasswordInput;
            if (win == null) return;

            win.HintBox.Visibility = win.CheckPassword && string.IsNullOrEmpty(win.PasswordQuestion)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public string PasswordQuestion
        {
            get { return (string)GetValue(PasswordQuestionProperty); }
            set { SetValue(PasswordQuestionProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
           DependencyProperty.Register("Message", typeof(string), typeof(PasswordInput),
               new PropertyMetadata(""));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty IsCorrectPasswordProperty =
          DependencyProperty.Register("IsCorrectPassword", typeof(bool), typeof(PasswordInput),
              new PropertyMetadata(true));

        public bool IsCorrectPassword
        {
            get { return (bool)GetValue(IsCorrectPasswordProperty); }
            set { SetValue(IsCorrectPasswordProperty, value); }
        }

        public static readonly DependencyProperty IsValidQuestionProperty =
         DependencyProperty.Register("IsValidQuestion", typeof(bool), typeof(PasswordInput),
             new PropertyMetadata(true));

        public bool IsValidQuestion
        {
            get { return (bool)GetValue(IsValidQuestionProperty); }
            set { SetValue(IsValidQuestionProperty, value); }
        }

        public static readonly DependencyProperty CheckPasswordProperty =
          DependencyProperty.Register("CheckPassword", typeof(bool), typeof(PasswordInput),
              new PropertyMetadata(false,UpdateCheck));

        private static void UpdateCheck(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = d as PasswordInput;
            if (win == null) return;

            win.HintBox.Visibility = win.CheckPassword && string.IsNullOrEmpty(win.PasswordQuestion)
                ? Visibility.Collapsed
                : Visibility.Visible;

            if(Settings.Default.HideQuestion &&win.CheckPassword)
                win.hintDock.Visibility =Settings.Default.HideQuestion &&win.CheckPassword ? Visibility.Collapsed:Visibility.Visible;
        }

        public bool CheckPassword
        {
            get { return (bool)GetValue(CheckPasswordProperty); }
            set { SetValue(CheckPasswordProperty, value); }
        }

        public static readonly DependencyProperty ShowPlainTextProperty =
         DependencyProperty.Register("ShowPlainText", typeof(bool), typeof(PasswordInput),
             new PropertyMetadata(false, UpdateShowPlainText));

        private static void UpdateShowPlainText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = d as PasswordInput;
            if (win == null) return;

            if (win.ShowPlainText)
            {
                win.Password = win.ContentPwdBox.Password;
                win.ContentBox.Focus();
            }
            else
            {
                win.ContentPwdBox.Password = win.Password;
                win.ContentPwdBox.Focus();
            }
        }

        public bool ShowPlainText
        {
            get { return (bool)GetValue(ShowPlainTextProperty); }
            set { SetValue(ShowPlainTextProperty, value); }
        }
        

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            if (ShowPlainText)
                ContentBox.Focus();
            else
                ContentPwdBox.Focus();
        }

        private void ContentBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (CheckAndReturn(MessageBoxResult.OK)) return;
                Result = MessageBoxResult.OK;
                e.Handled = true;
            }
        }

        public string GetPlainPassword()
        {
            return string.IsNullOrEmpty(PasswordDecrpyted) ? string.Empty: PasswordDecrpyted.Decrypt("MainWindow");
        }

        public string GetEncryptedPassword()
        {
            return string.IsNullOrEmpty(Password) ? string.Empty : Password.Encrypt("MainWindow");
        }

        private void UpdatePassword(object sender, RoutedEventArgs e)
        {
            var pwd = sender as PasswordBox;
            if (pwd != null)
                Password = pwd.Password;
            else
            {
                var text = sender as TextBox;
                if (text == null) return;

                Password = text.Text;
            }
        }
    }
}
