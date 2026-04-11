using Client.Commands;
using Client.Networking;
using Jables_Protocol;
using Jables_Protocol.DTOs;
using Jables_Protocol.Serializers;
using SharedModels.Core;
using SharedModels.Models;
using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;

// Matthew Romano & Brodie Arkell - March 12th, 2026 - GamplayViewModel Implementation
// The actual gameplay loop/aspects

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel responsible for managing the main gameplay screen.
    /// </summary>
    /// <remarks>
    /// Coordinates the core blackjack round flow by:
    /// <list type="bullet">
    /// <item><description>Receiving card updates from the server</description></item>
    /// <item><description>Tracking the player's bet and money</description></item>
    /// <item><description>Handling gameplay actions such as hit, stand, and double down</description></item>
    /// <item><description>Managing round-end and game-end transitions</description></item>
    /// <item><description>Exposing observable card collections for UI binding</description></item>
    /// </list>
    /// 
    /// This ViewModel acts as the central bridge between the gameplay UI and the
    /// networking layer gameplay.
    /// </remarks>
    public class GameplayViewModel : BaseModel
    {
        /// <summary>
        /// Shared network client used for communication with the server.
        /// </summary>
        private readonly NetworkClient _client;

        /// <summary>
        /// Stores the player's current bet amount.
        /// </summary>
        private double _betAmount;

        /// <summary>
        /// Stores the player's current available money.
        /// </summary>
        private double _playerMoney;

        /// <summary>
        /// Tracks whether the current dealer card is the first card dealt.
        /// </summary>
        private bool _isFirstCard;

        /// <summary>
        /// Determines whether the double down action is currently allowed.
        /// </summary>
        private bool _allowDouble;

        /// <summary>
        /// Tracks whether the player is still on their first turn of the round.
        /// </summary>
        private bool _isFirstTurn;

        /// <summary>
        /// Tracks whether the current round has ended.
        /// </summary>
        private bool _roundHasEnded;

        /// <summary>
        /// Stores the round result message shown to the player.
        /// </summary>
        private String _resultMessage;

        /// <summary>
        /// Action used to navigate to the round result screen.
        /// </summary>
        private readonly Action<String> _showResults;

        /// <summary>
        /// Action used to navigate back to the main menu.
        /// </summary>
        private readonly Action _showMainMenu;

        /// <summary>
        /// Action used to navigate to the loss image screen.
        /// </summary>
        private readonly Action _showLossScreen;

        /// <summary>
        /// Action used to navigate to the victory image screen.
        /// </summary>
        private readonly Action _showVictoryScreen;

        /// <summary>
        /// Collection of cards currently displayed in the player's hand.
        /// </summary>
        /// <remarks>
        /// Bound to the UI to display dealt player cards.
        /// </remarks>
        public ObservableCollection<CardViewModel> DealtPlayerCards { get; } =
            new ObservableCollection<CardViewModel>();

        /// <summary>
        /// Collection of cards currently displayed in the dealer's hand.
        /// </summary>
        /// <remarks>
        /// Bound to the UI to display dealt dealer cards.
        /// </remarks>
        public ObservableCollection<CardViewModel> DealtDealerCards { get; } =
            new ObservableCollection<CardViewModel>();

        /// <summary>
        /// Command used to request a hit action.
        /// </summary>
        public ICommand HitCommand { get; }

        /// <summary>
        /// Command used to request a stand action.
        /// </summary>
        public ICommand StandCommand { get; }

        /// <summary>
        /// Command used to request a double down action.
        /// </summary>
        public ICommand DoubleDownCommand { get; }

        /// <summary>
        /// Command used to navigate back to the main menu.
        /// </summary>
        public ICommand MainMenuCommand { get; }

        /// <summary>
        /// Serializer used to convert player commands into serialized dtos.
        /// </summary>
        PlayerCommandSerializer _commandSerializer = new PlayerCommandSerializer();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameplayViewModel"/> class.
        /// </summary>
        /// <param name="client">Shared network client.</param>
        /// <param name="ShowResults">Action used to navigate to the round result screen.</param>
        /// <param name="ShowMenu">Action used to navigate back to the main menu.</param>
        /// <param name="showLossScreen">Action used to navigate to the loss screen.</param>
        /// <param name="showVictoryScreen">Action used to navigate to the victory screen.</param>
        /// <remarks>
        /// Subscribes to all relevant gameplay update events and initializes command bindings.
        /// </remarks>
        public GameplayViewModel(NetworkClient client, Action<String> ShowResults, Action ShowMenu, Action showLossScreen, Action showVictoryScreen)
        {
            _resultMessage = String.Empty;
            _client = client;
            _showResults = ShowResults;
            _showMainMenu = ShowMenu;
            _showLossScreen = showLossScreen;
            _showVictoryScreen = showVictoryScreen;
            _client.PlayerCardUpdate += DealCardToPlayer; // subscribe to dealing player cards
            _client.DealerCardUpdate += DealCardToDealer; // subscribe to dealing dealer cards
            _client.PlayerMoneyUpdate += UpdatePlayerMoney;
            _client.PlayerBetUpdate += UpdateBetAmount;
            _client.RoundCheckUpdate += UpdateRound;
            _client.RoundResultUpdate += RoundResult;
            //_client.EndGameUpdate += FinishGame;
            HitCommand = new CommandRelay(Hit);
            StandCommand = new CommandRelay(Stand);
            DoubleDownCommand = new CommandRelay(DoubleDown);
            MainMenuCommand = new CommandRelay(ShowMainMenu);
            _isFirstCard = true;
            _roundHasEnded = false;
            _allowDouble = true;
            _isFirstTurn = true;
        }

        /// <summary>
        /// Gets or sets the player's current bet amount.
        /// </summary>
        public double BetAmount
        {
            get => _betAmount;
            set
            {
                _betAmount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the player's current available money.
        /// </summary>
        public double PlayerMoney
        {
            get => _playerMoney;
            set
            {
                _playerMoney = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the card is the first one shown.
        /// </summary>
        /// <remarks>
        /// Can be used by the UI to control display logic for face-down dealer cards.
        /// </remarks>
        public bool IsFirstCard
        {
            get 
            {
                return _isFirstCard;
            }
            set
            {
                _isFirstCard = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player is allowed to double down.
        /// </summary>
        public bool AllowDouble
        {
            get => _allowDouble;
            set
            {
                _allowDouble = value;
                OnPropertyChanged(nameof(AllowDouble));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current round has ended.
        /// </summary>
        public bool RoundHasEnded
        {
            get => _roundHasEnded;
            set
            {
                _roundHasEnded = value;
                OnPropertyChanged(nameof(RoundHasEnded));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player is on the first turn of the round.
        /// </summary>
        /// <remarks>
        /// This flag is used to determine whether actions such as double down are still legal.
        /// </remarks>
        public bool IsFirstTurn
        {
            get => _isFirstTurn;
            set
            {
                _isFirstTurn = false;
            }
        }

        /// <summary>
        /// Gets or sets the result message for the round.
        /// </summary>
        public String ResultMessage
        {
            set
            {
                _resultMessage = value;
                OnPropertyChanged();
            }
            get => _resultMessage;
        }

        /// <summary>
        /// Adds a newly dealt player card to the player's displayed hand.
        /// </summary>
        /// <param name="cardDto">The card received from the server.</param>
        /// <remarks>
        /// Executes on the UI thread so the observable collection can update bound cards.
        /// </remarks>
        public void DealCardToPlayer(CardDto cardDto)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"Attempting to deal card to player {cardDto.Rank}{cardDto.Suit}");
                DealtPlayerCards.Add(new CardViewModel(cardDto));
            }));

        }

        /// <summary>
        /// Adds a newly dealt dealer card to the dealer's displayed hand.
        /// </summary>
        /// <param name="cardDto">The card received from the server.</param>
        /// <remarks>
        /// Executes on the UI thread so the observable collection can update bound cards.
        /// </remarks>
        public async void DealCardToDealer(CardDto cardDto)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"Attempting to deal card to dealer {cardDto.Rank}{cardDto.Suit}");
                DealtDealerCards.Add(new CardViewModel(cardDto));
            }));
        }

        /// <summary>
        /// Updates the player's money when a money update is received from the server.
        /// </summary>
        /// <param name="amount">The updated player balance.</param>
        private void UpdatePlayerMoney(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                PlayerMoney = amount;
                OnPropertyChanged(nameof(PlayerMoney));
            }));
        }

        /// <summary>
        /// Updates the player's bet amount when a bet update is received from the server.
        /// </summary>
        /// <param name="amount">The updated bet amount.</param>
        private void UpdateBetAmount(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                BetAmount = amount;
                OnPropertyChanged(nameof(BetAmount));
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
        /// Sends a hit action to the server.
        /// </summary>
        /// <remarks>
        /// Disabled if the round has already ended. Marks the first turn as complete,
        /// sends the action packet, then clears the currently displayed card collections.
        /// </remarks>
        private void Hit()
        {
            if (RoundHasEnded) { return; }
            IsFirstTurn = false;
            PlayerCommandDto playerCommandDto = new PlayerCommandDto();
            playerCommandDto.Action = PlayerAction.Hit;
            playerCommandDto.BetAmount = 0;

            Packet pkt = new Packet();

            pkt.Type = PacketType.PlayerAction;
            pkt.Payload = _commandSerializer.Serialize(playerCommandDto);
            pkt.PayloadSize = 12;

            _client.Send(pkt.ToBytes());

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DealtPlayerCards.Clear();
                DealtDealerCards.Clear();
            }));
        }

        /// <summary>
        /// Sends a stand action to the server.
        /// </summary>
        /// <remarks>
        /// Disabled if the round has already ended. Marks the first turn as complete,
        /// sends the action packet, and clears the currently displayed card collections.
        /// </remarks>
        private void Stand()
        {
            if (RoundHasEnded) { return; }
            IsFirstTurn = false;
            // End turn
            PlayerCommandDto playerCommandDto = new PlayerCommandDto();
            playerCommandDto.Action = PlayerAction.Stand;
            playerCommandDto.BetAmount = 0;

            Packet pkt = new Packet();

            pkt.Type = PacketType.PlayerAction;
            pkt.Payload = _commandSerializer.Serialize(playerCommandDto);
            pkt.PayloadSize = 12;
            _client.Send(pkt.ToBytes());

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DealtPlayerCards.Clear();
                DealtDealerCards.Clear();
            }));
        }

        /// <summary>
        /// Sends a double down action to the server.
        /// </summary>
        /// <remarks>
        /// Disabled if the round has already ended, or if it is not the first turn. 
        /// Marks the first turn as complete, sends the action packet, and clears 
        /// the currently displayed card collections.
        /// </remarks>
        private void DoubleDown()
        {
            if (!IsFirstTurn || RoundHasEnded) { return; }

            if (PlayerMoney > BetAmount) // double down should not work if the player is broke
            { 

                IsFirstTurn = false;
                // End turn
                PlayerCommandDto playerCommandDto = new PlayerCommandDto();
                playerCommandDto.Action = PlayerAction.Double;
                playerCommandDto.BetAmount = 0;

                Packet pkt = new Packet();

                pkt.Type = PacketType.PlayerAction;
                pkt.Payload = _commandSerializer.Serialize(playerCommandDto);
                pkt.PayloadSize = 12;

                _client.Send(pkt.ToBytes());

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    DealtPlayerCards.Clear();
                    DealtDealerCards.Clear();
                }));

                OnPropertyChanged();

                AllowDouble = false;
            }

        }

        /// <summary>
        /// Updates round state when the server sends a new round condition.
        /// </summary>
        /// <param name="check">
        /// A value indicating whether the round state should be reset.
        /// </param>
        private void UpdateRound(bool check)
        {
            if (check)
            {
                RoundHasEnded = false;
                AllowDouble = true;
            }
        }

        /// <summary>
        /// Processes the result of the current round and prepares the result message.
        /// </summary>
        /// <param name="result">The round result reported by the server.</param>
        /// <remarks>
        /// If the game has not fully ended, a short delay is applied before navigating
        /// to the round result screen.
        /// </remarks>
        public void RoundResult(ROUND_RESULT result)
        {
            switch (result)
            {
                case ROUND_RESULT.WIN:
                    ResultMessage = "You won the round!";
                    break;
                case ROUND_RESULT.LOSS:
                    ResultMessage = "You lost the round :(";
                    break;
                case ROUND_RESULT.PUSH:
                    ResultMessage = "You pushed.";
                    break;
                case ROUND_RESULT.DEFAULT:
                    ResultMessage = "Uh oh, bad things happened.";
                    break;
            }
                Task.Delay(2000).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _showResults?.Invoke(ResultMessage);
                    }));
                });
        }

        /// <summary>
        /// Unsubscribes from all network client events associated with gameplay.
        /// </summary>
        /// <remarks>
        /// Must be called before leaving the gameplay screen to prevent duplicate
        /// event handlers when returning to gameplay.
        /// </remarks>
        public void Cleanup()
        {
            // unsubscribe to events for changing screens
            _client.PlayerCardUpdate -= DealCardToPlayer;
            _client.DealerCardUpdate -= DealCardToDealer;
            _client.PlayerMoneyUpdate -= UpdatePlayerMoney;
            _client.PlayerBetUpdate -= UpdateBetAmount;
            _client.RoundCheckUpdate -= UpdateRound;
            _client.RoundResultUpdate -= RoundResult;
        }

    }
}
