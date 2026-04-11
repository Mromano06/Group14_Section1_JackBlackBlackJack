using Client.Commands;
using Client.Networking;
using GameLogic.Actions.ActionTypes;
using Jables_Protocol.DTOs;
using SharedModels.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
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
        /// Tracks whether the full game session has ended.
        /// </summary>
        private bool _gameHasEnded;

        /// <summary>
        /// Action used to navigate to the betting screen.
        /// </summary>
        private readonly Action _showBetting;

        /// <summary>
        /// Action used to navigate to the main menu.
        /// </summary>
        private readonly Action _showMainMenu;

        /// <summary>
        /// Action used to navigate to the betting screen.
        /// </summary>
        private readonly Action _showVictoryScreen;

        /// <summary>
        /// Action used to navigate to the main menu.
        /// </summary>
        private readonly Action _showLossScreen;

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
        public ResultScreenViewModel(NetworkClient client, Action ShowBetting, Action ShowMainMenu,
            String resultMessage, Action showLossScreen, Action showVictoryScreen)
        {
            _client = client;
            _showBetting = ShowBetting;
            _showMainMenu = ShowMainMenu;
            _resultMessage = resultMessage;
            _showVictoryScreen = showVictoryScreen;
            _showLossScreen = showLossScreen;
            _client.GameResultUpdate += FinishGame;
            _gameHasEnded = false;
            ContinueCommand = new CommandRelay(Continue);
            MainMenuCommand = new CommandRelay(MainMenu);

            if (_client.LastGameResult != null)
            {
                FinishGame(_client.LastGameResult.Value);
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether the game has ended.
        /// </summary>
        public bool GameHasEnded
        {
            get => _gameHasEnded;
            set
            {
                _gameHasEnded = value;
                OnPropertyChanged(nameof(GameHasEnded));
            }
        }


        /// <summary>
        /// Handles full game completion and navigates to the appropriate end screen.
        /// </summary>
        /// <param name="result">The final game result reported by the server.
        /// </param>
        private void FinishGame(GameResult result)
        {
            GameHasEnded = true;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (result == GameResult.PLAYER_LOSE)
                {
                    _showLossScreen?.Invoke();
                }
                else if (result == GameResult.PLAYER_WIN)
                {
                    _showVictoryScreen?.Invoke();
                }
            }));
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
            Cleanup();
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
            Cleanup();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showMainMenu?.Invoke();
            }));
        }
        public void Cleanup()
        {
            _client.GameResultUpdate -= FinishGame;
        }
    }
}
