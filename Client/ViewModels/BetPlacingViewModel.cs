using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

// Matthew Romano - March 12th, 2026 - BetPlacingViewModel implementation
// Hnadles the logic of the bet placing view model

namespace Client.ViewModels
{
    public class BetPlacingViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly Action<double> _showGame;
        private double _playerMoney;
        private double _currentBet;

        public ICommand IncreaseBetCommand { get; }
        public ICommand DecreaseBetCommand { get; }
        public ICommand MaxBetCommand { get; }
        public ICommand ConfirmBetCommand { get; }

        public BetPlacingViewModel(NetworkClient client, Action<double> showGame)
        {
            _client = client;
            _showGame = showGame;
            _client.PlayerMoneyUpdate += UpdatePlayerMoney;
            IncreaseBetCommand = new CommandRelay(IncBet);
            DecreaseBetCommand = new CommandRelay(DecBet);
            MaxBetCommand = new CommandRelay(MaxBet);
            ConfirmBetCommand = new CommandRelay(Confirm);
        }

        public double CurrentBet
        {
            get => _currentBet;
            set
            {
                if (value <= _playerMoney && value >= 0)
                {
                    _currentBet = value;
                    OnPropertyChanged();
                }
            }
        }

        // No Setter because it's readonly
        public double PlayerMoney
        {
            get => _playerMoney;
            set
            {
                _playerMoney = value;
                OnPropertyChanged();
            }
        }

        private void IncBet()
        {
            if (_currentBet + 10 <= _playerMoney)
                CurrentBet += 10;
        }

        private void DecBet()
        {
            if (_currentBet - 10 >= 0)
                CurrentBet -= 10;
        }

        private void MaxBet()
        {
            CurrentBet = _playerMoney;
        }

        private void Confirm()
        {
            // TODO: Send the bet amount to the server
            _showGame?.Invoke(CurrentBet);

        }

        private void UpdatePlayerMoney(double amount)
        {
            Debug.WriteLine("updating the player's money");

            if (App.Current.Dispatcher.CheckAccess())
                PlayerMoney = amount;
            else
                App.Current.Dispatcher.Invoke(() => PlayerMoney = amount);
        }

        public void Cleanup()
        {
            _client.PlayerMoneyUpdate -= UpdatePlayerMoney;
        }
    }
}
