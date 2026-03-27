using Client.Commands;
using Client.Networking;
using Jables_Protocol.DTOs;
using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;


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
        public ObservableCollection<CardViewModel> DealtPlayerCards { get; } =
            new ObservableCollection<CardViewModel>();
        public ObservableCollection<CardViewModel> DealtDealerCards { get; } =
            new ObservableCollection<CardViewModel>();

        public ICommand HitCommand { get; }
        public ICommand StandCommand { get; }
        public ICommand DoubleDownCommand { get; }

        public GameplayViewModel(NetworkClient client, double betAmount, double playerMoney)
        {
            _betAmount = betAmount;
            _playerMoney = playerMoney;
            _client = client;
            _client.PlayerCardUpdate += DealCardToPlayer; // subscribe to dealing player cards
            _client.DealerCardUpdate += DealCardToDealer; // subscribe to dealing dealer cards
            _client.PlayerMoneyUpdate += UpdatePlayerMoney;
            _client.PlayerBetUpdate += UpdateBetAmount;
            HitCommand = new CommandRelay(Hit);
            StandCommand = new CommandRelay(Stand);
            DoubleDownCommand = new CommandRelay(DoubleDown);
            _isFirstCard = true;
        }

        public void DealCardToPlayer(CardDto cardDto)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsFirstCard)
                {
                    //DealtPlayerCards.Add(new CardViewModel("BACK"));
                    IsFirstCard = false;
                }

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

        // Readonly so no setters
        public double BetAmount
        {
            get => _betAmount;
            set
            {
                _betAmount = value;
            }
        }

        public double PlayerMoney
        {
            get => _playerMoney;
            set
            {
                _playerMoney = value;
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

        // TODO: Send the inital hands for the dealer and player
        private void DealInitalHand()
        {
            // DealCardToPlayer()
            // DealCardToDealer()
            // DealCardToPlayer()
            // DealCardToDealer()

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

        // TODO: Have the dispatcher send the card number to this function
        private void Hit()
        {
            // DealCardToPlayer()
        }

        // TODO: Have the dispatcher send an end round message to the server
        private void Stand()
        {
            // End turn
        }

        // TODO: Have the dispatcher send the card number to this function
        // TODO: Have the dispatcher send an end round message to the server
        private void DoubleDown()
        {
            // DealCardToPlayer()
            // End turn
        }

        public void Cleanup()
        {
            // unsubscribe to events for changing screens
            _client.PlayerCardUpdate -= DealCardToPlayer;
            _client.DealerCardUpdate -= DealCardToDealer;
        }

    }
}
