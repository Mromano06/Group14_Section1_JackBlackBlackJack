using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

// Matthew Romano - March 12th, 2026 - MainWindowViewModel implementation
// Hnadles the logic of the main view model

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

        public MainWindowViewModel()
        {
            _client = new NetworkClient();
            _currentViewModel = new MainMenuModel(_client, ShowMenu, ShowRules);
;        }

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

        public void ShowGame(double betAmount)
        {
            // TODO: Decide if this makes sense
            PlayerMoney -= betAmount;
            CurrentViewModel = new GameplayViewModel(_client, betAmount, _playerMoney);
        }

        public void ShowBetting()
        {
            PlayerMoney = 100; // TODO: Have server get the info and send it here
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
