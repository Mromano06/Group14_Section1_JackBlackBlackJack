using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

// Matthew Romano & Brodie Arkell - March 12th, 2026 - MainMenuViewModel implementation
// Hnadles the logic of the main menu

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the main menu screen.
    /// </summary>
    /// <remarks>
    /// Handles user interaction for the main menu, including starting the game,
    /// viewing rules, and exiting the application.
    /// 
    /// Uses command bindings to connect buttons to logic.
    /// </remarks>
    public class MainMenuModel : BaseModel
    {
        /// <summary>
        /// Network client used to establish connection with the game server.
        /// </summary>
        private readonly NetworkClient _client;

        /// <summary>
        /// An action item saved temporatily, used to navigate to the rules screen.
        /// </summary>
        private readonly Action _showRules;

        /// <summary>
        /// An action item saved temporatily, used to navigate to the betting screen.
        /// </summary>
        private readonly Action _showBetting;

        /// <summary>
        /// Action used to start the game.
        /// </summary>
        public ICommand PlayCommand { get; }

        /// <summary>
        /// Action used to view the game rules.
        /// </summary>
        public ICommand RulesCommand { get; }

        /// <summary>
        /// Action used to exit the game.
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuModel"/> class.
        /// </summary>
        /// <param name="client">The shared network client.</param>
        /// <param name="ShowBetting">Action to navigate to the betting screen.</param>
        /// <param name="ShowRules">Action to navigate to the rules screen.</param>
        /// <remarks>
        /// Binds UI commands to their respective handler methods.
        /// </remarks>
        public MainMenuModel(NetworkClient client, Action ShowBetting, Action ShowRules)
        {
            _client = client;
            _showBetting = ShowBetting;
            _showRules = ShowRules;

            PlayCommand = new CommandRelay(Play);
            ExitCommand = new CommandRelay(Exit);
            RulesCommand = new CommandRelay(Rules);
        }

        /// <summary>
        /// Handles the Play command.
        /// </summary>
        /// <remarks>
        /// Attempts to connect to the server, then navigates to the betting screen.
        /// </remarks>
        private async void Play()
        {
            // attempt connection to server
            if (!_client.IsConnected)
            {
                await _client.Connect("127.0.0.1", 27000);
            }

            _showBetting?.Invoke();
        }

        /// <summary>
        /// Handles the Rules command.
        /// </summary>
        /// <remarks>
        /// Navigates to the rules screen.
        /// </remarks>
        private void Rules()
        {
            _showRules?.Invoke();
        }

        /// <summary>
        /// Handles the Exit command.
        /// </summary>
        /// <remarks>
        /// Gracefully shuts down the application.
        /// </remarks>
        private void Exit()
        {
            // clean exiting
            Application.Current.Shutdown();
        }
    }
}
