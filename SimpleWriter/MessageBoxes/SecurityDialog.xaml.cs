using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ExtensionObjects;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class SecurityDialog
    {
        public SecurityDialog()
        {
            InitializeComponent();
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


        public static readonly DependencyProperty AskPasswordsProperty =
         DependencyProperty.Register("AskPasswords", typeof(bool), typeof(SecurityDialog),
             new PropertyMetadata(true));

        public bool AskPasswords
        {
            get { return (bool)GetValue(AskPasswordsProperty); }
            set { SetValue(AskPasswordsProperty, value); }
        }

        public static readonly DependencyProperty AskPasswordsOnTabChangeProperty =
        DependencyProperty.Register("AskPasswordsOnTabChange", typeof(bool), typeof(SecurityDialog),
            new PropertyMetadata(true));

        public bool AskPasswordsOnTabChange
        {
            get { return (bool)GetValue(AskPasswordsOnTabChangeProperty); }
            set { SetValue(AskPasswordsOnTabChangeProperty, value); }
        }
        public static readonly DependencyProperty UseGermanProperty =
       DependencyProperty.Register("UseGerman", typeof(bool), typeof(SecurityDialog),
           new PropertyMetadata(true));

        public bool UseGerman
        {
            get { return (bool)GetValue(UseGermanProperty); }
            set { SetValue(UseGermanProperty, value); }
        }

        public static readonly DependencyProperty UseEnglishProperty =
       DependencyProperty.Register("UseEnglish", typeof(bool), typeof(SecurityDialog),
           new PropertyMetadata(true));

        public bool UseEnglish
        {
            get { return (bool)GetValue(UseEnglishProperty); }
            set { SetValue(UseEnglishProperty, value); }
        }

        public static readonly DependencyProperty AllowEmptyPasswordQuestionsProperty =
        DependencyProperty.Register("AllowEmptyPasswordQuestions", typeof(bool), typeof(SecurityDialog),
            new PropertyMetadata(true));

        public bool AllowEmptyPasswordQuestions
        {
            get { return (bool)GetValue(AllowEmptyPasswordQuestionsProperty); }
            set { SetValue(AllowEmptyPasswordQuestionsProperty, value); }
        }

        public static readonly DependencyProperty SaveAutomaticalProperty =
        DependencyProperty.Register("SaveAutomatical", typeof(bool), typeof(SecurityDialog),
            new PropertyMetadata(true));

        public bool SaveAutomatical
        {
            get { return (bool)GetValue(SaveAutomaticalProperty); }
            set { SetValue(SaveAutomaticalProperty, value); }
        }

        public static readonly DependencyProperty HideQuestionProperty =
       DependencyProperty.Register("HideQuestion", typeof(bool), typeof(SecurityDialog),
           new PropertyMetadata(true));

        public bool HideQuestion
        {
            get { return (bool)GetValue(HideQuestionProperty); }
            set { SetValue(HideQuestionProperty, value); }
        }

    }
}
