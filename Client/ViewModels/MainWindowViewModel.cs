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

        public void ShowGame()
        {
            CurrentViewModel = new BetPlacingViewModel(_client);
        }

        public void ShowMenu()
        {
            CurrentViewModel = new MainMenuModel(_client, ShowGame);
        }


    }
}
