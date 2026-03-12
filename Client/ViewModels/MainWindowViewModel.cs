using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BaseModel
    {
        private BaseModel _currentViewModel;
        // Client shared across all screens
        private readonly NetworkClient _client;

        private int _playerMoney;
        private int _latestBet;

        public BaseModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            _client = new NetworkClient();
            CurrentViewModel = new MainMenuModel(_client, ShowMenu);
;        }

        public int PlayerMoney
        {
            get => _playerMoney;
            set
            {
                if (value >= 0)
                    _playerMoney = value;
            }
        }

        public int LatestBet
        {
            get => _latestBet;
            set
            {
                if (value <= this.PlayerMoney)
                    _latestBet = value;
            }
        }

        public void ShowGame()
        {
            CurrentViewModel = new BetPlacingViewModel(_client, _playerMoney);
        }

        public void ShowMenu()
        {
            CurrentViewModel = new MainMenuModel(_client, ShowGame);
        }


    }
}
