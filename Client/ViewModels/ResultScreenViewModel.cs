using Client.Commands;
using Client.Networking;
using GameLogic.Actions.ActionTypes;
using Jables_Protocol.DTOs;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

// Matthew Romano - April , 2026 - BetPlacingViewModel implementation
// Handles the logic of the result screen view model

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the result screen displayed after a round ends.
    /// </summary>
    /// <remarks>
    /// Displays the outcome of the game as a message and provides
    /// navigation options to either continue playing or return to the main menu.
    /// </remarks>
    public class ResultScreenViewModel : BaseModel
    {
        /// <summary>
        /// Action used to navigate to the betting screen.
        /// </summary>
        private readonly Action _showBetting;

        /// <summary>
        /// Action used to navigate to the main menu.
        /// </summary>
        private readonly Action _showMainMenu;

        /// <summary>
        /// Shared network client used by the application.
        /// 
        /// Unused here.
        /// </summary>
        private readonly NetworkClient _client;

        /// <summary>
        /// Stores the result message displayed to the user.
        /// </summary>
        private String _resultMessage;

        /// <summary>
        /// Command to continue to the next round (betting screen).
        /// </summary>
        public ICommand ContinueCommand { get; }

        /// <summary>
        /// Command to return to the main menu.
        /// </summary>
        public ICommand MainMenuCommand { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ResultScreenViewModel"/> class.
        /// </summary>
        /// <param name="client">Shared network client.</param>
        /// <param name="ShowBetting">Action to navigate to the betting screen.</param>
        /// <param name="ShowMainMenu">Action to navigate to the main menu.</param>
        /// <param name="resultMessage">Message describing the game outcome.</param>
        /// <remarks>
        /// Binds commands and initializes the result message displayed on screen.
        /// </remarks>
        public ResultScreenViewModel(NetworkClient client, Action ShowBetting, Action ShowMainMenu, String resultMessage)
        {
            _client = client;
            _showBetting = ShowBetting;
            _showMainMenu = ShowMainMenu;
            _resultMessage = resultMessage;
            ContinueCommand = new CommandRelay(Continue);
            MainMenuCommand = new CommandRelay(MainMenu);
        }

        /// <summary>
        /// Gets or sets the result message displayed on the screen.
        /// </summary>
        /// <remarks>
        /// Updating this property refreshes the UI through data binding.
        /// </remarks>
        public String ResultMessage
        {
            set
            {
                _resultMessage = value;
                OnPropertyChanged();
            }
            get => _resultMessage;
        }

        /// <summary>
        /// Handles the Continue command.
        /// </summary>
        /// <remarks>
        /// Navigates to the betting screen to allow the player to start a new round.
        /// </remarks>
        public void Continue()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showBetting?.Invoke();
            }));
        }

        /// <summary>
        /// Handles the Main Menu command.
        /// </summary>
        /// <remarks>
        /// Navigates to the betting screen to allow the player to start a new round.
        /// </remarks>
        public void MainMenu()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showMainMenu?.Invoke();
            }));
        }

    }
}
