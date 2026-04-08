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

namespace Client.ViewModels
{
    public class VictoryScreenViewModel: BaseModel
    {
        private readonly Action _showMainMenu;
        private readonly NetworkClient _client;
        private string _victoryImagePath;

        public VictoryScreenViewModel(NetworkClient client, Action showMainMenu)
        {
            _client = client;
            _showMainMenu = showMainMenu;
            _victoryImagePath = "pack://application:,,,/Loser.jpg"; // Default winner image path

            StartAutoReturnTimer(); // navigates back to the main menu
        }

        public string VictoryImagePath
        {
            get => _victoryImagePath;
            set
            {
                _victoryImagePath = value;
                OnPropertyChanged(nameof(VictoryImagePath));
            }
        }

        private async void StartAutoReturnTimer()
        {
            await Task.Delay(12000); // 12 seconds, change to 10000 or 15000 if you want

            Application.Current.Dispatcher.Invoke(() =>
            {
                _showMainMenu?.Invoke();
            });
        }
    }

}
