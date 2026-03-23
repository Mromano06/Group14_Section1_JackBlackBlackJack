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

// Matthew Romano & Brodie Arkell - March 12th, 2026 - GamplayViewModel Implementation
// The actual gameplay loop/aspects

// TODO: Send cards to display them
// TODO: Add functionality to each ICommand sister Function
namespace Client.ViewModels
{
    public class GameplayViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly double _betAmount;
        private double _playerMoney;
        private bool _isFirstCard;
        public ObservableCollection<CardViewModel> DealtPlayerCards { get; } =
            new ObservableCollection<CardViewModel>();
        public ObservableCollection<CardViewModel> DealtDealerCards { get; } =
            new ObservableCollection<CardViewModel>();

        public ICommand HitCommand { get; }
        public ICommand StandCommand { get; }
        public ICommand DoubleDownCommand { get; }

        public GameplayViewModel(NetworkClient client)
        { 
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
            if (IsFirstCard)
            {
                //DealtPlayerCards.Add(new CardViewModel("BACK"));
                IsFirstCard = false;
            }

            DealtPlayerCards.Add(new CardViewModel(cardDto));
        }

        public void DealCardToDealer(CardDto cardDto)
        {
            DealtDealerCards.Add(new CardViewModel(cardDto));
        }

        // Readonly so no setters
        public double BetAmount
        {
            get => _betAmount;

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

        private void DealInitalHand()
        {
            // DealCardToPlayer()
            // DealCardToDealer()
            // DealCardToPlayer()
            // DealCardToDealer()

        }

        private void UpdatePlayerMoney(double amount)
        {
            Debug.WriteLine($"updating the player's money to: {amount}");
            PlayerMoney = amount;
            OnPropertyChanged(nameof(PlayerMoney));
        }

        private void UpdateBetAmount(double amount)
        {
            Debug.WriteLine($"updating the player's money to: {amount}");
            PlayerMoney = amount;
            OnPropertyChanged(nameof(BetAmount));
        }

        private void Hit()
        {
            // DealCardToPlayer()
        }

        private void Stand()
        {
            // End turn
        }

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
