using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class BetPlacingViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly int _playerMoney;
        private int _currentBet;

        public ICommand IncreaseBetCommand { get; }
        public ICommand DecreaseBetCommand { get; }
        public ICommand MaxBetCommand { get; }
        public ICommand ConfirmBetCommand { get; }

        public BetPlacingViewModel(NetworkClient client, int playerMoney)
        {
            // TODO: Remove the default 100 money for testing
            _client = client;
            // this._playerMoney = playerMoney;
            _playerMoney = 100;
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
            // Send the bet amount to the server
            // Create new gameplay window
        }
    }
}
