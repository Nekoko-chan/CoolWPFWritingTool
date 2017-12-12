using System;
using System.Windows;
using System.Windows.Input;
using ComplexWriter.MessageBoxes;

namespace ComplexWriter.Commands
{
    public class IgnoreCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            //we can only close Windows
            return (parameter is UIElement);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                var window = parameter as CloseSettingsMessageBox;
                if (window == null)
                    return;
                window.EndResult = EndSettingsResult.DoNothing;
            }
        }

        #endregion


        public static readonly ICommand IgnoreOnI = new IgnoreCommand();
    }
}
