using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Windows.Input;

namespace Client.Commands
{
    public class CommandRelay : ICommand
    {

        private readonly Action _execute;

        public CommandRelay(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) {
            _execute();
        }

    }
}
