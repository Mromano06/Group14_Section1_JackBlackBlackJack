using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Server.Commands
{
    public class ServerCommandRelay : ICommand
    {

        private readonly Action _execute;

        public ServerCommandRelay(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute();
        }

    }
}
