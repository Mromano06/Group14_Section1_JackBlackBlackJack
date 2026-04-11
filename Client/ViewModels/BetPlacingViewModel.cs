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
// Handles the logic of the bet placing view model

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel responsible for handling bet placement logic.
    /// </summary>
    /// <remarks>
    /// Manages player betting interactions including:
    /// <list type="bullet">
    /// <item><description>Adjusting bet amounts</description></item>
    /// <item><description>Validating bets against available funds</description></item>
    /// <item><description>Sending bet actions to the server</description></item>
    /// <item><description>Reacting to real-time updates from the server</description></item>
    /// </list>
    /// 
    /// Acts as the bridge between the betting UI and backend networking logic.
    /// </remarks>
    public class BetPlacingViewModel : BaseModel
    {
        /// <summary>
        /// Shared network client used for server communication.
        /// </summary>
        private readonly NetworkClient _client;

        /// <summary>
        /// Action used to navigate to the gameplay screen.
        /// </summary>
        private readonly Action _showGame;

        /// <summary>
        /// Action used to navigate back to the main menu.
        /// </summary>
        private readonly Action _showMainMenu;

        /// <summary>
        /// Stores the player's current balance.
        /// </summary>
        private double _playerMoney;

        /// <summary>
        /// Stores the current bet amount.
        /// </summary>
        private double _currentBet;

        /// <summary>
        /// Serializer used to convert player commands into network packets.
        /// </summary>
        private readonly PlayerCommandSerializer _commandSerializer = new PlayerCommandSerializer();

        /// <summary>
        /// Command to set the current bet to the maximum available amount.
        /// </summary>
        public ICommand MaxBetCommand { get; }

        /// <summary>
        /// Command to the confirm and place a bet.
        /// </summary>
        public ICommand ConfirmBetCommand { get; }

        /// <summary>
        /// Command to set the current bet to 0.
        /// </summary>
        public ICommand ResetBetCommand { get; }

        /// <summary>
        /// Command to navigate to the MainMenu.
        /// </summary>
        public ICommand MainMenuCommand { get; }

        /// <summary>
        /// Command to increment the current bet by 10.
        /// </summary>
        public ICommand IncBetBy10Command { get; }

        /// <summary>
        /// Command to increment the current bet by 20.
        /// </summary>
        public ICommand IncBetBy20Command { get; }

        /// <summary>
        /// Command to increment the current bet by 50.
        /// </summary>
        public ICommand IncBetBy50Command { get; }

        /// <summary>
        /// Command to increment the current bet by 100.
        /// </summary>
        public ICommand IncBetBy100Command { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetPlacingViewModel"/> class.
        /// </summary>
        /// <param name="client">Shared network client.</param>
        /// <param name="showGame">Action to navigate to gameplay.</param>
        /// <param name="ShowMenu">Action to navigate to the main menu.</param>
        /// <remarks>
        /// Subscribes to player money updates and initializes command bindings.
        /// </remarks>
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

        /// <summary>
        /// Gets or sets the current bet amount.
        /// </summary>
        /// <remarks>
        /// Ensures the bet:
        /// <list type="bullet">
        /// <item><description>Does not exceed available funds</description></item>
        /// <item><description>Is not negative</description></item>
        /// </list>
        /// </remarks>
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
        /// <summary>
        /// Gets or sets the player's current balance.
        /// </summary>
        public double PlayerMoney
        {
            get => _playerMoney;
            set
            {
                _playerMoney = value;
                OnPropertyChanged(nameof(PlayerMoney));
            }
        }

        /// <summary>
        /// Increases the current bet by 10 if valid.
        /// </summary>
        private void IncBetBy10()
        {
            if (_currentBet + 10 <= _playerMoney)
                CurrentBet += 10;
        }

        /// <summary>
        /// Increases the current bet by 20 if valid.
        /// </summary>
        private void IncBetBy20()
        {
            if (_currentBet + 20 <= _playerMoney)
                CurrentBet += 20;
        }

        /// <summary>
        /// Increases the current bet by 50 if valid.
        /// </summary>
        private void IncBetBy50()
        {
            if (_currentBet + 50 <= _playerMoney)
                CurrentBet += 50;
        }

        /// <summary>
        /// Increases the current bet by 100 if valid.
        /// </summary>
        private void IncBetBy100()
        {
            if (_currentBet + 100 <= _playerMoney)
                CurrentBet += 100;
        }

        /// <summary>
        /// Sets the current bet to the player's maximum available money.
        /// </summary>
        private void MaxBet()
        {
            CurrentBet = _playerMoney;
        }

        /// <summary>
        /// Resets the current bet to zero.
        /// </summary>
        private void Reset()
        {
            CurrentBet = 0;
        }

        /// <summary>
        /// Confirms the bet and sends it to the server.
        /// </summary>
        /// <remarks>
        /// Serializes the bet into a packet and sends it via the network client,
        /// then navigates to the gameplay screen.
        /// </remarks>e
        private void Confirm()
        {
            if (CurrentBet >= 10)
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

        }

        /// <summary>
        /// Updates the player's money based on server events.
        /// </summary>
        /// <param name="amount">The updated balance.</param>
        private void UpdatePlayerMoney(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                PlayerMoney = amount;
            }));
        }

        /// <summary>
        /// Updates the bet amount based on server events.
        /// </summary>
        /// <param name="amount">The updated bet value.</param>
        private void UpdateBetAmount(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                CurrentBet = amount;
            }));
        }

        /// <summary>
        /// Navigates back to the main menu.
        /// </summary>
        public void ShowMainMenu()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showMainMenu?.Invoke();
            }));
        }

        /// <summary>
        /// Cleans up event subscriptions to prevent memory leaks.
        /// </summary>
        /// <remarks>
        /// Should be called when leaving the betting screen.
        /// </remarks>
        public void Cleanup()
        {
            _client.PlayerMoneyUpdate -= UpdatePlayerMoney;
            _client.PlayerBetUpdate -= UpdateBetAmount;
        }
    }
}
