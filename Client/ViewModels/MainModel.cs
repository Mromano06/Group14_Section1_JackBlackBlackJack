using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Client.ViewModels;
using Client.Networking;

// Matthew Romano - March 10th, 2026 - BaseModel implementation
// Main Model acts as the scaffolding for building menus/screens on

namespace Client.ViewModels
{
    public class MainModel : BaseModel
    {
        private BaseModel _currentViewModel; 
        public BaseModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }
        private NetworkClient _client;

        public MainModel()
        {
            _client = new NetworkClient();

            // default screen
            CurrentViewModel = new MainMenuModel(_client);
        }

    }
}
