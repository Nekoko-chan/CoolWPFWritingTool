using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComplexWriter.MessageBoxes;

namespace ComplexWriter.Commands
{
    public class OwnPasteCommand : RoutedUICommand
    {
        #region ICommand Members

        public OwnPasteCommand()
        {
            this.Text = "Einfügen (OHNE Formatierung)";
            this.InputGestures.Add(new KeyGesture(Key.V, ModifierKeys.Control | ModifierKeys.Shift));
        }


        #endregion

        public static readonly ICommand Paste = new OwnPasteCommand();
    }
}
