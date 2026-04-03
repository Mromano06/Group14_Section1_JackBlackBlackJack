using Client.Commands;
using Client.Networking;
using Jables_Protocol;
using Jables_Protocol.DTOs;
using Jables_Protocol.Serializers;
using SharedModels.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;

// Matthew Romano - March 12th, 2026 - BetPlacingViewModel implementation
// Hnadles the logic of the bet placing view model

namespace Client.ViewModels
{
    public class BetPlacingViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly Action _showGame;
        private double _playerMoney;
        private double _currentBet;
        private readonly PlayerCommandSerializer _commandSerializer = new PlayerCommandSerializer();

        public ICommand IncreaseBetCommand { get; }
        public ICommand DecreaseBetCommand { get; }
        public ICommand MaxBetCommand { get; }
        public ICommand ConfirmBetCommand { get; }
        public ICommand ResetBetCommand { get; }

        public BetPlacingViewModel(NetworkClient client, Action showGame)
        {
            _client = client;
            _showGame = showGame;
            _playerMoney = _client.LatestPlayerMoney; // set backing field directly
            _client.PlayerMoneyUpdate += UpdatePlayerMoney;
            _client.PlayerBetUpdate += UpdateBetAmount;
            IncreaseBetCommand = new CommandRelay(IncBet);
            DecreaseBetCommand = new CommandRelay(DecBet);
            MaxBetCommand = new CommandRelay(MaxBet);
            ConfirmBetCommand = new CommandRelay(Confirm);
            ResetBetCommand = new CommandRelay(Reset);
        }

        public double CurrentBet
        {
            get => _currentBet;
            set
            {
                if (value <= _playerMoney && value >= 0 && _currentBet != value)
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
                OnPropertyChanged(nameof(PlayerMoney));
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

        private void Reset()
        {
            CurrentBet = 0;
        }

        private void Confirm()
        {
            PlayerCommandDto cmd = new PlayerCommandDto();
            cmd.Action = PlayerAction.Bet;
            cmd.BetAmount = CurrentBet;

            byte[] commandBytes = _commandSerializer.Serialize(cmd);

            Packet packetToSend = new Packet
            {
                Type = PacketType.PlayerAction,
                PayloadSize = commandBytes.Length,
                Payload = commandBytes
            };

            byte[] bytesToSend = packetToSend.ToBytes();

            _client.Send(bytesToSend);
            _showGame?.Invoke();

        }

        private void UpdatePlayerMoney(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                PlayerMoney = amount;
            }));
        }

        private void UpdateBetAmount(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                CurrentBet = amount;
            }));
        }

        public void Cleanup()
        {
            _client.PlayerMoneyUpdate -= UpdatePlayerMoney;
            _client.PlayerBetUpdate -= UpdateBetAmount;
        }
    }
}
