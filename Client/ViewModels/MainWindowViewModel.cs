using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

// Matthew Romano & Brodie Arkell - March 12th, 2026 - MainWindowViewModel implementation
// Handles the logic of the main view model

namespace Client.ViewModels
{
    public class MainWindowViewModel : BaseModel
    {
        private BaseModel _currentViewModel;
        // Client shared across all screens
        private readonly NetworkClient _client;

        private double _playerMoney;
        private double _latestBet;

        public BaseModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel(NetworkClient client, double playerMoney)
        {
            _client = client;
            _playerMoney = playerMoney;
            _client = new NetworkClient();
            _currentViewModel = new MainMenuModel(_client, ShowMenu, ShowRules);
        }

        public double PlayerMoney
        {
            get => _playerMoney;
            set
            {
                if (value >= 0)
                    _playerMoney = value;
            }
        }

        public double LatestBet
        {
            get => _latestBet;
            set
            {
                if (value <= this.PlayerMoney)
                    _latestBet = value;
            }
        }

        // TODO: Link Dispatcher to each of these functions
        public void ShowGame(double betAmount)
        {
            /// TODO: create command objects so we can enqueue them here
            // var playCommand = new PlayCommand();
            //_client.EnqueueCommand(playCommand);
            PlayerMoney -= betAmount;
            CurrentViewModel = new GameplayViewModel(_client, betAmount, _playerMoney);
        }

        public void ShowBetting()
        {
            CurrentViewModel = new BetPlacingViewModel(_client, _playerMoney, ShowGame);
        }

        public void ShowMenu()
        {
            CurrentViewModel = new MainMenuModel(_client, ShowBetting, ShowRules);
        }

        public void ShowRules()
        {
            CurrentViewModel = new RulesViewModel(ShowMenu);
        }

    }
}
