using Client.Commands;
using Client.Networking;
using Jables_Protocol.DTOs;
using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly double _playerMoney;
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

        // TODO: Link Dispatcher to each of these functions
        private void Hit()
        {
        
        }

        private void Stand()
        {

        }

        private void DoubleDown()
        {

        }

        public void Cleanup()
        {
            _client.PlayerCardUpdate -= DealCardToPlayer;
            _client.DealerCardUpdate -= DealCardToDealer;
        }

    }
}
