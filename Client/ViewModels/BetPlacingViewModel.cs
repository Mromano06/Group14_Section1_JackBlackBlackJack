using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

// Matthew Romano - March 12th, 2026 - BetPlacingViewModel implementation
// Hnadles the logic of the bet placing view model

namespace Client.ViewModels
{
    public class BetPlacingViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly Action<int> _showGame;
        private readonly int _playerMoney;
        private int _currentBet;

        public ICommand IncreaseBetCommand { get; }
        public ICommand DecreaseBetCommand { get; }
        public ICommand MaxBetCommand { get; }
        public ICommand ConfirmBetCommand { get; }

        public BetPlacingViewModel(NetworkClient client, int playerMoney, Action<int> showGame)
        {
            _client = client;
            _showGame = showGame;
            _playerMoney = playerMoney;
            IncreaseBetCommand = new CommandRelay(IncBet);
            DecreaseBetCommand = new CommandRelay(DecBet);
            MaxBetCommand = new CommandRelay(MaxBet);
            ConfirmBetCommand = new CommandRelay(Confirm);
        }

        public int CurrentBet
        {
            get => _currentBet;
            set
            {
                if (value <= _playerMoney && value >= 0)
                    _currentBet = value;
                OnPropertyChanged();
            }
        }
        

        // No Setter because it's readonly
        public int PlayerMoney
        {
            get => _playerMoney;
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
            _showGame(CurrentBet);
        }
    }
}
