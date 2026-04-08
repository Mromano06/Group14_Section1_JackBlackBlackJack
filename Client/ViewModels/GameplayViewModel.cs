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
    public class GameplayViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private double _betAmount;
        private double _playerMoney;
        private bool _isFirstCard;
        private bool _allowDouble = true;
        private bool _isFirstTurn = true;
        private bool _roundHasEnded;
        private bool _gameHasEnded = false;
        private String _resultMessage;
        private readonly Action<String> _showResults;
        private readonly Action _showMainMenu;
        private readonly Action _showLossScreen;
        private readonly Action _showVictoryScreen;

        public ObservableCollection<CardViewModel> DealtPlayerCards { get; } =
            new ObservableCollection<CardViewModel>();
        public ObservableCollection<CardViewModel> DealtDealerCards { get; } =
            new ObservableCollection<CardViewModel>();

        public ICommand HitCommand { get; }
        public ICommand StandCommand { get; }
        public ICommand DoubleDownCommand { get; }
        public ICommand MainMenuCommand { get; }

        PlayerCommandSerializer _commandSerializer = new PlayerCommandSerializer();

        public GameplayViewModel(NetworkClient client, Action<String> ShowResults, Action ShowMenu, Action showLossScreen, Action showVictoryScreen)
        {
            _resultMessage = String.Empty;
            _client = client;
            _showResults = ShowResults;
            _showMainMenu = ShowMenu;
            _showLossScreen = showLossScreen;
            _showVictoryScreen = showVictoryScreen;
            _client.GameResultUpdate += FinishGame;
            _client.PlayerCardUpdate += DealCardToPlayer; // subscribe to dealing player cards
            _client.DealerCardUpdate += DealCardToDealer; // subscribe to dealing dealer cards
            _client.PlayerMoneyUpdate += UpdatePlayerMoney;
            _client.PlayerBetUpdate += UpdateBetAmount;
            _client.RoundCheckUpdate += UpdateRound;
            _client.RoundResultUpdate += RoundResult;
            HitCommand = new CommandRelay(Hit);
            StandCommand = new CommandRelay(Stand);
            DoubleDownCommand = new CommandRelay(DoubleDown);
            MainMenuCommand = new CommandRelay(ShowMainMenu);
            _isFirstCard = true;
            _roundHasEnded = false;
            _gameHasEnded = false;
        }

        // Readonly so no setters
        public double BetAmount
        {
            get => _betAmount;
            set
            {
                _betAmount = value;
                OnPropertyChanged();
            }
        }

        public double PlayerMoney
        {
            get => _playerMoney;
            set
            {
                _playerMoney = value;
                OnPropertyChanged();
            }
        }

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

        public bool AllowDouble
        {
            get => _allowDouble;
            set
            {
                _allowDouble = value;
                OnPropertyChanged(nameof(AllowDouble));
            }
        }

        public bool RoundHasEnded
        {
            get => _roundHasEnded;
            set
            {
                _roundHasEnded = value;
                OnPropertyChanged(nameof(RoundHasEnded));
            }
        }

        public bool GameHasEnded
        {
            get => _gameHasEnded;
            set
            {
                _gameHasEnded = value;
                OnPropertyChanged(nameof(GameHasEnded));
            }
        }

        public bool IsFirstTurn
        {
            get => _isFirstTurn;
            set
            {
                _isFirstTurn = false;
            }
        }

        public String ResultMessage
        {
            set
            {
                _resultMessage = value;
                OnPropertyChanged();
            }
            get => _resultMessage;
        }

        public void DealCardToPlayer(CardDto cardDto)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"Attempting to deal card to player {cardDto.Rank}{cardDto.Suit}");
                DealtPlayerCards.Add(new CardViewModel(cardDto));
            }));

        }

        public async void DealCardToDealer(CardDto cardDto)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"Attempting to deal card to dealer {cardDto.Rank}{cardDto.Suit}");
                DealtDealerCards.Add(new CardViewModel(cardDto));
            }));
        }

        private void UpdatePlayerMoney(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                PlayerMoney = amount;
                OnPropertyChanged(nameof(PlayerMoney));
            }));
        }

        private void UpdateBetAmount(double amount)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"updating the player's money to: {amount}");
                BetAmount = amount;
                OnPropertyChanged(nameof(BetAmount));
            }));
        }

        public void ShowMainMenu()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showMainMenu?.Invoke();
            }));
        }

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

        private void DoubleDown()
        {
            if (!IsFirstTurn || RoundHasEnded) { return; }

            if (PlayerMoney < BetAmount) // double down should not work if the player is broke

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

        private void UpdateRound(bool check)
        {
            if (check)
            {
                RoundHasEnded = false;
                AllowDouble = true;
            }
        }

        private void FinishGame(GameResult result)
        {
            GameHasEnded = true;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (result == GameResult.PLAYER_LOSE)
                {
                    _showLossScreen?.Invoke();
                }
                else if (result == GameResult.PLAYER_WIN)
                {
                    _showVictoryScreen?.Invoke();
                }
            }));
        }

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

            if (!GameHasEnded)
            {
                Task.Delay(2000).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!GameHasEnded)
                            _showResults?.Invoke(ResultMessage);
                    }));
                });
            }
        }

        public void Cleanup()
        {
            // unsubscribe to events for changing screens
            _client.PlayerCardUpdate -= DealCardToPlayer;
            _client.DealerCardUpdate -= DealCardToDealer;
            _client.PlayerMoneyUpdate -= UpdatePlayerMoney;
            _client.PlayerBetUpdate -= UpdateBetAmount;
            _client.RoundCheckUpdate -= UpdateRound;
            _client.RoundResultUpdate -= RoundResult;
            _client.GameResultUpdate -= FinishGame;  
        }

    }
}
