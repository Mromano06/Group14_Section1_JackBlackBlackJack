using Client.Commands;
using Client.Networking;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

// Matthew Romano & Brodie Arkell - March 12th, 2026 - MainMenuViewModel implementation
// Handles the logic of the main menu

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the main menu screen.
    /// </summary>
    public class MainMenuModel : BaseModel
    {
        /// <summary>
        /// The network client shared between all screens.
        /// </summary>
        private readonly NetworkClient _client;

        /// <summary>
        /// An action item used to navigate to the rules screen.
        /// </summary>
        private readonly Action _showRules;

        /// <summary>
        /// An action item used to navigate to the rules screen.
        /// </summary>
        private readonly Action _showBetting;

        /// <summary>
        /// The entered passcode by the user, used for authentication.
        /// </summary>
        private string _passcodeInput = string.Empty;

        /// <summary>
        /// The entered name by the user, used for authentication.
        /// </summary>
        private string _playerNameInput = string.Empty;

        /// <summary>
        /// The status of the connection attempt or errors.
        /// </summary>
        private string _statusMessage = string.Empty;

        /// <summary>
        /// Command used to navigate to the betting screen.
        /// </summary>
        public ICommand PlayCommand { get; }

        /// <summary>
        /// Command used to navigate to the rules screen.
        /// </summary>
        public ICommand RulesCommand { get; }

        /// <summary>
        /// Exits the game safely, ensuring any necessary cleanup is performed.
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuModel"/> class.
        /// </summary>
        /// <param name="client">Shared network client.</param>
        /// <param name="ShowBetting">Action used to navigate to the betting screen.</param>
        /// <param name="ShowRules">Action used to navigate to the rules screen.</param>
        /// <remarks>
        /// Subscribes to all relevant gameplay update events and initializes command bindings.
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
        /// The passcode the player types in before hitting Play.
        /// </summary>
        public string PasscodeInput
        {
            get => _passcodeInput;
            set { _passcodeInput = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// The player's display name entered on the main menu.
        /// </summary>
        public string PlayerNameInput
        {
            get => _playerNameInput;
            set { _playerNameInput = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Feedback shown to the player (e.g. "Wrong passcode" or "Connecting...").
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Attempts to connect to the server, sends the login request, and waits
        /// for the server's LoginResponse before navigating to the betting screen.
        /// </summary>
        private async void Play()
        {
            // Basic input validation
            if (string.IsNullOrWhiteSpace(PlayerNameInput)) {
                StatusMessage = "Please enter a name.";
                return;
            }

            if (!int.TryParse(PasscodeInput, out int passcode)) {
                StatusMessage = "Passcode must be a number.";
                return;
            }

            StatusMessage = "Connecting...";

            try {
                if (!_client.IsConnected) {
                    await _client.Connect("127.0.0.1", 27000); 
                }
            }
            catch (Exception) {
                StatusMessage = "Could not reach the server.";
                return;
            }

            // Set up a one-time listener for the server's response, using a
            // TaskCompletionSource so we can await it cleanly.
            var tcs = new TaskCompletionSource<(bool accepted, string message)>();

            /// <summary>
            /// Response handler for the server's login response. Unsubscribes itself.
            /// </summary>
            void OnResponse(bool accepted, string message)
            {
                _client.LoginResponseReceived -= OnResponse;
                tcs.TrySetResult((accepted, message));
            }

            _client.LoginResponseReceived += OnResponse;

            // Send the login packet
            _client.SendLoginRequest(passcode, PlayerNameInput.Trim());

            // Wait up to 5 seconds for the server to reply
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(5000));

            if (completed != tcs.Task) {
                // Timed out — clean up the listener
                _client.LoginResponseReceived -= OnResponse;
                StatusMessage = "Server did not respond. Try again.";
                return;
            }

            // Saves as a touple
            var (accepted, responseMessage) = tcs.Task.Result;

            if (!accepted) {
                StatusMessage = responseMessage; // e.g. "Wrong passcode. Access denied."
                _client.sendDisconnect();
                return;
            }

            // Login accepted — navigate to betting
            StatusMessage = string.Empty;
            Application.Current.Dispatcher.Invoke(() => _showBetting?.Invoke());
        }

        private void Rules() => _showRules?.Invoke();

        private void Exit() => Application.Current.Shutdown();
    }
}