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


// Matthew Romano & Brodie Arkell - March 12th, 2026 - GamplayViewModel Implementation
// The actual gameplay loop/aspects

// TODO: Send cards to display them
namespace Client.ViewModels
{
    public class GameplayViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private double _betAmount;
        private double _playerMoney;
        private bool _isFirstCard;
        private bool _allowDouble = true;
        private readonly Action _showBetting;

        public ObservableCollection<CardViewModel> DealtPlayerCards { get; } =
            new ObservableCollection<CardViewModel>();
        public ObservableCollection<CardViewModel> DealtDealerCards { get; } =
            new ObservableCollection<CardViewModel>();

        public ICommand HitCommand { get; }
        public ICommand StandCommand { get; }
        public ICommand DoubleDownCommand { get; }

        PlayerCommandSerializer _commandSerializer = new PlayerCommandSerializer();

        public GameplayViewModel(NetworkClient client, double betAmount, double playerMoney, Action ShowBetting)
        {
            _betAmount = betAmount;
            _playerMoney = playerMoney;
            _client = client;
            _showBetting = ShowBetting;
            _client.PlayerCardUpdate += DealCardToPlayer; // subscribe to dealing player cards
            _client.DealerCardUpdate += DealCardToDealer; // subscribe to dealing dealer cards
            _client.PlayerMoneyUpdate += UpdatePlayerMoney;
            _client.PlayerBetUpdate += UpdateBetAmount;
            _client.RoundCheckUpdate += UpdateRound;
            HitCommand = new CommandRelay(Hit);
            StandCommand = new CommandRelay(Stand);
            DoubleDownCommand = new CommandRelay(DoubleDown);
            _isFirstCard = true;
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

        public void DealCardToPlayer(CardDto cardDto)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"Attempting to deal card to player {cardDto.Rank}{cardDto.Suit}");
                DealtPlayerCards.Add(new CardViewModel(cardDto));
            }));

        }

        public void DealCardToDealer(CardDto cardDto)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
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
                //OnPropertyChanged(nameof(PlayerMoney));
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

        private void Hit()
        {
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
                IsFirstCard = true;
            }));
        }

        private void Stand()
        {
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
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _showBetting?.Invoke();
                }));
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
        }

    }
}
