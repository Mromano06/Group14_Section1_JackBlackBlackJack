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
                if (_currentViewModel is GameplayViewModel previousScreen) // before changing screens check if was on gamescreen
                {
                    previousScreen.Cleanup(); // unsubscribes to events when switching screens
                }

                /// TODO: double check if this is needed when testing the game
                if (_currentViewModel is BetPlacingViewModel previousBetPlacing)
                {
                    previousBetPlacing.Cleanup(); // unsubscribing to events when switching screens
                }

                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            _client = new NetworkClient();
            _currentViewModel = new MainMenuModel(_client, ShowBetting, ShowRules);
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
        public void ShowGame(double betAmount, double playerMoney)
        {
            playerMoney -= betAmount; // temp saving to display on the main gameplay window
            CurrentViewModel = new GameplayViewModel(_client, betAmount, playerMoney, ShowBetting);
        }

        public void ShowBetting()
        {
            CurrentViewModel = new BetPlacingViewModel(_client, ShowGame);
        }

        public void ShowMenu()
        {
            CurrentViewModel = new MainMenuModel(_client, ShowBetting, ShowRules);
        }

        public void ShowRules()
        {
            CurrentViewModel = new RulesViewModel(ShowMenu);
        }

        public void betUpdate(double betAmount)
        {
            LatestBet = betAmount;
        }

        public void moneyUpdate(double money)
        {
            PlayerMoney = money;
        }
    }
}
