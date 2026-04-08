using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Windows.Input;

namespace Client.Commands
{
    /// <summary>
    /// A simple implementation of <see cref="ICommand"/> used to bind UI actions
    /// to ViewModel methods in our MVVM architecture.
    /// </summary>
    /// <remarks>
    /// This class wraps an <see cref="Action"/> and exposes it as a command
    /// that can be bound to UI elements like buttons.
    /// 
    /// This implementation always allows execution (i.e., <see cref="CanExecute(object?)"/> returns true).
    /// </remarks>
    public class CommandRelay : ICommand
    {
        /// <summary>
        /// The action to execute when the command is triggered.
        /// </summary>
        private readonly Action _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRelay"/> class.
        /// </summary>
        /// <param name="execute">The action to execute when the command is invoked.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the provided execute action is null.
        /// </exception>
        /// <remarks>
        /// This allows ViewModels to pass method references directly into command bindings.
        /// </remarks>
        public CommandRelay(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        }

        /// <summary>
        /// Occurs when changes affect whether the command should execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Determines whether the command can execute.
        /// </summary>
        /// <param name="parameter">Optional parameter passed from the UI.</param>
        /// <returns>Always returns true.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>
        /// Executes the bound action.
        /// </summary>
        /// <param name="parameter">Optional parameter passed from the UI.</param>
        /// <remarks>
        /// Invokes the stored action when the command is triggered.
        /// </remarks>
        public void Execute(object? parameter) {
            _execute();
        }

    }
}
