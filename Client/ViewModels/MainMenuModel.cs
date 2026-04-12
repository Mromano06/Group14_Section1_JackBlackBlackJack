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
        private readonly NetworkClient _client;
        private readonly Action _showRules;
        private readonly Action _showBetting;

        private string _passcodeInput = string.Empty;
        private string _playerNameInput = string.Empty;
        private string _statusMessage = string.Empty;

        public ICommand PlayCommand { get; }
        public ICommand RulesCommand { get; }
        public ICommand ExitCommand { get; }

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