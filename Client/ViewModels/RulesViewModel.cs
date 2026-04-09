using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

// Matthew Romano - March 12th, 2026 - RulesgViewModel implementation
// Handles the logic of the rules view model

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the rules screen.
    /// </summary>
    /// <remarks>
    /// Provides navigation logic for returning to the main menu,
    /// other than that this just serves to display the rules of the game.
    /// </remarks>
    public class RulesViewModel : BaseModel
    {
        /// <summary>
        /// Action used to navigate back to the main menu.
        /// </summary>
        private readonly Action _showMainMenu;

        /// <summary>
        /// Command to return to the main menu.
        /// </summary>
        public ICommand BackCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesViewModel"/> class.
        /// </summary>
        /// <param name="showMainMenu">Action used to navigate back to the main menu.</param>
        /// <remarks>
        /// Binds the Back command to its handler.
        /// </remarks>
        public RulesViewModel(Action showMainMenu) {
            _showMainMenu = showMainMenu;
            BackCommand = new CommandRelay(Back);
        }

        /// <summary>
        /// Handles the Back command.
        /// </summary>
        /// <remarks>
        /// Invokes navigation to return to the main menu screen.
        /// </remarks>
        void Back()
        {
            _showMainMenu?.Invoke();
        }
    }
}
