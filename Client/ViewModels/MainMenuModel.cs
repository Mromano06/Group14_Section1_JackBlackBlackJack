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
    public class MainMenuModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly Action _showRules;
        private readonly Action _showBetting;

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

        private async void Play()
        {
            // attempt connection to server
            if (!_client.IsConnected)
            {
                await _client.Connect("127.0.0.1", 27000);
            }

            _showBetting?.Invoke();
        }

        private void Rules()
        {
            _showRules?.Invoke();
        }

        // Exit's the application from the main menu
        private void Exit()
        {
            // clean exiting
            Application.Current.Shutdown();
        }
    }
}
