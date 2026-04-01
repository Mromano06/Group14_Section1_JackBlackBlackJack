using Client.Commands;
using Client.Networking;
using GameLogic.Actions.ActionTypes;
using Jables_Protocol.DTOs;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class ResultScreenViewModel : BaseModel
    {
        private readonly Action _showBetting;
        private readonly NetworkClient _client;
        private String _resultMessage;

        public ICommand ContinueCommand { get; }
        public ICommand MainMenuCommand { get; }

        public ResultScreenViewModel(NetworkClient client, Action ShowBetting, Action ShowMainMenu)
        {
            _client = client;
            _showBetting = ShowBetting;
            _resultMessage = "Waiting for round result..."; // Default until a message is recieved
            ContinueCommand = new CommandRelay(Continue);
            MainMenuCommand = new CommandRelay(MainMenu);
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

        public void Continue()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showBetting?.Invoke();
            }));
        }

        public void MainMenu()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _showBetting?.Invoke();
            }));
        }

        // TODO: Update the result message based on the result

    }
}
