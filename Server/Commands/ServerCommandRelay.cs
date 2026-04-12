using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Server.Commands
{
    /// <summary>
    /// A simple implementation of <see cref="ICommand"/> for server-side command handling.
    /// </summary>
    /// <remarks>
    /// This class wraps an <see cref="Action"/> item and executes it when the command is triggered.
    /// It always returns true for <see cref="CanExecute"/>, meaning the command is always enabled.
    /// </remarks>
    public class ServerCommandRelay : ICommand
    {
        /// <summary>
        /// The action to execute when the command is invoked.
        /// </summary>
        private readonly Action _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCommandRelay"/> class.
        /// </summary>
        /// <param name="execute">The action to execute when the command is triggered.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="execute"/> is null.</exception>
        public ServerCommandRelay(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        }

        /// <summary>
        /// Occurs when changes happen that affect whether the command should execute.
        /// </summary>
        /// <remarks>
        /// This implementation does not raise this event.
        /// </remarks>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Determines whether the command can execute.
        /// </summary>
        /// <param name="parameter">Optional parameter (unused).</param>
        /// <returns>Always returns true.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Optional parameter (unused).</param>
        public void Execute(object? parameter)
        {
            _execute();
        }

    }
}
