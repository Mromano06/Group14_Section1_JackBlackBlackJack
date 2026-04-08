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
        private readonly Action _showMainMenu;
        private double _playerMoney;
        private double _currentBet;
        private readonly PlayerCommandSerializer _commandSerializer = new PlayerCommandSerializer();

        public ICommand MaxBetCommand { get; }
        public ICommand ConfirmBetCommand { get; }
        public ICommand ResetBetCommand { get; }
        public ICommand MainMenuCommand { get; }
        public ICommand IncBetBy10Command { get; }
        public ICommand IncBetBy20Command { get; }
        public ICommand IncBetBy50Command { get; }
        public ICommand IncBetBy100Command { get; }


        public BetPlacingViewModel(NetworkClient client, Action showGame, Action ShowMenu)
        {
            _client = client;
            _showGame = showGame;
            _showMainMenu = ShowMenu;
            _playerMoney = _client.LatestPlayerMoney; // set backing field directly
            _client.PlayerMoneyUpdate += UpdatePlayerMoney;
            MaxBetCommand = new CommandRelay(MaxBet);
            ConfirmBetCommand = new CommandRelay(Confirm);
            ResetBetCommand = new CommandRelay(Reset);
            MainMenuCommand = new CommandRelay(ShowMainMenu);
            IncBetBy10Command = new CommandRelay(IncBetBy10);
            IncBetBy20Command = new CommandRelay(IncBetBy20);
            IncBetBy50Command = new CommandRelay(IncBetBy50);
            IncBetBy100Command = new CommandRelay(IncBetBy100);
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

        private void IncBetBy10()
        {
            if (_currentBet + 10 <= _playerMoney)
                CurrentBet += 10;
        }

        private void IncBetBy20()
        {
            if (_currentBet + 20 <= _playerMoney)
                CurrentBet += 20;
        }

        private void IncBetBy50()
        {
            if (_currentBet + 50 <= _playerMoney)
                CurrentBet += 50;
        }

        private void IncBetBy100()
        {
            if (_currentBet + 100 <= _playerMoney)
                CurrentBet += 100;
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

        public void ShowMainMenu()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showMainMenu?.Invoke();
            }));
        }

        public void Cleanup()
        {
            _client.PlayerMoneyUpdate -= UpdatePlayerMoney;
            _client.PlayerBetUpdate -= UpdateBetAmount;
        }
    }
}
