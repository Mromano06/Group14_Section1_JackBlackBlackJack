using Client.Commands;
using Client.Networking;
using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

// Matthew Romano & Brodie Arkell - March 12th, 2026 - GamplayViewModel Implementation
// The actual gameplay loop/aspects

// TODO: Figure out the actual gameplay logic and loop
// TODO: Add functionality to each ICommand sister Function
namespace Client.ViewModels
{
    public class GameplayViewModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly double _wager;
        private readonly double _playerMoney;
        private bool _isFirstCard;
        public ObservableCollection<CardViewModel> DealtPlayerCards { get; } =
            new ObservableCollection<CardViewModel>();
        public ObservableCollection<CardViewModel> DealtDealerCards { get; } =
            new ObservableCollection<CardViewModel>();

        public ICommand HitCommand { get; }
        public ICommand StandCommand { get; }
        public ICommand DoubleDownCommand { get; }

        public GameplayViewModel(NetworkClient client, double wager, double playerMoney)
        {
            _wager = wager;
            _client = client;
            _playerMoney = playerMoney;
            HitCommand = new CommandRelay(Hit);
            StandCommand = new CommandRelay(Stand);
            DoubleDownCommand = new CommandRelay(DoubleDown);
            _isFirstCard = true;
        }

        public void DealCardToPlayer(string cardCode)
        {
            if (IsFirstCard)
            {
                DealtPlayerCards.Add(new CardViewModel("BACK"));
                IsFirstCard = false;
            }

            DealtPlayerCards.Add(new CardViewModel(cardCode));
        }

        public void DealCardToDealer(string cardCode)
        {
            DealtPlayerCards.Add(new CardViewModel(cardCode));
        }

        // Readonly so no setters
        public double Wager
        {
            get => _wager;

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

        private void Hit()
        {
            // Get the card from the 
        }

        private void Stand()
        {

        }

        private void DoubleDown()
        {

        }
    }
}
