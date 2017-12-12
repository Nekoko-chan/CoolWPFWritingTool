using System;
using System.Windows;
using System.Windows.Input;
using ComplexWriter.MessageBoxes;

namespace ComplexWriter.Commands
{
    public class OkCommand : ICommand
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
                var window = parameter as MessageResultWindow;
                if (window == null)
                    return;
                window.Result = MessageBoxResult.OK;
            }
        }

        #endregion

        public static readonly ICommand OkOnEnterOrO = new OkCommand();
    }
}
