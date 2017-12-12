using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Forms.VisualStyles;

namespace ComplexWriter.CharacterNames
{
    /// <summary>
    /// Interaktionslogik für CharacterNameControl.xaml
    /// </summary>
    public partial class CharacterNameControl : UserControl
    {
        public CharacterNameControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NamesProperty =
           DependencyProperty.Register("Names", typeof(ObservableCollection<Character>), typeof(CharacterNameControl), new PropertyMetadata(new ObservableCollection<Character>()));


        public ObservableCollection<Character> Names
        {
            get { return (ObservableCollection<Character>)GetValue(NamesProperty); }
            set { SetValue(NamesProperty, value); }
        }

        public static readonly DependencyProperty IsEditableProperty =
  DependencyProperty.Register("IsEditable", typeof(bool), typeof(CharacterNameControl), new PropertyMetadata(false));


        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }


        public static readonly RoutedEvent TextClickedEvent = EventManager.RegisterRoutedEvent(
         "TextClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CharacterNameControl));

        public event RoutedEventHandler TextClicked
        {
            add { AddHandler(TextClickedEvent, value); }
            remove { RemoveHandler(TextClickedEvent, value); }
        }

        private void RaiseTextClickedEvent(string text)
        {
            var newEventArgs = new NameClickedRoutedEventArgs(TextClickedEvent) { ClickedText = text };
            RaiseEvent(newEventArgs);
        }

        private void NameSelected(object sender, RoutedEventArgs e)
        {
            var value = ((TextButton) sender).Caption;
            RaiseTextClickedEvent(value);
        }

        private void AddNew(object sender, RoutedEventArgs e)
        {
            var character = new Character() { Name = "" };
            if(SetDescription(character, Names))
                Names.Add(character);
        }

        private void DeleteMe(object sender, RoutedEventArgs e)
        {
            var elem = ((Button)sender).Tag as Character;
            if (elem == null) return;

            Names.Remove(elem);
        }

        private void EditDescription(object sender, RoutedEventArgs e)
        {
            var elem = ((FrameworkElement)sender).Tag as Character;
            if (elem == null) return;
            SetDescription(elem, Names);
        }

        internal static bool SetDescription(Character elem,IEnumerable<Character> charas )
        {
            var multi = new MessageBoxes.CharacterEditor() { Owner = MainWindow.Global, Character = elem.Clone(), CharacterNames = charas};
            var b = multi.ShowDialog() == true&& multi.Result == MessageBoxResult.OK;
            if (b)
            {
                elem.Description = multi.Character.Description;
                elem.Name = multi.Character.Name;
                elem.Type = multi.Character.Type;
                elem.IsMain = multi.Character.IsMain;
                elem.ImageBuffer = multi.Character.ImageBuffer;
            }

            return b;
        }

        private void AddSelected(object sender, RoutedEventArgs e)
        {
           MainWindow.Global.AddSelectedNameToDocument();
        }

        private void DeleteChara(object sender, RoutedEventArgs e)
        {
            var chara = ((FrameworkElement) sender).Tag as Character;
            if (chara == null) return;

            if(MessageBoxes.QuestionBox.ShowMessage(MainWindow.Global, "Soll der Character wirklich gelöscht werden?", "Löschen", false) == MessageBoxResult.Yes)
                Names.Remove(chara);
        }
    }
}
